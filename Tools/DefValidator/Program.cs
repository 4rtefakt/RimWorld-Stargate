using System.Reflection;
using System.Xml.Linq;

namespace DefValidator
{
    /// <summary>
    /// Validateur statique des Defs XML d'un mod RimWorld.
    ///
    /// Charge (en métadonnées seulement) les assemblies du jeu et des mods dépendants,
    /// puis vérifie pour chaque Def du mod :
    ///   - que le type de Def et les attributs Class="..." existent ;
    ///   - que chaque balise correspond à un champ réel du type ciblé (récursivement) ;
    ///   - que les valeurs d'enum et de System.Type sont valides ;
    ///   - que chaque référence vers une autre Def pointe vers un defName connu
    ///     (défini par le mod, par une dépendance, ou par une classe [DefOf] du jeu).
    ///
    /// Usage :
    ///   DefValidator --mod &lt;dossier du mod&gt; --asm &lt;dossier de DLL&gt; [--asm ...]
    ///                [--dep &lt;dossier d'un mod dépendant&gt; ...] [--core &lt;mscorlib.dll&gt;]
    /// Code de sortie : 0 si aucune erreur, 1 sinon (les avertissements ne font pas échouer).
    /// </summary>
    public static class Program
    {
        private static readonly string[] ImplicitNamespaces =
        {
            "Verse", "RimWorld", "Verse.AI", "Verse.AI.Group", "Verse.Sound", "Verse.Grammar",
            "RimWorld.Planet", "RimWorld.BaseGen", "RimWorld.QuestGen", "RimWorld.SketchGen",
            "RimWorld.Utility", "Verse.Noise", "Verse.Steam", "RimWorld.IO", "Verse.Glow"
        };

        private static readonly Dictionary<string, Type> TypeByFullName = new Dictionary<string, Type>();
        private static Type defType;

        // defName -> types de Def (noms complets) sous lesquels il est connu, et d'où il vient.
        private static readonly Dictionary<string, HashSet<string>> KnownDefs = new Dictionary<string, HashSet<string>>();
        // Noms abstraits/parents (attribut Name="...") disponibles, par type racine.
        private static readonly Dictionary<string, HashSet<string>> KnownParents = new Dictionary<string, HashSet<string>>();
        // Jetons rencontrés dans le XML des dépendances (indice faible d'existence d'une Def vanilla).
        private static readonly HashSet<string> DependencyTokens = new HashSet<string>();
        // Références vanilla déjà validées en jeu (Type:defName), cf. vanilla-verified.txt.
        private static readonly HashSet<string> Baseline = new HashSet<string>();
        private static readonly SortedSet<string> Unverified = new SortedSet<string>();

        private static readonly List<string> Errors = new List<string>();
        private static readonly List<string> Warnings = new List<string>();
        private static readonly SortedSet<string> WeakRefs = new SortedSet<string>();
        private static readonly SortedSet<string> VanillaParents = new SortedSet<string>();

        public static int Main(string[] args)
        {
            string modDir = null;
            string core = null;
            var asmDirs = new List<string>();
            var depDirs = new List<string>();
            for (int i = 0; i < args.Length - 1; i++)
            {
                switch (args[i])
                {
                    case "--mod": modDir = args[++i]; break;
                    case "--asm": asmDirs.Add(args[++i]); break;
                    case "--dep": depDirs.Add(args[++i]); break;
                    case "--core": core = args[++i]; break;
                    case "--baseline":
                        foreach (string line in File.ReadAllLines(args[++i]))
                        {
                            string l = line.Trim();
                            if (l.Length > 0 && !l.StartsWith("#")) Baseline.Add(l);
                        }
                        break;
                }
            }
            if (modDir == null || asmDirs.Count == 0)
            {
                Console.Error.WriteLine("Usage: DefValidator --mod <dir> --asm <dir> [--asm <dir>] [--dep <dir>] [--core <mscorlib.dll>]");
                return 2;
            }

            LoadAssemblies(asmDirs, core, Path.Combine(modDir, "Assemblies"));

            // Defs des dépendances : connues, et sources d'indices pour les Defs vanilla.
            foreach (string dep in depDirs)
            {
                foreach (XDocument doc in LoadDefFiles(dep, out _))
                {
                    IndexDefs(doc, collectTokens: true);
                }
            }

            // Defs du mod.
            List<(XDocument doc, string file)> modDocs = new List<(XDocument, string)>();
            foreach (XDocument doc in LoadDefFiles(modDir, out List<string> files))
            {
                modDocs.Add((doc, files[modDocs.Count]));
                IndexDefs(doc, collectTokens: false);
            }

            CheckDuplicates(modDocs);
            foreach ((XDocument doc, string file) in modDocs)
            {
                ValidateFile(doc, Path.GetRelativePath(modDir, file));
            }

            CheckTranslationKeys(modDir);

            foreach (string w in Warnings) Console.WriteLine("AVERT  " + w);
            foreach (string e in Errors) Console.WriteLine("ERREUR " + e);
            if (VanillaParents.Count > 0)
            {
                Console.WriteLine("INFO   parents supposés vanilla (non vérifiables hors jeu) : " + string.Join(", ", VanillaParents));
            }
            if (WeakRefs.Count > 0)
            {
                Console.WriteLine("INFO   références vanilla vues seulement dans le XML des dépendances : " + string.Join(", ", WeakRefs));
            }
            if (Unverified.Count > 0)
            {
                Console.WriteLine("INFO   références non vérifiées (Type:defName, une par ligne) :");
                foreach (string u in Unverified) Console.WriteLine("       " + u);
            }
            Console.WriteLine($"Bilan : {modDocs.Count} fichiers, {Errors.Count} erreur(s), {Warnings.Count} avertissement(s).");
            return Errors.Count == 0 ? 0 : 1;
        }

        // ------------------------------------------------------------------ traductions

        /// <summary>Chaque "Clé".Translate(...) du C# doit exister dans chaque langue Keyed du mod.</summary>
        private static void CheckTranslationKeys(string modDir)
        {
            string source = Path.Combine(modDir, "Source");
            string languages = Path.Combine(modDir, "Languages");
            if (!Directory.Exists(source) || !Directory.Exists(languages)) return;

            var used = new SortedSet<string>();
            var regex = new System.Text.RegularExpressions.Regex("\"([A-Za-z0-9_]+)\"\\.Translate\\(");
            foreach (string f in Directory.GetFiles(source, "*.cs", SearchOption.AllDirectories))
            {
                foreach (System.Text.RegularExpressions.Match m in regex.Matches(File.ReadAllText(f)))
                {
                    used.Add(m.Groups[1].Value);
                }
            }

            foreach (string lang in Directory.GetDirectories(languages))
            {
                string keyed = Path.Combine(lang, "Keyed");
                if (!Directory.Exists(keyed)) continue;
                var keys = new HashSet<string>();
                foreach (string f in Directory.GetFiles(keyed, "*.xml", SearchOption.AllDirectories))
                {
                    try
                    {
                        foreach (XElement e in XDocument.Load(f).Root.Elements()) keys.Add(e.Name.LocalName);
                    }
                    catch (Exception ex)
                    {
                        Errors.Add($"{f} : XML invalide ({ex.Message})");
                    }
                }
                foreach (string key in used)
                {
                    // Les clés sans préfixe SG_ viennent du jeu ou des dépendances.
                    if (key.StartsWith("SG_") && !keys.Contains(key))
                    {
                        Errors.Add($"Languages/{Path.GetFileName(lang)} : clé de traduction \"{key}\" manquante (utilisée dans Source/)");
                    }
                }
            }
        }

        // ------------------------------------------------------------------ assemblies

        private static void LoadAssemblies(List<string> asmDirs, string core, string modAssemblies)
        {
            var paths = new List<string>();
            foreach (string d in asmDirs)
            {
                paths.AddRange(Directory.GetFiles(d, "*.dll"));
                string facades = Path.Combine(d, "Facades");
                if (Directory.Exists(facades)) paths.AddRange(Directory.GetFiles(facades, "*.dll"));
            }
            if (Directory.Exists(modAssemblies)) paths.AddRange(Directory.GetFiles(modAssemblies, "*.dll"));
            if (core != null) paths.Add(core);

            // Un seul fichier par nom d'assembly (le premier gagne).
            var unique = paths.GroupBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase).Select(g => g.First()).ToList();
            var resolver = new PathAssemblyResolver(unique);
            var mlc = new MetadataLoadContext(resolver, core != null ? Path.GetFileNameWithoutExtension(core) : "mscorlib");

            foreach (string p in unique)
            {
                Assembly asm;
                try { asm = mlc.LoadFromAssemblyPath(p); }
                catch { continue; }
                Type[] types;
                try { types = asm.GetTypes(); }
                catch (ReflectionTypeLoadException ex) { types = ex.Types.Where(t => t != null).ToArray(); }
                catch { continue; }
                foreach (Type t in types)
                {
                    string name;
                    try { name = t.FullName; } catch { continue; }
                    if (name != null && !TypeByFullName.ContainsKey(name)) TypeByFullName[name] = t;
                }
            }

            TypeByFullName.TryGetValue("Verse.Def", out defType);
            if (defType == null) throw new Exception("Verse.Def introuvable : vérifier --asm (Assembly-CSharp.dll).");

            // Les champs des classes [DefOf] sont autant de defNames vanilla garantis.
            foreach (Type t in TypeByFullName.Values)
            {
                bool isDefOf;
                try { isDefOf = t.GetCustomAttributesData().Any(a => a.AttributeType.Name == "DefOf"); }
                catch { continue; }
                if (!isDefOf) continue;
                foreach (FieldInfo f in t.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    try
                    {
                        if (IsDef(f.FieldType)) AddKnown(f.Name, f.FieldType.FullName);
                    }
                    catch { }
                }
            }
        }

        private static Type ResolveType(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            name = name.Trim();
            if (TypeByFullName.TryGetValue(name, out Type t)) return t;
            foreach (string ns in ImplicitNamespaces)
            {
                if (TypeByFullName.TryGetValue(ns + "." + name, out t)) return t;
            }
            // Dernier recours : nom court unique dans n'importe quel namespace (comme GenTypes).
            var matches = TypeByFullName.Values.Where(x => x.Name == name).ToList();
            return matches.Count == 1 ? matches[0] : null;
        }

        private static bool IsDef(Type t) => t != null && defType != null && IsSubclassOrSame(t, defType);

        private static bool IsSubclassOrSame(Type t, Type baseType)
        {
            for (Type cur = t; cur != null; cur = SafeBase(cur))
            {
                if (cur.FullName == baseType.FullName) return true;
            }
            return false;
        }

        private static Type SafeBase(Type t)
        {
            try { return t.BaseType; } catch { return null; }
        }

        private static FieldInfo FindField(Type type, string name)
        {
            for (Type cur = type; cur != null; cur = SafeBase(cur))
            {
                FieldInfo f;
                try
                {
                    f = cur.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                }
                catch { f = null; }
                if (f != null) return f;

                // [LoadAlias("ancienNom")]
                FieldInfo[] all;
                try { all = cur.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly); }
                catch { continue; }
                foreach (FieldInfo candidate in all)
                {
                    try
                    {
                        if (candidate.GetCustomAttributesData().Any(a => a.AttributeType.Name == "LoadAliasAttribute"
                            && a.ConstructorArguments.Count > 0 && (string)a.ConstructorArguments[0].Value == name))
                        {
                            return candidate;
                        }
                    }
                    catch { }
                }
            }
            return null;
        }

        private static bool HasCustomLoader(Type t)
        {
            for (Type cur = t; cur != null; cur = SafeBase(cur))
            {
                try
                {
                    if (cur.GetMethod("LoadDataFromXmlCustom", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly) != null)
                    {
                        return true;
                    }
                }
                catch { }
            }
            return false;
        }

        // ------------------------------------------------------------------ indexation

        private static IEnumerable<XDocument> LoadDefFiles(string root, out List<string> files)
        {
            files = new List<string>();
            var docs = new List<XDocument>();
            foreach (string sub in new[] { "Defs", Path.Combine("1.6", "Defs") })
            {
                string dir = Path.Combine(root, sub);
                if (!Directory.Exists(dir)) continue;
                foreach (string f in Directory.GetFiles(dir, "*.xml", SearchOption.AllDirectories).OrderBy(x => x, StringComparer.Ordinal))
                {
                    try
                    {
                        docs.Add(XDocument.Load(f));
                        files.Add(f);
                    }
                    catch (Exception ex)
                    {
                        Errors.Add($"{f} : XML invalide ({ex.Message})");
                    }
                }
            }
            return docs;
        }

        private static void AddKnown(string defName, string typeFullName)
        {
            if (!KnownDefs.TryGetValue(defName, out HashSet<string> set))
            {
                KnownDefs[defName] = set = new HashSet<string>();
            }
            set.Add(typeFullName);
        }

        private static void IndexDefs(XDocument doc, bool collectTokens)
        {
            if (doc.Root == null) return;
            foreach (XElement def in doc.Root.Elements())
            {
                Type t = ResolveType((string)def.Attribute("Class") ?? def.Name.LocalName);
                string typeName = t?.FullName ?? def.Name.LocalName;
                string defName = def.Element("defName")?.Value.Trim();
                if (!string.IsNullOrEmpty(defName)) AddKnown(defName, typeName);

                string name = (string)def.Attribute("Name");
                if (name != null)
                {
                    string rootKey = def.Name.LocalName;
                    if (!KnownParents.TryGetValue(rootKey, out HashSet<string> set)) KnownParents[rootKey] = set = new HashSet<string>();
                    set.Add(name);
                }

                if (collectTokens)
                {
                    foreach (XElement e in def.DescendantsAndSelf())
                    {
                        DependencyTokens.Add(e.Name.LocalName);
                        if (!e.HasElements && !string.IsNullOrWhiteSpace(e.Value)) DependencyTokens.Add(e.Value.Trim());
                        string parent = (string)e.Attribute("ParentName");
                        if (parent != null) DependencyTokens.Add("parent:" + e.Name.LocalName + ":" + parent);
                    }
                }
            }
        }

        private static void CheckDuplicates(List<(XDocument doc, string file)> docs)
        {
            var seen = new Dictionary<string, string>();
            foreach ((XDocument doc, string file) in docs)
            {
                if (doc.Root == null) continue;
                foreach (XElement def in doc.Root.Elements())
                {
                    string defName = def.Element("defName")?.Value.Trim();
                    if (string.IsNullOrEmpty(defName)) continue;
                    string key = def.Name.LocalName + "/" + defName;
                    if (seen.TryGetValue(key, out string other))
                    {
                        Errors.Add($"{Path.GetFileName(file)} : {key} défini deux fois (aussi dans {Path.GetFileName(other)})");
                    }
                    else
                    {
                        seen[key] = file;
                    }
                }
            }
        }

        // ------------------------------------------------------------------ validation

        private static void ValidateFile(XDocument doc, string file)
        {
            if (doc.Root == null || doc.Root.Name.LocalName != "Defs")
            {
                Errors.Add($"{file} : la racine doit être <Defs>");
                return;
            }
            foreach (XElement def in doc.Root.Elements())
            {
                string defName = def.Element("defName")?.Value.Trim() ?? (string)def.Attribute("Name") ?? "?";
                string ctx = $"{file} [{def.Name.LocalName} {defName}]";

                Type t = ResolveType((string)def.Attribute("Class") ?? def.Name.LocalName);
                if (t == null)
                {
                    Errors.Add($"{ctx} : type de Def inconnu");
                    continue;
                }
                if (!IsDef(t))
                {
                    Errors.Add($"{ctx} : {t.FullName} n'est pas une Def");
                    continue;
                }

                bool isAbstract = string.Equals((string)def.Attribute("Abstract"), "True", StringComparison.OrdinalIgnoreCase);
                if (!isAbstract && def.Element("defName") == null)
                {
                    Errors.Add($"{ctx} : defName manquant");
                }

                string parent = (string)def.Attribute("ParentName");
                if (parent != null)
                {
                    bool known = (KnownParents.TryGetValue(def.Name.LocalName, out HashSet<string> set) && set.Contains(parent))
                                 || Baseline.Contains("Parent:" + def.Name.LocalName + ":" + parent);
                    if (!known)
                    {
                        if (DependencyTokens.Contains("parent:" + def.Name.LocalName + ":" + parent))
                        {
                            VanillaParents.Add(parent + " (vu dans les dépendances)");
                        }
                        else
                        {
                            Warnings.Add($"{ctx} : ParentName \"{parent}\" introuvable dans le mod et ses dépendances (parent vanilla non vérifié)");
                            VanillaParents.Add(parent);
                        }
                    }
                }

                ValidateObject(def, t, ctx, isRoot: true);
            }
        }

        private static void ValidateObject(XElement node, Type type, string ctx, bool isRoot)
        {
            foreach (XElement child in node.Elements())
            {
                if (child.Attribute("MayRequire") != null || child.Attribute("MayRequireAnyOf") != null)
                {
                    continue; // contenu conditionnel à un mod optionnel
                }
                string fieldName = child.Name.LocalName;
                FieldInfo field = FindField(type, fieldName);
                if (field == null)
                {
                    if (string.Equals((string)child.Attribute("IgnoreIfNoMatchingField"), "True", StringComparison.OrdinalIgnoreCase)) continue;
                    Errors.Add($"{ctx} : champ <{fieldName}> inexistant dans {type.FullName}");
                    continue;
                }
                ValidateValue(child, field.FieldType, ctx + "/" + fieldName);
            }
        }

        private static void ValidateValue(XElement node, Type type, string ctx)
        {
            if (string.Equals((string)node.Attribute("IsNull"), "True", StringComparison.OrdinalIgnoreCase)) return;

            string className = (string)node.Attribute("Class");
            if (className != null)
            {
                Type sub = ResolveType(className);
                if (sub == null)
                {
                    Errors.Add($"{ctx} : Class=\"{className}\" introuvable");
                    return;
                }
                if (!IsSubclassOrSame(sub, type) && !type.IsInterface)
                {
                    Errors.Add($"{ctx} : Class=\"{className}\" n'hérite pas de {type.FullName}");
                    return;
                }
                type = sub;
            }

            // Nullable<T>
            if (type.IsGenericType && type.GetGenericTypeDefinition().FullName == "System.Nullable`1")
            {
                type = type.GetGenericArguments()[0];
            }

            if (IsDef(type))
            {
                if (node.HasElements)
                {
                    Errors.Add($"{ctx} : une référence de Def ({type.Name}) doit être un defName, pas un objet");
                    return;
                }
                CheckDefRef(node.Value.Trim(), type, ctx);
                return;
            }

            if (type.FullName == "System.Type")
            {
                if (ResolveType(node.Value) == null) Errors.Add($"{ctx} : type \"{node.Value.Trim()}\" introuvable");
                return;
            }

            if (type.IsEnum)
            {
                CheckEnum(node.Value, type, ctx);
                return;
            }

            if (type.IsGenericType)
            {
                string gen = type.GetGenericTypeDefinition().FullName;
                if (gen == "System.Collections.Generic.List`1")
                {
                    ValidateList(node, type.GetGenericArguments()[0], ctx);
                    return;
                }
                if (gen == "System.Collections.Generic.Dictionary`2")
                {
                    return; // format libre (li/key/value ou balises), non vérifié
                }
            }

            if (!node.HasElements)
            {
                return; // valeur scalaire parsée par ParseHelper (int, IntRange, Vector3, Color…)
            }

            if (HasCustomLoader(type))
            {
                return; // chargeur XML personnalisé : structure libre
            }

            ValidateObject(node, type, ctx, isRoot: false);
        }

        private static void ValidateList(XElement node, Type itemType, string ctx)
        {
            bool custom = HasCustomLoader(itemType);
            foreach (XElement li in node.Elements())
            {
                if (li.Attribute("MayRequire") != null || li.Attribute("MayRequireAnyOf") != null) continue;
                if (li.Name.LocalName == "li")
                {
                    ValidateValue(li, itemType, ctx + "/li");
                }
                else if (custom)
                {
                    // Format <DefName>valeur</DefName> (StatModifier, ThingDefCountClass, PawnGenOption…).
                    Type refType = FirstDefFieldType(itemType);
                    if (refType != null) CheckDefRef(li.Name.LocalName, refType, ctx);
                }
                else
                {
                    Errors.Add($"{ctx} : élément <{li.Name.LocalName}> inattendu dans une liste de {itemType.Name} (attendu <li>)");
                }
            }
        }

        private static Type FirstDefFieldType(Type t)
        {
            for (Type cur = t; cur != null; cur = SafeBase(cur))
            {
                FieldInfo[] fields;
                try { fields = cur.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly); }
                catch { continue; }
                foreach (FieldInfo f in fields)
                {
                    if (IsDef(f.FieldType)) return f.FieldType;
                }
            }
            return null;
        }

        private static void CheckEnum(string value, Type enumType, string ctx)
        {
            string[] names;
            try { names = enumType.GetFields(BindingFlags.Public | BindingFlags.Static).Select(f => f.Name).ToArray(); }
            catch { return; }
            foreach (string part in value.Split(new[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string p = part.Trim();
                if (p.Length == 0 || char.IsDigit(p[0]) || p[0] == '-') continue;
                if (!names.Contains(p)) Errors.Add($"{ctx} : \"{p}\" n'est pas une valeur de {enumType.Name}");
            }
        }

        private static void CheckDefRef(string defName, Type wanted, string ctx)
        {
            if (string.IsNullOrEmpty(defName)) return;
            if (KnownDefs.TryGetValue(defName, out HashSet<string> types))
            {
                foreach (string tn in types)
                {
                    Type known = ResolveType(tn);
                    if (known == null || IsSubclassOrSame(known, wanted) || IsSubclassOrSame(wanted, known)) return;
                }
                // Même defName sous un autre type : légal dans RimWorld (espaces de noms par type),
                // on continue donc la recherche d'indices pour le type voulu.
            }
            if (Baseline.Contains(wanted.Name + ":" + defName))
            {
                return;
            }
            if (DependencyTokens.Contains(defName))
            {
                WeakRefs.Add($"{defName} ({wanted.Name})");
                return;
            }
            Warnings.Add($"{ctx} : {wanted.Name} \"{defName}\" introuvable (ni dans le mod, ni les dépendances, ni un [DefOf] du jeu)");
            Unverified.Add(wanted.Name + ":" + defName);
        }
    }
}
