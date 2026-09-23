import json, os, sys, concurrent.futures as cf
from gen import gen
ICON=("flat white silhouette icon, bold simple shapes, centered, pure black background, no gradient, no text, no border. Subject: ")
A=[]  # (id, prompt, mode, [(outpath,size)])
def obj(i,p,out,size): A.append((i,p,'obj',out,size))
def icon(i,p,out,size): A.append((i,ICON+p,'icon',out,size))
T='Textures/'
obj('naq_a',"a single small ingot of dense dark metallic alien ore, gunmetal grey with teal glowing cracks, naquadah",T+'Things/Item/Resource/SG_Naquadah/SG_Naquadah_a',64)
obj('naq_b',"three small ingots of dense dark metallic alien ore stacked, gunmetal grey with teal glowing cracks, naquadah",T+'Things/Item/Resource/SG_Naquadah/SG_Naquadah_b',64)
obj('naq_c',"a pile of six ingots of dense dark metallic alien ore, gunmetal grey with teal glowing cracks, naquadah",T+'Things/Item/Resource/SG_Naquadah/SG_Naquadah_c',64)
obj('larva',"a small pale translucent eel-like alien larva with tiny fins and a yellow sheen, curled, slimy, Stargate Goa'uld larva",T+'Things/Item/SG_GoauldLarva',64)
obj('symb',"an adult alien serpent symbiote, grey-silver scaled eel with a flared cobra-like head and small fins, glowing eyes, curled in an S shape",T+'Things/Item/SG_GoauldSymbiote',64)
obj('tokra',"an adult alien serpent symbiote with pale blue-grey scales and a calm blue glow, flared cobra-like head, curled in an S shape",T+'Things/Item/SG_TokraSymbiote',64)
obj('tret',"a small sealed glass vial of amber liquid with a golden cap and faint alien hieroglyph engravings",T+'Things/Item/SG_Tretonin',64)
obj('staff',"a long ornate bronze and gold staff weapon lying diagonally from bottom-left to top-right, bulbous emitter head with petals at the top, Egyptian engravings, Stargate Jaffa staff weapon",T+'Things/Item/Equipment/SG_StaffWeapon',128)
obj('zat',"an alien handgun that looks like a curled bronze snake: S-shaped segmented body forming the grip, the snake head folded forward as the muzzle, no gun barrel, no trigger guard, glowing blue eyes, side view, pure white background",T+'Things/Item/Equipment/SG_Zat',64)
obj('kara',"a golden ornate bracelet-and-palm-plate jewelry device lying flat on its own, with finger rings attached by golden chains and a big red-orange jewel on the palm plate, Egyptian style, absolutely no hand, no fingers, no skin, empty object only",T+'Things/Item/Equipment/SG_KaraKesh',64)
obj('armor',"heavy ceremonial armor chest piece of chainmail and bronze plates with broad shoulder guards, dark leather, Stargate Jaffa armor, laid flat",T+'Things/Apparel/SG_JaffaArmor/SG_JaffaArmor',64)
obj('helm',"a bronze helmet shaped like a cobra head with segmented hood and glowing red eyes, Stargate serpent guard helmet",T+'Things/Apparel/SG_JaffaHelmet/SG_JaffaHelmet',64)
obj('robe',"an opulent folded robe of red and gold silk with Egyptian collar and jeweled chest ornaments",T+'Things/Apparel/SG_GoauldRobe/SG_GoauldRobe',64)
obj('sgc',"a folded olive-green military combat jacket with tactical vest and a round patch showing a ring with chevrons",T+'Things/Apparel/SG_SGCUniform/SG_SGCUniform',64)
obj('gate',"perfectly flat orthographic front view of the Stargate: a thick circular ring of dark grey stone-metal with carved glyphs, nine orange triangular chevrons evenly spaced on the outer edge, the inside of the ring is empty white background, no ground, no base, no perspective, no shadow",T+'Things/Building/SG_Stargate',320)
obj('dhd',"orthographic view from directly above of a round grey stone console with two concentric rings of small glyph buttons around a large red glowing dome in the center, perfectly circular, no shadow, no perspective",T+'Things/Building/SG_DHD',96)
obj('rings',"orthographic view from directly above of a perfectly circular flat golden floor disc with concentric engraved rings and Egyptian patterns, seen straight down like a map, perfectly round, no thickness visible, no perspective, no shadow",T+'Things/Building/SG_TransportRings',192)
obj('gen',"a compact alien reactor, a squat metal cylinder with a glowing green-blue core visible through slots, cables at the base",T+'Things/Building/SG_NaquadahGenerator',128)
obj('scbase',"orthographic view from directly above of a square flat turret platform, sandstone with gold trim and Egyptian patterns, a dark circular socket in the middle, seen straight down like a floor plan, no sides visible, no perspective, no shadow",T+'Things/Building/SG_StaffCannon_Base',192)
obj('sctop',"orthographic view from directly above of a golden cannon lying horizontally, oriented vertically in the image with the muzzle at the top edge and a round swivel base at the bottom, long straight barrel ending in a bulbous plasma emitter, Egyptian ornaments, seen straight down, no perspective, no shadow",T+'Things/Building/SG_StaffCannon_Top',256)
obj('ionbase',"strict top-down view of a large sleek pale grey-blue hexagonal turret mount, clean alien design, soft blue lights",T+'Things/Building/SG_IonCannon_Base',320)
obj('iontop',"strict top-down view of a sleek elongated ion cannon pointing straight up toward the top of the image, glowing blue emitter at the tip, smooth white-grey hull plates, vertical",T+'Things/Building/SG_IonCannon_Top',384)
obj('shield',"strict top-down view of a large golden pyramid-shaped shield generator with a glowing core and four emitter spikes at the corners, Goa'uld technology",T+'Things/Building/SG_GoauldShield',384)
icon('f_tauri',"the Stargate: a ring with nine chevrons around it",T+'UI/Factions/SG_Tauri',128)
icon('f_goauld',"a pyramid topped by a sun disk flanked by a cobra",T+'UI/Factions/SG_Goauld',128)
icon('f_tokra',"two interlocked elegant arcs forming a symbol",T+'UI/Factions/SG_Tokra',128)
icon('f_jaffa',"an upright staff spear in front of a broken ring",T+'UI/Factions/SG_FreeJaffa',128)
icon('f_asgard',"a stylized Thor hammer",T+'UI/Factions/SG_Asgard',128)
icon('g_pouch',"a human abdomen with an X-shaped pouch",T+'UI/Icons/Genes/SG_JaffaPouch',64)
icon('g_sarc',"an Egyptian sarcophagus",T+'UI/Icons/Genes/SG_SarcophagusRegen',64)
icon('g_queen',"a serpent coiled around eggs",T+'UI/Icons/Genes/SG_QueenBrooder',64)
icon('g_ess',"an open hand with a glowing drop above the palm",T+'UI/Icons/Genes/SG_VitalEssence',64)
icon('g_drain',"a clawed hand with an eye-like organ in the palm",T+'UI/Icons/Genes/SG_LifeDrain',64)
for k,d in [('Tauri','a human soldier head in profile with a military cap'),('Jaffa','a bald warrior head in profile with a golden emblem on the forehead'),
            ('Goauld','a regal head in profile with glowing eyes and an Egyptian headdress'),('GoauldQueen','a regal female head in profile with a tall Egyptian crown'),
            ('Tokra','a calm human head in profile with a subtle serpent behind the neck'),('Asgard','a small grey alien head in profile with huge black eyes'),
            ('Unas','a reptilian brute head in profile with horns and heavy brow'),('Wraith','a pale alien head in profile with long hair and slit nose')]:
    icon('x_'+k,d,T+'UI/Icons/Xenotypes/SG_'+k,64)
if __name__=="__main__":
    only=set(sys.argv[1:])
    todo=[a for a in A if not only or a[0] in only]
    def run(a):
        i,p,mode,out,size=a
        raw=os.path.join(os.path.dirname(os.path.abspath(__file__)),'raw',f'{i}.png')
        if os.path.exists(raw): return i,True
        for _ in range(2):
            try:
                if gen(p,raw,style=(mode=='obj')): return i,True
            except Exception as e: print('err',i,e)
        return i,False
    with cf.ThreadPoolExecutor(1) as ex:
        res=list(ex.map(run,todo))
    print('échecs:',[i for i,ok in res if not ok])
    json.dump([list(a) for a in A],open(os.path.join(os.path.dirname(os.path.abspath(__file__)),'raw','spec.json'),'w'))
