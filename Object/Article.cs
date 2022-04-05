using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Objets100cLib;
using WebservicesSage.Object;
using WebservicesSage.Singleton;
using WebservicesSage.Utils;

namespace WebservicesSage.Object
{
    [Serializable()]
    public class Article
    {
        public string Designation { get; set; }
        public string Reference { get; set; }
        public double PrixAchat { get; set; }
        public double PrixVente { get; set; }
        public string Langue1 { get; set; }
        public string Langue2 { get; set; }
        public string CodeBarres { get; set; }
        public double Poid { get; set; }
        public bool Sommeil { get; set; }
        public bool IsPriceTTC { get; set; }
        public bool isGamme { get; set; }
        public List<Gamme> Gammes { get; set; }
        public List<PrixGamme> prixGammes { get; set; }
        public List<PrixRemise> prixRemises { get; set; }
        public List<PrixCatTarif> prixCatTarifs { get; set; }
        public List<PrixRemiseClient> prixRemisesClient { get; set; }
        public List<PrixClientTarif> prixClientTarif { get; set; }
        public List<RemiseFamille> remiseFamilles { get; set; }
        public List<InfoLibre> infoLibre { get; set; }
        public bool IsDoubleGamme { get; set; }
        public double Stock { get; set; }
        public bool HaveNomenclature { get; set; }
        public ArticleNomenclature ArticleNomenclature { get; set; }
        public List<Conditionnement> conditionnements { get; set; }
        //editeur article
        public string Resume { get; set; }
        public string LinkRewrite { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }
        public List<Category> Categories { get; set; }
        public string Ecotaxe { get; set; }
        public string Fournisseur { get; set; }
        public string Longueur {get;set;}
        public string Largeur {get;set;}
        public string Taille {get;set;}
//editeur article
        public string Dimension { get; set; }
        public string Couleur { get; set; }
        public string PaysOrigin { get; set; }
public Article(IBOArticle3 articleFC)
        {
            var compta = SingletonConnection.Instance.Gescom.CptaApplication;
            conditionnements = new List<Conditionnement>();
            infoLibre = new List<InfoLibre>();
            prixRemises = new List<PrixRemise>();
            prixRemisesClient = new List<PrixRemiseClient>();
            prixCatTarifs = new List<PrixCatTarif>();
            prixClientTarif = new List<PrixClientTarif>();
            remiseFamilles = new List<RemiseFamille>();
            PaysOrigin = articleFC.AR_Pays;
            Couleur = "";
            Dimension = "";
            var gescom = SingletonConnection.Instance.Gescom;
            var infolibreField = gescom.FactoryArticle.InfoLibreFields;
            int compteur = 1;
            foreach (var infoLibreValue in articleFC.InfoLibre)
            {
                if (infolibreField[compteur].Name.Equals(UtilsConfig.DimensionName))
                {
                    Dimension = infoLibreValue.ToString();
                }
                infoLibre.Add(new InfoLibre(infolibreField[compteur].Name, infoLibreValue.ToString()));
                compteur++;
            }
            Couleur =  articleFC.AR_Stat[UtilsConfig.CouleurIndex].ToString();
            Designation = articleFC.AR_Design;
            Reference = articleFC.AR_Ref;
            PrixAchat = articleFC.AR_PrixAchat;

            PrixVente = articleFC.AR_PrixVen;
            Langue1 = articleFC.AR_Langue1;
            Langue2 = articleFC.AR_Langue2;
            CodeBarres = articleFC.AR_CodeBarre;
            if (UtilsConfig.DefaultStock.Equals("TRUE"))
            {
                Stock = articleFC.StockReel();
            }
            else
            {
                Stock = articleFC.StockATerme();
            }

            // gestion de la conversion du poids en KG pour prestashop
            switch (articleFC.AR_UnitePoids)
            {
                case UnitePoidsType.UnitePoidsTypeTonne:
                    Poid = articleFC.AR_PoidsNet * 1000;
                    break;
                case UnitePoidsType.UnitePoidsTypeQuintal:
                    Poid = articleFC.AR_PoidsNet * 100;
                    break;
                case UnitePoidsType.UnitePoidsTypeKilogramme:
                    Poid = articleFC.AR_PoidsNet;
                    break;
                case UnitePoidsType.UnitePoidsTypeGramme:
                    Poid = articleFC.AR_PoidsNet / 1000;
                    break;
                case UnitePoidsType.UnitePoidsTypeMilligramme:
                    Poid = articleFC.AR_PoidsNet / 1000000;
                    break;
                default:
                    break;
            }

            Sommeil = articleFC.AR_Sommeil;
            IsPriceTTC = articleFC.AR_PrixTTC;
        }

        public Article()
        {

        }
    }
}
