using Verse;

namespace Stargate
{
    /// <summary>
    /// Paramètres de ponte d'un gène de reine (Gene_GoauldBrooder), définis en XML.
    /// Permet à une reine goa'uld et à une reine tok'ra de partager la même classe
    /// tout en pondant des produits différents à des cadences différentes.
    /// </summary>
    public class GeneExtension_Brooder : DefModExtension
    {
        // Intervalle entre deux pontes, en ticks (60000 ticks = 1 jour de jeu).
        public int productionIntervalTicks = 300000; // ~5 jours par défaut

        // Ce qui est pondu (larve goa'uld immature, ou symbiote tok'ra déjà mature).
        public ThingDef product;

        // Nombre max d'exemplaires de ce produit tolérés sur la carte avant pause.
        public int maxOnMap = 6;
    }
}
