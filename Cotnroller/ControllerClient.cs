using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Objets100cLib;
using WebservicesSage.Object;
using WebservicesSage.Singleton;
using WebservicesSage.Utils;
using WebservicesSage.Utils.Enums;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Xml;
using WebservicesSage.Object.CustomerSearch;
using WebservicesSage.Object.Order;
using System.Globalization;
using System.Timers;
using WebservicesSage.Object.CustomerSearchByEmail;
using System.Data.SqlClient;

namespace WebservicesSage.Cotnroller
{
    public static class ControllerClient
    {

        
        /// <summary>
        /// Permets de remonter toute la base clients de SAGE vers Prestashop
        /// Ne remonte que les clients avec un mail 
        /// </summary>
        public static void SendAllClients()
        {
            try
            {
                var compta = SingletonConnection.Instance.Gescom.CptaApplication;
                var clientsSageObj = compta.FactoryClient.List;

                //var clients = GetListOfClientToProcess(clientsSageObj);
                foreach (IBOClient3 clientSageObj in clientsSageObj)
                {
                    var clients = GetListOfClientToProcess(clientSageObj);

                    //int increm = 100 / clients.Count;

                    foreach (Client client in clients)
                    {

                        /*string clientXML = UtilsSerialize.SerializeObject<Client>(client);
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(clientXML);*/
                        /*Customer customerMagento = new Customer();
                        var jsonClient = JsonConvert.SerializeObject(customerMagento.NewCustomer(client, clientSageObj));
                        UtilsWebservices.SendDataJson(jsonClient, @"rest/all/V1/customers/");
                        //UtilsWebservices.SendData(UtilsConfig.BaseUrl + EnumEndPoint.Client.Value, clientXML);
                        */

                        CustomerSearchByEmail ClientSearch = CustomerSearchByEmail.FromJson(UtilsWebservices.GetMagentoData("rest/V1/customers/search" + UtilsWebservices.SearchOrderCriteria("email", client.Email, "eq")));
                        Customer customerMagento = new Customer();
                        CustomerSearch clientM = UtilsWebservices.GetClientCtNum(ClientSearch.Items[0].Id.ToString());
                        if (!String.IsNullOrEmpty(ClientSearch.Items[0].Email))
                        {
                            var jsonClient = JsonConvert.SerializeObject(customerMagento.UpdateCustomer(client.CT_NUM, "", clientM));
                            UtilsWebservices.SendDataJson(jsonClient, @"rest/all/V1/customers/" + ClientSearch.Items[0].Id.ToString(), "PUT");
                        }
                        else
                        {
                            var jsonClient = JsonConvert.SerializeObject(customerMagento.NewCustomer(client, clientSageObj, ClientSearch));
                            UtilsWebservices.SendDataJson(jsonClient, @"rest/all/V1/customers/");
                        }
                    }
                }
                

                MessageBox.Show("end client sync", "ok",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Information);
            }

        }

        public static void SendClient(string ct_num)
        {
            try
            {
                var compta = SingletonConnection.Instance.Gescom.CptaApplication;
                var clientsSageObj = compta.FactoryClient.ReadNumero(ct_num);
                var clients = GetListOfClientToProcess(clientsSageObj);

                int increm = 100 / clients.Count;

                foreach (Client client in clients)
                {

                    /*string clientXML = UtilsSerialize.SerializeObject<Client>(client);
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(clientXML);*/
                    CustomerSearchByEmail ClientSearch = CustomerSearchByEmail.FromJson(UtilsWebservices.GetMagentoData("rest/V1/customers/search" + UtilsWebservices.SearchOrderCriteria("email", client.Email, "eq")));
                    Customer customerMagento = new Customer();
                    CustomerSearch clientM = UtilsWebservices.GetClientCtNum(ClientSearch.Items[0].Id.ToString());
                    if (!String.IsNullOrEmpty(ClientSearch.Items[0].Email))
                    {
                        var jsonClient = JsonConvert.SerializeObject(customerMagento.UpdateCustomer(ct_num, "", clientM));
                        UtilsWebservices.SendDataJson(jsonClient, @"rest/all/V1/customers/"+ClientSearch.Items[0].Id.ToString(),"PUT");
                    }
                    else
                    {
                        var jsonClient = JsonConvert.SerializeObject(customerMagento.NewCustomer(client, clientsSageObj, ClientSearch));
                        UtilsWebservices.SendDataJson(jsonClient, @"rest/all/V1/customers/");
                    }
                    
                    //UtilsWebservices.SendData(UtilsConfig.BaseUrl + EnumEndPoint.Client.Value, clientXML);
                }

                MessageBox.Show("end client sync", "ok",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Permet de vérifier si un client comporte des erreur ou non
        /// </summary>
        /// <param name="client">Client à tester</param>
        /// <returns></returns>
        private static bool HandleClientError(Client client)
        {
            bool error = false;

            if (String.IsNullOrEmpty(client.Email))
            {
                //error = true;
                // SingletonUI.Instance.LogBox.Invoke((MethodInvoker)(() => SingletonUI.Instance.LogBox.AppendText("Client :  " + client.Intitule + " No Mail Found" + Environment.NewLine)));


                // on affiche une erreur + log 
            }

            return error;
        }

        /// <summary>
        /// Permet de récupérer une liste de Client depuis une liste de Client SAGE
        /// </summary>
        /// <param name="clientsSageObj">List de client SAGE</param>
        /// <returns></returns>
        private static List<Client> GetListOfClientToProcess(IBOClient3 clientSageObj)
        {
            List<Client> clientToProcess = new List<Client>();
                    if (!String.IsNullOrEmpty(clientSageObj.Telecom.EMail))
                    {
                        Client client = new Client(clientSageObj);
                        client.setClientLivraisonAdresse();
                        clientToProcess.Add(client);
                    }
                    else
                    {
                    // On ajoute les contacts à la liste
                    foreach (IBOTiersContact3 contact in clientSageObj.FactoryTiersContact.List)
                    {
                        if (!string.IsNullOrEmpty(contact.Telecom.EMail))
                        {
                            Client clientContact = new Client();
                            clientContact.CT_NUM = clientSageObj.CT_Num;
                            clientContact.Sommeil = clientSageObj.CT_Sommeil;
                            clientContact.GroupeTarifaireIntitule = clientSageObj.CatTarif.CT_Intitule;
                            clientContact.Email = contact.Telecom.EMail;
                            clientContact.Contact = "";
                            if (!String.IsNullOrEmpty(contact.Prenom))
                            {
                                clientContact.Contact = contact.Prenom;
                            }
                            if (!String.IsNullOrEmpty(contact.Nom))
                            {
                                clientContact.Contact = clientContact.Contact + " " + contact.Nom;
                            }
                            //clientContact.clientLivraisonAdresses = client.clientLivraisonAdresses;
                            clientToProcess.Add(clientContact);
                        }
                    }
                    }
            return clientToProcess;
        }

        private static List<Client> GetClientToProcess(IBOClient3 clientsSageObj)
        {
            List<Client> clientToProcess = new List<Client>();

            Client client = new Client(clientsSageObj);

            if (!HandleClientError(client))
            {
                client.setClientLivraisonAdresse();
                clientToProcess.Add(client);
            }
            return clientToProcess;
        }

        /// <summary>
        /// Permet de vérifier si un Client existe dans SAGE
        /// </summary>
        /// <param name="CT_num"></param>
        /// <returns></returns>
        public static bool CheckIfClientExist(string CT_num)
        {
            if (String.IsNullOrEmpty(CT_num))
            {
                return false;
            }
            else
            {
                var compta = SingletonConnection.Instance.Gescom.CptaApplication;
                if (compta.FactoryTiers.ExistNumero(CT_num))// FactoryClient.ExistNumero(CT_num))
                {
                    //File.AppendAllText("Log\\GetOrderCustomer.txt", "Client existe : " + CT_num + Environment.NewLine);
                    return true;

                }
                else
                {
                    //File.AppendAllText("Log\\GetOrderCustomer.txt", "Client n'existe pas : " + CT_num + Environment.NewLine);
                    return false;
                }

            }

        }

        /// <summary>
        /// ToDo
        /// </summary>
        public static void CreateNewClient()
        {

        }

        /// <summary>
        /// Permet de crée un Client dans la base SAGE depuis un objet json de prestashop
        /// </summary>
        /// <param name="jsonClient">json du Client à crée</param>
        /// <returns></returns>
        public static string CreateNewClient(CustomerSearch customer, OrderSearchItem order)
        {
            //JObject customer = JObject.Parse(jsonClient);

            var compta = SingletonConnection.Instance.Gescom.CptaApplication;
            var gescom = SingletonConnection.Instance.Gescom;
            IBOClient3 clientSage = (IBOClient3)compta.FactoryClient.Create();
            clientSage.SetDefault();
            //File.AppendAllText("Log\\GetCustomer.txt", tarifsSearch.ToString() + Environment.NewLine);

            /*
            var test = clientSage.FactoryTiersContact.Create();
            test.Factory.List[0].*/
            try
            {
                Object.CustomerSearch.Address defaultAddress = new Object.CustomerSearch.Address();
                foreach (Object.CustomerSearch.Address addressCustomer in customer.Addresses)
                {
                    if (addressCustomer.Id == customer.DefaultBilling)
                    {
                        defaultAddress = addressCustomer;
                        break;
                    }
                }
                if (String.IsNullOrEmpty(UtilsConfig.PrefixClient))
                {
                    // pas de configuration renseigner pour le prefix client
                    // todo log
                    //int iterID = Int32.Parse(UtilsWebservices.SendDataNoParse(UtilsConfig.BaseUrl + EnumEndPoint.Client.Value, "getClientIterationSage&clientID=" + customer["id"].ToString()));
                    int iterID = Int32.Parse(customer.Id.ToString());
                    while (compta.FactoryClient.ExistNumero(iterID.ToString()))
                    {
                        iterID++;
                    }
                    clientSage.CT_Num = iterID.ToString();
                }
                else
                {
                    clientSage.CT_Num = UtilsConfig.PrefixClient + customer.Id.ToString();
                }
                string intitule;//TODO TAKE CARE OF NULL VALUE
                if (!String.IsNullOrEmpty(defaultAddress.company))
                {
                    intitule = defaultAddress.company.ToUpper();
                }
                else
                {
                    intitule = defaultAddress.Firstname.ToString().ToUpper() + " " + defaultAddress.Lastname.ToString().ToUpper();
                }
                if (intitule.Length > 35)
                {
                    clientSage.CT_Intitule = intitule.Substring(0, 35);
                }
                else
                {
                    clientSage.CT_Intitule = intitule;
                }

                IBOCompteG3 compteG = compta.FactoryCompteG.ReadNumero("41110000");
                clientSage.CompteGPrinc = compteG;
                if (defaultAddress.Street.Count > 0)
                {
                    if (defaultAddress.Street[0].Length > 35)
                    {
                        clientSage.Adresse.Adresse = defaultAddress.Street[0].ToString().Substring(0, 35);
                    }
                    else
                    {
                        clientSage.Adresse.Adresse = defaultAddress.Street[0].ToString();
                    }
                }
                if (defaultAddress.Street.Count > 1)
                {
                    if (defaultAddress.Street[1].Length > 35)
                    {
                        clientSage.Adresse.Complement = defaultAddress.Street[1].ToString().Substring(0, 35);
                    }
                    else
                    {
                        clientSage.Adresse.Complement = defaultAddress.Street[1].ToString();
                    }

                }

                clientSage.Adresse.CodePostal = defaultAddress.Postcode.ToString();
                clientSage.Adresse.Ville = defaultAddress.City.ToString();
                var region = CultureInfo
                                .GetCultures(CultureTypes.SpecificCultures)
                                .Select(ci => new RegionInfo(ci.LCID))
                                .FirstOrDefault(rg => rg.TwoLetterISORegionName == defaultAddress.CountryId);
                clientSage.Adresse.Pays = region.DisplayName.ToString();
                clientSage.Telecom.Telephone = defaultAddress.Telephone.ToString();
                //clientSage.Telecom.Telecopie = customer.Addresses[0]..ToString();




                clientSage.Write();
                foreach (Object.CustomerSearch.CustomAttribute customarttribute in customer.CustomAttributes)
                {
                    if (customarttribute.AttributeCode.Equals("customer_type"))
                    {
                        if (customarttribute.Value.ToString().Equals("1"))
                        {
                            clientSage.InfoLibre[3] = "PRIMAIRE/MATERNELLE";
                        }
                        if (customarttribute.Value.ToString().Equals("2"))
                        {
                            clientSage.InfoLibre[3] = "COLLEGE";
                        }
                        if (customarttribute.Value.ToString().Equals("3"))
                        {
                            clientSage.InfoLibre[3] = "LYCEE";
                        }
                        if (customarttribute.Value.ToString().Equals("4"))
                        {
                            clientSage.InfoLibre[3] = "POST-BAC";
                        }
                        if (customarttribute.Value.ToString().Equals("5"))
                        {
                            clientSage.InfoLibre[3] = "GROUPE SCOLAIRE";
                        }
                        if (customarttribute.Value.ToString().Equals("6"))
                        {
                            clientSage.InfoLibre[3] = "INSTITUT SPECIALISE";
                        }
                        if (customarttribute.Value.ToString().Equals("7"))
                        {
                            clientSage.InfoLibre[3] = "CENTRE DE FORMATION";
                        }
                        if (customarttribute.Value.ToString().Equals("8"))
                        {
                            clientSage.InfoLibre[3] = "SOCIETE";
                        }
                        if (customarttribute.Value.ToString().Equals("9"))
                        {
                            clientSage.InfoLibre[3] = "PARTICULIER";
                        }
                        if (customarttribute.Value.ToString().Equals("10"))
                        {
                            clientSage.InfoLibre[3] = "ASSOCIATION";
                        }
                        if (customarttribute.Value.ToString().Equals("11"))
                        {
                            clientSage.InfoLibre[3] = "ADMINISTRATION";
                        }
                        if (customarttribute.Value.ToString().Equals("12"))
                        {
                            clientSage.InfoLibre[3] = "CENTRALE D\'ACHATS";
                        }

                    }
                }

                /*
                if (String.IsNullOrEmpty(UtilsConfig.CatTarif))
                {
                    // pas de configuration renseigner pour la cat tarif par defaut
                    // todo log
                }
                else
                {
                    clientSage.CatTarif = gescom.FactoryCategorieTarif.ReadIntitule(UtilsConfig.CatTarif);
                }
                if (String.IsNullOrEmpty(UtilsConfig.CompteG))
                {
                    // pas de configuration renseigner pour la cat tarif par defaut
                    // todo log
                }
                else
                {
                    clientSage.CompteGPrinc = compta.FactoryCompteG.ReadNumero(UtilsConfig.CompteGnum);
                }
                */
                string contactS = defaultAddress.Firstname.ToString() + " " + defaultAddress.Lastname.ToString();
                if (contactS.Length > 35)
                {
                    clientSage.CT_Contact = contactS.Substring(0, 35);
                }
                else
                {
                    clientSage.CT_Contact = contactS;
                }

                clientSage.Telecom.EMail = customer.Email.ToString();


                // abrégé client 
                if (intitule.Length > 17)
                {
                    clientSage.CT_Classement = intitule.Substring(0, 17);
                }
                else
                {
                    clientSage.CT_Classement = intitule;
                }
                if (!String.IsNullOrEmpty(customer.taxvat))
                {
                    clientSage.CT_Identifiant = customer.taxvat;
                }
                else
                {
                    clientSage.CT_Identifiant = "";
                }


                if (region.DisplayName.ToString().ToUpper() != "FRANCE" && !clientSage.CT_Identifiant.ToString().Equals(""))
                {
                    try
                    {
                        IBICategorieCompta categorieCompta = gescom.FactoryCategorieComptaVente.ReadIntitule(UtilsConfig.CategorieComptableForeigner);
                        clientSage.CategorieCompta = categorieCompta;
                    }
                    catch (Exception e)
                    {
                        UtilsMail.SendErrorMail(DateTime.Now + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine, "CREATE CLIENT CATEGORIE COMPTABLE");
                    }
                }
                clientSage.Write();
            }
            catch (Exception e)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(DateTime.Now + "Erreur création client : " + Environment.NewLine);
                sb.Append(DateTime.Now + e.Message + Environment.NewLine);
                sb.Append(DateTime.Now + e.StackTrace + Environment.NewLine);
                File.AppendAllText("Log\\CreateClient.txt", sb.ToString());
            }

            clientSage.Write();

            try
            {
                IBOClientLivraison3 addrprinc = (IBOClientLivraison3)clientSage.FactoryClientLivraison.Create();

                if (!String.IsNullOrEmpty(order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.company))
                {
                    if (order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.company.ToString().Length > 35)
                    {
                        addrprinc.LI_Intitule = order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.company.ToUpper().Substring(0, 35);
                    }
                    else
                    {
                        addrprinc.LI_Intitule = order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.company.ToUpper();
                    }
                }
                else
                {
                    string intitule = "";
                    intitule = order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Firstname + " " + order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Lastname;
                    if (intitule.Length > 35)
                    {
                        intitule = intitule.Substring(0, 35);
                    }
                    addrprinc.LI_Intitule = intitule.ToUpper();
                }

                if (order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Street[0].ToString().Length > 35)
                {
                    addrprinc.Adresse.Adresse = order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Street[0].ToString().Substring(0, 35);
                }
                else
                {
                    addrprinc.Adresse.Adresse = order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Street[0].ToString();
                }
                if (order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Street.Count > 1)
                {
                    addrprinc.Adresse.Complement = order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Street[1].ToString();
                }
                addrprinc.Adresse.CodePostal = order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Postcode;
                addrprinc.Adresse.Ville = order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.City;
                var region = CultureInfo
                                    .GetCultures(CultureTypes.SpecificCultures)
                                    .Select(ci => new RegionInfo(ci.LCID))
                                    .FirstOrDefault(rg => rg.TwoLetterISORegionName == order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.CountryId);
                addrprinc.Adresse.Pays = region.DisplayName.ToString();
                addrprinc.Telecom.Telephone = order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Telephone.ToString();
                //addrprinc.Telecom.Telecopie = order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Telecopie.ToString();

                if (String.IsNullOrEmpty(UtilsConfig.CondLivraison))
                {
                    // pas de configuration renseigner pour CondLivraison par defaut
                    // todo log
                }
                else
                {
                    addrprinc.ConditionLivraison = gescom.FactoryConditionLivraison.ReadIntitule(UtilsConfig.CondLivraison);
                }
                if (String.IsNullOrEmpty(UtilsConfig.Expedition))
                {
                    // pas de configuration renseigner pour Expedition par defaut
                    // todo log
                }
                else
                {
                    addrprinc.Expedition = gescom.FactoryExpedition.ReadIntitule(UtilsConfig.Expedition);
                }
                clientSage.LivraisonPrincipal = addrprinc;
                addrprinc.Write();
            }
            catch (Exception e)
            {
                UtilsMail.SendErrorMail(DateTime.Now + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine, "CREATE CLIENT ADRESS P");
                StringBuilder sb = new StringBuilder();
                sb.Append(DateTime.Now + e.Message + Environment.NewLine);
                sb.Append(DateTime.Now + e.StackTrace + Environment.NewLine);
                File.AppendAllText("Log\\CreateClient.txt", sb.ToString());
                sb.Clear();
                return "";
            }



            return clientSage.CT_Num;
        }

        public static string CreateNewClientDevis(CustomerSearch customer, Object.Devis.Devis order)
        {
            //JObject customer = JObject.Parse(jsonClient);

            var compta = SingletonConnection.Instance.Gescom.CptaApplication;
            var gescom = SingletonConnection.Instance.Gescom;
            IBOClient3 clientSage = (IBOClient3)compta.FactoryClient.Create();
            clientSage.SetDefault();/*
            var test = clientSage.FactoryTiersContact.Create();
            test.Factory.List[0].*/
            try
            {
                if (customer.Addresses.Count >0)
                {
                    Object.CustomerSearch.Address defaultAddress = new Object.CustomerSearch.Address();
                    foreach (Object.CustomerSearch.Address addressCustomer in customer.Addresses)
                    {
                        if (addressCustomer.Id == customer.DefaultBilling)
                        {
                            defaultAddress = addressCustomer;
                            break;
                        }
                    }
                    if (defaultAddress.Street[0].Length > 35)
                    {
                        clientSage.Adresse.Adresse = defaultAddress.Street[0].ToString().Substring(0, 35);
                    }
                    else
                    {
                        clientSage.Adresse.Adresse = defaultAddress.Street[0].ToString();
                    }
                    if (defaultAddress.Street.Count > 1)
                    {
                        clientSage.Adresse.Complement = defaultAddress.Street[1].ToString();
                    }

                    clientSage.Adresse.CodePostal = defaultAddress.Postcode.ToString();
                    clientSage.Adresse.Ville = defaultAddress.City.ToString();
                    var region = CultureInfo
                                    .GetCultures(CultureTypes.SpecificCultures)
                                    .Select(ci => new RegionInfo(ci.LCID))
                                    .FirstOrDefault(rg => rg.TwoLetterISORegionName == defaultAddress.CountryId);
                    clientSage.Adresse.Pays = region.DisplayName.ToString();
                    clientSage.Telecom.Telephone = defaultAddress.Telephone.ToString();
                    string intitule;//TODO TAKE CARE OF NULL VALUE
                    if (!String.IsNullOrEmpty(defaultAddress.company))
                    {
                        intitule = defaultAddress.company.ToUpper();
                    }
                    else
                    {
                        intitule = defaultAddress.Firstname.ToString().ToUpper() + " " + defaultAddress.Lastname.ToString().ToUpper();
                    }
                    string contactS = defaultAddress.Firstname.ToString() + " " + defaultAddress.Lastname.ToString();
                    if (contactS.Length > 35)
                    {
                        clientSage.CT_Contact = contactS.Substring(0, 35);
                    }
                    else
                    {
                        clientSage.CT_Contact = contactS;
                    }

                    
                    if (intitule.Length > 35)
                    {
                        clientSage.CT_Intitule = intitule.Substring(0, 35);
                    }
                    else
                    {
                        clientSage.CT_Intitule = intitule;
                    }

                    // abrégé client 
                    if (intitule.Length > 17)
                    {
                        clientSage.CT_Classement = intitule.Substring(0, 17);
                    }
                    else
                    {
                        clientSage.CT_Classement = intitule;
                    }
                    /*
                    if (region.DisplayName.ToString().ToUpper() != "FRANCE" && !clientSage.CT_Identifiant.ToString().Equals(""))
                    {
                        try
                        {
                            IBICategorieCompta categorieCompta = gescom.FactoryCategorieComptaVente.ReadIntitule(UtilsConfig.CategorieComptableForeigner);
                            clientSage.CategorieCompta = categorieCompta;
                        }
                        catch (Exception e)
                        {
                            UtilsMail.SendErrorMail(DateTime.Now + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine, "CREATE CLIENT CATEGORIE COMPTABLE");
                        }
                    }*/
                }
                else
                {
                    string intitule;//TODO TAKE CARE OF NULL VALUE
                    intitule = customer.Firstname.ToString().ToUpper() + " " + customer.Lastname.ToString().ToUpper();
                    if (intitule.Length > 35)
                    {
                        clientSage.CT_Intitule = intitule.Substring(0, 35);
                    }
                    else
                    {
                        clientSage.CT_Intitule = intitule;
                    }
                }


                //clientSage.Telecom.Telecopie = customer.Addresses[0]..ToString();

                clientSage.Telecom.EMail = customer.Email.ToString();

                if (String.IsNullOrEmpty(UtilsConfig.PrefixClient))
                {
                    // pas de configuration renseigner pour le prefix client
                    // todo log
                    //int iterID = Int32.Parse(UtilsWebservices.SendDataNoParse(UtilsConfig.BaseUrl + EnumEndPoint.Client.Value, "getClientIterationSage&clientID=" + customer["id"].ToString()));
                    int iterID = Int32.Parse(customer.Id.ToString());
                    while (compta.FactoryClient.ExistNumero(iterID.ToString()))
                    {
                        iterID++;
                    }
                    clientSage.CT_Num = iterID.ToString();
                }
                else
                {
                    clientSage.CT_Num = UtilsConfig.PrefixClient + customer.Id.ToString();
                }
                clientSage.Write();
                foreach (Object.CustomerSearch.CustomAttribute customarttribute in customer.CustomAttributes)
                {
                    if (customarttribute.AttributeCode.Equals("customer_type"))
                    {
                        if (customarttribute.Value.ToString().Equals("1"))
                        {
                            clientSage.InfoLibre[3] = "PRIMAIRE/MATERNELLE";
                        }
                        if (customarttribute.Value.ToString().Equals("2"))
                        {
                            clientSage.InfoLibre[3] = "COLLEGE";
                        }
                        if (customarttribute.Value.ToString().Equals("3"))
                        {
                            clientSage.InfoLibre[3] = "LYCEE";
                        }
                        if (customarttribute.Value.ToString().Equals("4"))
                        {
                            clientSage.InfoLibre[3] = "POST-BAC";
                        }
                        if (customarttribute.Value.ToString().Equals("5"))
                        {
                            clientSage.InfoLibre[3] = "GROUPE SCOLAIRE";
                        }
                        if (customarttribute.Value.ToString().Equals("6"))
                        {
                            clientSage.InfoLibre[3] = "INSTITUT SPECIALISE";
                        }
                        if (customarttribute.Value.ToString().Equals("7"))
                        {
                            clientSage.InfoLibre[3] = "CENTRE DE FORMATION";
                        }
                        if (customarttribute.Value.ToString().Equals("8"))
                        {
                            clientSage.InfoLibre[3] = "SOCIETE";
                        }
                        if (customarttribute.Value.ToString().Equals("9"))
                        {
                            clientSage.InfoLibre[3] = "PARTICULIER";
                        }
                        if (customarttribute.Value.ToString().Equals("10"))
                        {
                            clientSage.InfoLibre[3] = "ASSOCIATION";
                        }
                        if (customarttribute.Value.ToString().Equals("11"))
                        {
                            clientSage.InfoLibre[3] = "ADMINISTRATION";
                        }
                        if (customarttribute.Value.ToString().Equals("12"))
                        {
                            clientSage.InfoLibre[3] = "CENTRALE D\'ACHATS";
                        }

                    }
                }
                //clientSage.InfoLibre[3] = "test";
                /*if (String.IsNullOrEmpty(UtilsConfig.CatTarif))
                {
                    // pas de configuration renseigner pour la cat tarif par defaut
                    // todo log
                }
                else
                {
                    clientSage.CatTarif = gescom.FactoryCategorieTarif.ReadIntitule(UtilsConfig.CatTarif);
                }
                if (String.IsNullOrEmpty(UtilsConfig.CompteG))
                {
                    // pas de configuration renseigner pour la cat tarif par defaut
                    // todo log
                }
                else
                {
                    clientSage.CompteGPrinc = compta.FactoryCompteG.ReadNumero(UtilsConfig.CompteGnum);
                }

                */
                if (!String.IsNullOrEmpty(customer.taxvat))
                {
                    clientSage.CT_Identifiant = customer.taxvat;
                }
                else
                {
                    clientSage.CT_Identifiant = "";
                }


                
            }
            catch (Exception e)
            {
                UtilsMail.SendErrorMail(DateTime.Now + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine, "CREATE CLIENT INFO G");
            }

            clientSage.Write();
/*
            try
            {
                IBOClientLivraison3 addrprinc = (IBOClientLivraison3)clientSage.FactoryClientLivraison.Create();

                if (!String.IsNullOrEmpty(order.Items[0]. ))//ExtensionAttributes.ShippingAssignments[0].Shipping.Address.company))
                {
                    addrprinc.LI_Intitule = order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.company.ToUpper();
                }
                else
                {
                    string intitule = "";
                    intitule = order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Firstname + " " + order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Lastname;
                    if (intitule.Length > 35)
                    {
                        intitule.Substring(0, 35);
                    }
                    addrprinc.LI_Intitule = intitule.ToUpper();
                }

                if (order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Street[0].ToString().Length > 35)
                {
                    addrprinc.Adresse.Adresse = order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Street[0].ToString().Substring(0, 35);
                }
                else
                {
                    addrprinc.Adresse.Adresse = order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Street[0].ToString();
                }
                if (order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Street.Count > 1)
                {
                    addrprinc.Adresse.Complement = order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Street[1].ToString();
                }
                addrprinc.Adresse.CodePostal = order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Postcode;
                addrprinc.Adresse.Ville = order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.City;
                var region = CultureInfo
                                    .GetCultures(CultureTypes.SpecificCultures)
                                    .Select(ci => new RegionInfo(ci.LCID))
                                    .FirstOrDefault(rg => rg.TwoLetterISORegionName == order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.CountryId);
                addrprinc.Adresse.Pays = region.DisplayName.ToString();
                addrprinc.Telecom.Telephone = order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Telephone.ToString();
                //addrprinc.Telecom.Telecopie = order.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Telecopie.ToString();

                if (String.IsNullOrEmpty(UtilsConfig.CondLivraison))
                {
                    // pas de configuration renseigner pour CondLivraison par defaut
                    // todo log
                }
                else
                {
                    addrprinc.ConditionLivraison = gescom.FactoryConditionLivraison.ReadIntitule(UtilsConfig.CondLivraison);
                }
                if (String.IsNullOrEmpty(UtilsConfig.Expedition))
                {
                    // pas de configuration renseigner pour Expedition par defaut
                    // todo log
                }
                else
                {
                    addrprinc.Expedition = gescom.FactoryExpedition.ReadIntitule(UtilsConfig.Expedition);
                }
                clientSage.LivraisonPrincipal = addrprinc;
                addrprinc.Write();
            }
            catch (Exception e)
            {
                UtilsMail.SendErrorMail(DateTime.Now + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine, "CREATE CLIENT ADRESS P");
            }


            // on envoie une notification à préstashop pour lui informer de la créeation dans SAGE du client
            //UtilsWebservices.SendDataNoParse(UtilsConfig.BaseUrl + EnumEndPoint.Client.Value, "updateCTnum&clientID=" + customer["id"].ToString() + "&ct_num=" + clientSage.CT_Num);
            //UtilsWebservices.SendDataNoParse(UtilsConfig.BaseUrl + EnumEndPoint.Client.Value, "updateIter&iter=" + clientSage.CT_Num);

            */
            return clientSage.CT_Num;
        }
        public static string CheckIfClientEmailExist(string email)
        {
            string ct_num = "";
            string sql1 = "SELECT CT_Num FROM [" + System.Configuration.ConfigurationManager.AppSettings["DBNAME"].ToString() + "].[dbo].[F_COMPTET] WHERE CT_EMail like '" + email + "'";
            //File.AppendAllText("Log\\SQL.txt", DateTime.Now + sql1.ToString() + Environment.NewLine);
            SqlDataReader CTnumEmailCompte = SingletonConnection.Instance.dB.Select(sql1);
            while (CTnumEmailCompte.Read())
            {
                if (!String.IsNullOrEmpty(CTnumEmailCompte.GetValue(0).ToString()))
                {
                    
                    ct_num = CTnumEmailCompte.GetValue(0).ToString();
                }
            }
            CTnumEmailCompte.Close();
            //DB.Disconnect();
            string sql2 = "SELECT CT_Num FROM [" + System.Configuration.ConfigurationManager.AppSettings["DBNAME"].ToString() + "].[dbo].[F_CONTACTT] WHERE CT_EMail like '" + email + "'";
            //File.AppendAllText("Log\\SQL.txt", DateTime.Now + sql2.ToString() + Environment.NewLine);
            SqlDataReader CTnumEmailContact = SingletonConnection.Instance.dB.Select(sql2);
            while (CTnumEmailContact.Read())
            {
                if (!String.IsNullOrEmpty(CTnumEmailContact.GetValue(0).ToString()))
                {
                    ct_num = CTnumEmailContact.GetValue(0).ToString();
                }
            }
            CTnumEmailContact.Close();
            //DB.Disconnect();
            return ct_num;
            //var compta = SingletonConnection.Instance.Gescom.CptaApplication;
            /*foreach (IBOClient3 client3 in SingletonConnection.Instance.Compta.FactoryClient.List)
            {
                if (client3.Telecom.EMail.ToUpper().Equals(email.ToUpper()))
                {
                    return client3.CT_Num;
                }
                else
                {
                    foreach (IBOTiersContact3 contact3 in client3.FactoryTiersContact.List)
                    {
                        if (contact3.Telecom.EMail.ToUpper().Equals(email.ToUpper()))
                        {
                            return client3.CT_Num;
                        }
                    }
                }
            }*/
        }
    }
}
