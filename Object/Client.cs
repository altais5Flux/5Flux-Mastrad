using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Objets100cLib;
using WebservicesSage.Object;
using WebservicesSage.Cotnroller;

namespace WebservicesSage.Object
{
    [Serializable()]
    public class Client
    {
        public string Intitule { get; set; }
        public string Contact { get; set; }
        public bool Sommeil { get; set; }
        public string Email { get; set; }
        public string GroupeTarifaireIntitule { get; set; }
        public string CT_NUM { get; set; }
        public List<ClientLivraisonAdress> clientLivraisonAdresses { get; set; }
        private IBOClient3 clientFC;

        public Client(IBOClient3 clientFC)
        {
            clientLivraisonAdresses = new List<ClientLivraisonAdress>();
            this.clientFC = clientFC;
            Email = clientFC.Telecom.EMail;
            Intitule = clientFC.CT_Intitule;
            Sommeil = clientFC.CT_Sommeil;
            Contact = clientFC.CT_Contact;
            GroupeTarifaireIntitule = clientFC.CatTarif.CT_Intitule;
            CT_NUM = clientFC.CT_Num;
            
        }
        public Client()
        {

        }
        public void setClientLivraisonAdresse()
        {
            clientLivraisonAdresses = ControllerClientLivraisonAdress.getAllClientLivraisonAdressToProcess(clientFC.FactoryClientLivraison.List);
        }
    }
}
