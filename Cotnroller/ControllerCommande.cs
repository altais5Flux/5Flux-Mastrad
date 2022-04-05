using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WebservicesSage.Utils;
using WebservicesSage.Utils.Enums;
using WebservicesSage.Singleton;
using Objets100cLib;
using WebservicesSage.Object;
using WebservicesSage.Object.DBObject;
using System.Windows.Forms;
using LiteDB;
using System.IO;
using Newtonsoft.Json;
using WebservicesSage.Object.Order;
using WebservicesSage.Object.CustomerSearch;
using System.Globalization;
using WebservicesSage.Object.Devis;
using Customer = WebservicesSage.Object.Customer;
using WebservicesSage.Object.CustomerSearchByEmail;
using System.Data.SqlClient;

namespace WebservicesSage.Cotnroller
{
    public static class ControllerCommande
    {

        /// <summary>
        /// Lance le service de check des nouvelles commandes prestashop
        /// Définir le temps de passage de la tâche dans la config
        /// </summary>
        public static void LaunchService()
        {
            //SingletonUI.Instance.LogBox.Invoke((MethodInvoker)(() => SingletonUI.Instance.LogBox.AppendText("Commande Services Launched " + Environment.NewLine)));
            
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(CheckForNewOrderMagento);
            timer.Interval = UtilsConfig.CronTaskCheckForNewOrder;
            timer.Enabled = true;
            /*
            System.Timers.Timer timerUpdateStatut = new System.Timers.Timer();
            timerUpdateStatut.Elapsed += new ElapsedEventHandler(UpdateStatuOrder);
            timerUpdateStatut.Interval = UtilsConfig.CronTaskUpdateStatut;
            timerUpdateStatu*t.Enabled = true;
            */
            
        }

        /// <summary>
        /// Event levé par une nouvelle commande dans prestashop
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public static void CheckForNewOrderMagento(object source, ElapsedEventArgs e)
        {
            try
            {
                string response = UtilsWebservices.SearchOrder(UtilsConfig.BaseUrl + "/rest/V1/orders", UtilsWebservices.SearchOrderCriteria("order_flag", UtilsConfig.Flag.ToString(), "eq"));
                OrderSearch orderSearch = OrderSearch.FromJson(response);
                if (orderSearch.TotalCount > 0)
                {
                    //todo create BC sage
                    for (int i = 0; i < orderSearch.TotalCount; i++)
                    {
                        string currentIdOrder = "0";
                        string currentIncrementedId = "";
                        currentIdOrder = orderSearch.Items[i].EntityId.ToString();
                        currentIncrementedId = orderSearch.Items[i].IncrementId.ToString();
                        try
                        {

                            if (
                                (orderSearch.Items[i].Status.Equals("canceled") ||
                                orderSearch.Items[i].Status.Equals("fraud") ||
                                orderSearch.Items[i].State.Equals("fraud") ||
                                orderSearch.Items[i].Status.Equals("payment_review") ||
                                orderSearch.Items[i].State.Equals("payment_review") ||
                                orderSearch.Items[i].State.Equals("canceled"))
                                && !UtilsConfig.Flag.Equals("9"))
                            {
                                var jsonFlag = JsonConvert.SerializeObject(UpdateOrderFlag(currentIdOrder, currentIncrementedId, "3"));
                                UtilsWebservices.SendDataJson(jsonFlag, @"rest/all/V1/orders/", "POST");
                                continue;
                            }
                            if (orderSearch.Items[i].Status.Equals("pending_payment") || orderSearch.Items[i].State.Equals("pending_payment"))
                            {
                                continue;
                            }
                            AddNewOrderForCustomerESHOP(orderSearch.Items[i]);
                        }
                        catch (Exception s)
                        {
                            if (s.Message.ToString().Equals("Cet élément est en cours d'utilisation !"))
                            {
                                StringBuilder erreur = new StringBuilder();
                                erreur.Append(DateTime.Now + " fiche client ouverte dans sage" + Environment.NewLine);
                                erreur.Append(DateTime.Now + " Erreur avec la commande : " + currentIncrementedId + Environment.NewLine);
                                erreur.Append(DateTime.Now + s.Message + Environment.NewLine);
                                erreur.Append(DateTime.Now + s.StackTrace + Environment.NewLine);
                                UtilsMail.SendErrorMail(DateTime.Now + s.Message + Environment.NewLine + s.StackTrace + Environment.NewLine, "COMMANDE LIGNE");
                                File.AppendAllText("Log\\order.txt", erreur.ToString());
                                erreur.Clear();
                            }
                            else
                            {
                                var jsonFlag = JsonConvert.SerializeObject(UpdateOrderFlag(currentIdOrder, currentIncrementedId, "2"));
                                UtilsWebservices.SendDataJson(jsonFlag, @"rest/all/V1/orders/", "POST");
                                StringBuilder sb = new StringBuilder();
                                sb.Append(DateTime.Now + " Erreur avec la commande : " + currentIncrementedId + Environment.NewLine);
                                sb.Append(DateTime.Now + s.Message + Environment.NewLine);
                                sb.Append(DateTime.Now + s.StackTrace + Environment.NewLine);
                                File.AppendAllText("Log\\order.txt", sb.ToString());
                                sb.Clear();
                            }
                        }
                    }
                }
            }
            catch (Exception s)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(DateTime.Now + s.Message + Environment.NewLine);
                sb.Append(DateTime.Now + s.StackTrace + Environment.NewLine);
                File.AppendAllText("Log\\order.txt", sb.ToString());
                sb.Clear();
                //var jsonFlag = JsonConvert.SerializeObject(UpdateOrderFlag(currentIdOrder, currentIncrementedId, "2"));
                //UtilsWebservices.SendDataJson(jsonFlag, @"rest/all/V1/orders/", "POST");
                //UtilsWebservices.SendDataNoParse(UtilsConfig.BaseUrl + EnumEndPoint.Commande.Value, "errorOrder&orderID=" + currentIdOrder);
            }

        }



        public static void CheckForNewOrderMagento()
        {

            try
            {
                string response = UtilsWebservices.SearchOrder(UtilsConfig.BaseUrl + "/rest/V1/orders", UtilsWebservices.SearchOrderCriteria("order_flag", UtilsConfig.Flag.ToString(), "eq"));
                File.AppendAllText("Log\\order.txt", response);
                OrderSearch orderSearch = OrderSearch.FromJson(response);
                if (orderSearch.TotalCount > 0)
                {
                    //todo create BC sage
                    for (int i = 0; i < orderSearch.TotalCount; i++)
                    {
                        string currentIdOrder = "0";
                        string currentIncrementedId = "";
                        currentIdOrder = orderSearch.Items[i].EntityId.ToString();
                        currentIncrementedId = orderSearch.Items[i].IncrementId.ToString();
                        try
                        {

                            if (
                                (orderSearch.Items[i].Status.Equals("canceled") ||
                                orderSearch.Items[i].Status.Equals("fraud") ||
                                orderSearch.Items[i].State.Equals("fraud") ||
                                orderSearch.Items[i].Status.Equals("payment_review") ||
                                orderSearch.Items[i].State.Equals("payment_review") ||
                                orderSearch.Items[i].State.Equals("canceled"))
                                && !UtilsConfig.Flag.Equals("9")
                                )
                            {
                                var jsonFlag = JsonConvert.SerializeObject(UpdateOrderFlag(currentIdOrder, currentIncrementedId, "3"));
                                UtilsWebservices.SendDataJson(jsonFlag, @"rest/all/V1/orders/", "POST");
                                continue;
                            }
                            if (orderSearch.Items[i].Status.Equals("pending_payment") || orderSearch.Items[i].State.Equals("pending_payment") || orderSearch.Items[i].Status.Equals("pending") || orderSearch.Items[i].State.Equals("pending"))
                            {
                                continue;
                            }
                            AddNewOrderForCustomerESHOP(orderSearch.Items[i]);
                        }
                        catch (Exception s)
                        {
                            if (s.Message.ToString().Equals("Cet élément est en cours d'utilisation !"))
                            {
                                StringBuilder erreur = new StringBuilder();
                                erreur.Append(DateTime.Now + " fiche client ouverte dans sage" + Environment.NewLine);
                                erreur.Append(DateTime.Now + " Erreur avec la commande : " + currentIncrementedId + Environment.NewLine);
                                erreur.Append(DateTime.Now + s.Message + Environment.NewLine);
                                erreur.Append(DateTime.Now + s.StackTrace + Environment.NewLine);
                                UtilsMail.SendErrorMail(DateTime.Now + s.Message + Environment.NewLine + s.StackTrace + Environment.NewLine, "COMMANDE LIGNE");
                                File.AppendAllText("Log\\order.txt", erreur.ToString());
                                erreur.Clear();
                            }
                            else
                            {
                                var jsonFlag = JsonConvert.SerializeObject(UpdateOrderFlag(currentIdOrder, currentIncrementedId, "2"));
                                UtilsWebservices.SendDataJson(jsonFlag, @"rest/all/V1/orders/", "POST");
                                StringBuilder sb = new StringBuilder();
                                sb.Append(DateTime.Now + " Erreur avec la commande : " + currentIncrementedId + Environment.NewLine);
                                sb.Append(DateTime.Now + s.Message + Environment.NewLine);
                                sb.Append(DateTime.Now + s.StackTrace + Environment.NewLine);
                                File.AppendAllText("Log\\order.txt", sb.ToString());
                                sb.Clear();
                            }
                        }
                    }
                }
            }
            catch (Exception s)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(DateTime.Now + s.Message + Environment.NewLine);
                sb.Append(DateTime.Now + s.StackTrace + Environment.NewLine);
                File.AppendAllText("Log\\order.txt", sb.ToString());
                sb.Clear();
                //var jsonFlag = JsonConvert.SerializeObject(UpdateOrderFlag(currentIdOrder, currentIncrementedId, "2"));
                //UtilsWebservices.SendDataJson(jsonFlag, @"rest/all/V1/orders/", "POST");
                //UtilsWebservices.SendDataNoParse(UtilsConfig.BaseUrl + EnumEndPoint.Commande.Value, "errorOrder&orderID=" + currentIdOrder);
            }

        }

        public static void UpdateStatuOrder(object source, ElapsedEventArgs e)
        {
            string test = "";
            try
            {
                var gescom = SingletonConnection.Instance.Gescom;
                var compta = SingletonConnection.Instance.Compta;
                
                //IBICollection AllOrders = gescom.FactoryDocumentVente.List;
                using (var db = new LiteDatabase(@"MyData.db"))
                {
                    // Get OrderMapping from Config

                    string statut1;
                    //string[] MagentoID, orderStatut;
                    //UtilsConfig.MagentoStatutId.TryGetValue("default", out MagentoStatutId);
                    //MagentoID = MagentoStatutId.Split('_');
                    string statutMagento1;
                    UtilsConfig.MagentoStatutId.TryGetValue("1", out statutMagento1);
                    //UtilsConfig.MagentoStatutId.TryGetValue(MagentoID[1], out statutMagento2);
                    //UtilsConfig.MagentoStatutId.TryGetValue(MagentoID[2], out statutMagento3);
                    //UtilsConfig.OrderMapping.TryGetValue("default", out orderStatutId);
                    //orderStatut = orderStatutId.Split('_');
                    UtilsConfig.OrderMapping.TryGetValue("1" , out statut1);
                    //UtilsConfig.OrderMapping.TryGetValue(orderStatut[1], out statut2);
                    //UtilsConfig.OrderMapping.TryGetValue(orderStatut[2], out statut3);
                    //statut1 =UtilsConfig.OrderMapping. //orderStatut[0];
                    //statut2 = orderStatut[1];
                    //statut3 = orderStatut[2];

                    // Get a collection (or create, if doesn't exist)
                    var col = db.GetCollection<LinkedCommandeDB>("Commande");
                    foreach (LinkedCommandeDB item in col.FindAll())
                    {
                        DocumentType OrderDocumentType = DocumentType.DocumentTypeVenteLivraison;
                        string sql = "SELECT DO_Type FROM [" + System.Configuration.ConfigurationManager.AppSettings["DBNAME"].ToString() + "].[dbo].[F_DOCENTETE] WHERE DO_Ref = '" + item.DO_Ref + "'"; ;
                        File.AppendAllText("Log\\SQL.txt", DateTime.Now + sql.ToString() + Environment.NewLine);
                        SqlDataReader orderType = SingletonConnection.Instance.dB.Select(sql);
                        while (orderType.Read())
                        {

                            File.AppendAllText("Log\\SQL.txt", DateTime.Now + orderType.GetValue(0).ToString() + Environment.NewLine);
                            if (orderType.GetValue(0).ToString().Equals("2"))
                            {
                                OrderDocumentType = DocumentType.DocumentTypeVentePrepaLivraison;
                            }
                            else if (orderType.GetValue(0).ToString().Equals("3"))
                            {
                                OrderDocumentType = DocumentType.DocumentTypeVenteLivraison;
                            }
                            else if (orderType.GetValue(0).ToString().Equals("6"))
                            {
                                OrderDocumentType = DocumentType.DocumentTypeVenteFacture;
                            }
                            else if (orderType.GetValue(0).ToString().Equals("7"))
                            {
                                OrderDocumentType = DocumentType.DocumentTypeVenteFactureCpta;
                            }
                        }
                        test = item.OrderID;
                        orderType.Close();
                        if (OrderDocumentType.ToString().Equals("DocumentTypeVenteLivraison"))
                        {
                            continue;
                        }
                        else
                        {
                            if (OrderDocumentType.ToString().Equals(statut1))
                            {
                                UtilsWebservices.SendDataJson(UpdateStatusOnMagento(item.OrderID, item.incremented_id, statutMagento1), @"rest/V1/orders");
                                item.OrderType = statut1.Split('_')[0];
                                col.Delete(item.Id);
                                //col.Update()
                                File.AppendAllText("Log\\statut.txt", DateTime.Now + " stat1  " + item.DO_Ref + " " + item.OrderType + Environment.NewLine);
                                continue;
                            }
                            /*if (OrderDocumentType.ToString().Equals(statut2.Split('_')[0]))
                            {
                                UtilsWebservices.SendDataJson(UpdateStatusOnMagento(item.OrderID, item.incremented_id, statutMagento2), @"rest/V1/orders");
                                item.OrderType = statut2.Split('_')[0];
                                col.Update(item);
                                File.AppendAllText("Log\\statut.txt", DateTime.Now + " stat2  " + item.DO_Ref +" "+ item.OrderType + Environment.NewLine);
                                //col.Update()
                                continue;
                            }
                            if (OrderDocumentType.ToString().Equals(statut3.Split('_')[0]))
                            {
                                UtilsWebservices.SendDataJson(UpdateStatusOnMagento(item.OrderID, item.incremented_id, statutMagento3), @"rest/V1/orders");
                                col.Delete(item.Id);
                                File.AppendAllText("Log\\statut.txt", DateTime.Now + " stat3  " + item.DO_Ref + " " + item.OrderType + Environment.NewLine);
                                continue;
                            }*/
                        }
                        //DB.Disconnect(); 
                    }
                }
            }
            catch(Exception s)
            {
                UtilsMail.SendErrorMail(DateTime.Now + s.Message + Environment.NewLine + s.StackTrace + Environment.NewLine, "UPDATE STATUT ORDER "+ test);
            }
        }

        public static void UpdateStatuOrder()
        {
            string test = "";
            try
            {
                var gescom = SingletonConnection.Instance.Gescom;
                var compta = SingletonConnection.Instance.Compta;

                //IBICollection AllOrders = gescom.FactoryDocumentVente.List;
                using (var db = new LiteDatabase(@"MyData.db"))
                {
                    // Get OrderMapping from Config

                    string  statut1;
                    string[] orderStatut;
                    //UtilsConfig.MagentoStatutId.TryGetValue("default", out MagentoStatutId);
                    //MagentoID = MagentoStatutId.Split('_');
                    string statutMagento1;
                    UtilsConfig.MagentoStatutId.TryGetValue("default", out statutMagento1);
                    //UtilsConfig.MagentoStatutId.TryGetValue(MagentoID[1], out statutMagento2);
                    //UtilsConfig.MagentoStatutId.TryGetValue(MagentoID[2], out statutMagento3);
                    //UtilsConfig.OrderMapping.TryGetValue("default", out orderStatutId);
                    //orderStatut = orderStatutId.Split('_');
                    UtilsConfig.OrderMapping.TryGetValue("default", out statut1);
                    orderStatut = statut1.Split('_');
                    statut1 = orderStatut[0].ToString();
                    //UtilsConfig.OrderMapping.TryGetValue(orderStatut[1], out statut2);
                    //UtilsConfig.OrderMapping.TryGetValue(orderStatut[2], out statut3);
                    //statut1 =UtilsConfig.OrderMapping. //orderStatut[0];
                    //statut2 = orderStatut[1];
                    //statut3 = orderStatut[2];

                    // Get a collection (or create, if doesn't exist)
                    var col = db.GetCollection<LinkedCommandeDB>("Commande");
                    foreach (LinkedCommandeDB item in col.FindAll())
                    {
                        DocumentType OrderDocumentType = DocumentType.DocumentTypeVenteLivraison;
                        string sql = "SELECT DO_Type FROM [" + System.Configuration.ConfigurationManager.AppSettings["DBNAME"].ToString() + "].[dbo].[F_DOCENTETE] WHERE DO_Ref = '" + item.DO_Ref + "'"; ;
                        File.AppendAllText("Log\\SQL.txt", DateTime.Now + sql.ToString() + Environment.NewLine);
                        SqlDataReader orderType = SingletonConnection.Instance.dB.Select(sql);
                        while (orderType.Read())
                        {

                            File.AppendAllText("Log\\SQL.txt", DateTime.Now + orderType.GetValue(0).ToString() + Environment.NewLine);
                            if (orderType.GetValue(0).ToString().Equals("2"))
                            {
                                OrderDocumentType = DocumentType.DocumentTypeVentePrepaLivraison;
                            }
                            else if (orderType.GetValue(0).ToString().Equals("3"))
                            {
                                OrderDocumentType = DocumentType.DocumentTypeVenteLivraison;
                            }
                            else if (orderType.GetValue(0).ToString().Equals("6"))
                            {
                                OrderDocumentType = DocumentType.DocumentTypeVenteFacture;
                            }
                            else if (orderType.GetValue(0).ToString().Equals("7"))
                            {
                                OrderDocumentType = DocumentType.DocumentTypeVenteFactureCpta;
                            }
                        }
                        test = item.OrderID;
                        orderType.Close();
                        if (OrderDocumentType.ToString().Equals("DocumentTypeVenteLivraison"))
                        {
                            continue;
                        }
                        else
                        {
                            if (OrderDocumentType.ToString().Equals(statut1))
                            {
                                UtilsWebservices.SendDataJson(UpdateStatusOnMagento(item.OrderID, item.incremented_id, statutMagento1), @"rest/V1/orders");
                                item.OrderType = statut1.Split('_')[0];
                                col.Delete(item.Id);
                                //col.Update()
                                File.AppendAllText("Log\\statut.txt", DateTime.Now + " stat1  " + item.DO_Ref + " " + item.OrderType + Environment.NewLine);
                                continue;
                            }
                            /*if (OrderDocumentType.ToString().Equals(statut2.Split('_')[0]))
                            {
                                UtilsWebservices.SendDataJson(UpdateStatusOnMagento(item.OrderID, item.incremented_id, statutMagento2), @"rest/V1/orders");
                                item.OrderType = statut2.Split('_')[0];
                                col.Update(item);
                                File.AppendAllText("Log\\statut.txt", DateTime.Now + " stat2  " + item.DO_Ref +" "+ item.OrderType + Environment.NewLine);
                                //col.Update()
                                continue;
                            }
                            if (OrderDocumentType.ToString().Equals(statut3.Split('_')[0]))
                            {
                                UtilsWebservices.SendDataJson(UpdateStatusOnMagento(item.OrderID, item.incremented_id, statutMagento3), @"rest/V1/orders");
                                col.Delete(item.Id);
                                File.AppendAllText("Log\\statut.txt", DateTime.Now + " stat3  " + item.DO_Ref + " " + item.OrderType + Environment.NewLine);
                                continue;
                            }*/
                        }
                        //DB.Disconnect(); 
                    }
                }
            }
            catch (Exception s)
            {
                UtilsMail.SendErrorMail(DateTime.Now + s.Message + Environment.NewLine + s.StackTrace + Environment.NewLine, "UPDATE STATUT ORDER " + test);
            }
        }
        public static void AddNewOrderForCustomerESHOP(OrderSearchItem orderMagento)
        {
            var gescom = SingletonConnection.Instance.Gescom;

            // création de l'entête de la commande 
            try
            {
                IBOClient3 customer = gescom.CptaApplication.FactoryClient.ReadNumero(UtilsConfig.CtNumClient);
                IBODocumentVente3 order = gescom.FactoryDocumentVente.CreateType(DocumentType.DocumentTypeVenteLivraison);
                int BillingLI_NO = 0;
                int ShippingLi_No = 0;
                string intitulAdressLivraison = "";
                string intitulAdressFacturation = "";
                order.SetDefault();
                order.SetDefaultClient(customer);
                order.DO_Date = DateTime.Now;

                order.Souche = gescom.FactorySoucheVente.ReadIntitule(UtilsConfig.Souche);
                string intituleOrder = "";
                if (!String.IsNullOrEmpty(orderMagento.ExtensionAttributes.MarketPlaceName.ToString()))
                {
                    intituleOrder = orderMagento.IncrementId.ToString() + " " + orderMagento.ExtensionAttributes.MarketPlaceName.ToString();
                }
                else
                {
                    intituleOrder = orderMagento.IncrementId.ToString();
                }
                if (intituleOrder.Length >17)
                {
                    order.DO_Ref = intituleOrder.Substring(0,17);
                }
                else
                {
                    order.DO_Ref = intituleOrder;
                }
                
                order.SetDefaultDO_Piece();
                string carrier_id = "default";
                try
                {


                    if (orderMagento.ExtensionAttributes.ShippingAssignments[0].Shipping.Method.Equals("mondialrelay_pickup"))
                    {
                        carrier_id = "1";
                    }
                    else if (orderMagento.ExtensionAttributes.ShippingAssignments[0].Shipping.Method.Equals("colissimo_homecl"))
                    {
                        carrier_id = "2";
                    }else
                    {
                        carrier_id = "3";
                    }
                    order.Expedition = gescom.FactoryExpedition.ReadIntitule(UtilsConfig.OrderCarrierMapping[carrier_id]);
                }
                catch (Exception s)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(DateTime.Now + "problème order expidition" + Environment.NewLine);
                    sb.Append(DateTime.Now + s.Message + Environment.NewLine);
                    sb.Append(DateTime.Now + s.StackTrace + Environment.NewLine);
                    UtilsMail.SendErrorMail(DateTime.Now + s.Message + Environment.NewLine + s.StackTrace + Environment.NewLine, "TRANSPORTEUR");
                    File.AppendAllText("Log\\order.txt", sb.ToString());
                    sb.Clear();
                }


                //order.FraisExpedition = orderMagento.ExtensionAttributes.ShippingAssignments[0].Shipping.Total.ShippingAmount;
                string shippingAdressStreet = "";
                string billingAdressStreet = "";
                if(orderMagento.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Street[0].Length > 35)
                {
                    shippingAdressStreet = orderMagento.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Street[0].Substring(0, 35);
                }
                else
                {
                    shippingAdressStreet = orderMagento.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Street[0];
                }

                if (orderMagento.BillingAddress.Street[0].Length > 35)
                {
                    billingAdressStreet = orderMagento.BillingAddress.Street[0].Substring(0, 35);
                }
                else
                {
                    billingAdressStreet = orderMagento.BillingAddress.Street[0];
                }

                if (ExistAdress(shippingAdressStreet, orderMagento.CustomerEmail))
                {
                    intitulAdressLivraison = GetIntitulAdress(shippingAdressStreet, orderMagento.CustomerEmail);
                }
                if (ExistAdress(billingAdressStreet, orderMagento.CustomerEmail))
                {
                    intitulAdressFacturation = GetIntitulAdress(billingAdressStreet, orderMagento.CustomerEmail);
                }
                if (String.IsNullOrEmpty(intitulAdressFacturation) && String.IsNullOrEmpty(intitulAdressLivraison))
                {
                    if (shippingAdressStreet.Equals(billingAdressStreet))
                    {
                        //File.AppendAllText("Log\\test.txt", DateTime.Now + " Création addresse livraison/facturation : " + Environment.NewLine);
                        createBtoCAdress(orderMagento, customer, "Livraison/Facturation", carrier_id);
                        intitulAdressFacturation = GetIntitulAdress(billingAdressStreet, orderMagento.CustomerEmail);
                        intitulAdressLivraison = GetIntitulAdress(shippingAdressStreet, orderMagento.CustomerEmail);
                        //File.AppendAllText("Log\\test.txt", DateTime.Now + " Création addresse livraison/facturation : " + intitulAdressLivraison + Environment.NewLine);
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(intitulAdressFacturation))
                        {
                            createBtoCBillingAdress(orderMagento, customer, "Facturation", carrier_id);
                            //File.AppendAllText("Log\\test.txt", DateTime.Now + " Création addresse facturation : " + Environment.NewLine);
                        }
                        if (String.IsNullOrEmpty(intitulAdressLivraison))
                        {
                            createBtoCShippingAdress(orderMagento, customer, "Livraison", carrier_id);
                            //File.AppendAllText("Log\\test.txt", DateTime.Now + " Création addresse livraison : " + Environment.NewLine);
                        }
                    }
                }

                //affectation de l'adress de livraison à la commande
                intitulAdressLivraison = GetIntitulAdress(shippingAdressStreet, orderMagento.CustomerEmail);
                //File.AppendAllText("Log\\test.txt", "order adress to find : " + intitulAdressLivraison + Environment.NewLine);
                if (String.IsNullOrEmpty(intitulAdressLivraison))
                {
                    createBtoCShippingAdress(orderMagento, customer, "Livraison", carrier_id);
                    //File.AppendAllText("Log\\test.txt", DateTime.Now + " Création addresse livraison avant affectation à la commande: " + Environment.NewLine);
                }

                intitulAdressLivraison = GetIntitulAdress(shippingAdressStreet, orderMagento.CustomerEmail);
                IBOClient3 customer2 = gescom.CptaApplication.FactoryClient.ReadNumero(UtilsConfig.CtNumClient);
                foreach (IBOClientLivraison3 item in customer2.FactoryClientLivraison.List)
                {
                    
                    if (item.LI_Intitule.Equals(intitulAdressLivraison))
                    {
                        order.LieuLivraison = item;
                        order.Write();
                        //File.AppendAllText("Log\\test.txt", "found " + intitulAdressLivraison + Environment.NewLine);
                        break;
                    }
                }
                // order.LieuLivraison = adress;
                //File.AppendAllText("Log\\test.txt", "order adress " + order.LieuLivraison.LI_Intitule + Environment.NewLine);

                order.Write();
                if (orderMagento.Payment.Method.Equals("systempay_standard"))
                {
                    order.InfoLibre[3] = "Carte bancaire";
                }
                else if (orderMagento.Payment.Method.Equals("paypal_express"))
                {
                    order.InfoLibre[3] ="Paypal";
                }

                // création des lignes de la commandes
                try
                {
                    foreach (Object.Order.ParentItemElement product in orderMagento.Items)
                    {
                        if (product.ProductType.Equals("configurable"))
                        {
                            continue;
                        }
                        IBODocumentVenteLigne3 docLigne = (IBODocumentVenteLigne3)order.FactoryDocumentLigne.Create();
                        var ArticleExist = gescom.FactoryArticle.ExistReference(product.Sku);
                        docLigne.SetDefaultArticle(gescom.FactoryArticle.ReadReference(product.Sku), Int32.Parse(product.QtyOrdered.ToString()));
                        if (product.Price == 0)
                        {
                            foreach (Object.Order.ParentItemElement productConfigurable in orderMagento.Items)
                            {
                                if (productConfigurable.ProductId == product.ParentItemId)
                                {
                                    docLigne.DL_PrixUnitaire = productConfigurable.ParentItem.Price;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            docLigne.DL_PrixUnitaire = product.Price;
                        }
                        
                        docLigne.Write();
                    }
                    IBODocumentLigne3 docLignePort = (IBODocumentLigne3)order.FactoryDocumentLigne.Create();
                    IBOArticle3 articlePort = gescom.FactoryArticle.ReadReference(UtilsConfig.DefaultTransportReference);

                    docLignePort.SetDefaultArticle(articlePort, Int32.Parse("1"));
                    docLignePort.DL_PrixUnitaire = orderMagento.ExtensionAttributes.ShippingAssignments[0].Shipping.Total.ShippingAmount;
                    docLignePort.Write();
                    if (orderMagento.ExtensionAttributes.MarketPlaceFees > 0)
                    {
                        IBODocumentLigne3 docLigneFees = (IBODocumentLigne3)order.FactoryDocumentLigne.Create();
                        IBOArticle3 articleRemise = gescom.FactoryArticle.ReadReference(UtilsConfig.DefaultRemiseReference);

                        docLigneFees.SetDefaultArticle(articleRemise, Int32.Parse("1"));
                        docLigneFees.DL_PrixUnitaire = orderMagento.ExtensionAttributes.MarketPlaceFees / 1.2;
                        docLigneFees.Write();
                    }
                    if (orderMagento.DiscountAmount != 0)
                    {
                        double price = Convert.ToDouble(orderMagento.DiscountAmount);
                        IBODocumentLigne3 docLigneCoupon = (IBODocumentLigne3)order.FactoryDocumentLigne.Create();
                        IBOArticle3 articleCouon = gescom.FactoryArticle.ReadReference("COUPONWEB");

                        docLigneCoupon.SetDefaultArticle(articleCouon, Int32.Parse("1"));
                        docLigneCoupon.DL_PrixUnitaire  = orderMagento.DiscountAmount / 1.2;
                        docLigneCoupon.Write();
                    }

                }
                catch (Exception e)
                {
                    //UtilsWebservices.UpdateOrderFlag(orderMagento.EntityId.ToString(), "2");
                    var jsonFlag2 = JsonConvert.SerializeObject(UpdateOrderFlag(orderMagento.EntityId.ToString(), orderMagento.IncrementId.ToString(), "2"));
                    UtilsWebservices.SendDataJson(jsonFlag2, @"rest/all/V1/orders", "POST");
                    StringBuilder sb = new StringBuilder();
                    sb.Append(DateTime.Now + e.Message + Environment.NewLine);
                    sb.Append(DateTime.Now + e.StackTrace + Environment.NewLine);
                    UtilsMail.SendErrorMail(DateTime.Now + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine, "COMMANDE LIGNE");
                    File.AppendAllText("Log\\order.txt", sb.ToString());
                    sb.Clear();
                    // UtilsWebservices.SendDataNoParse(UtilsConfig.BaseUrl + EnumEndPoint.Commande.Value, "errorOrder&orderID=" + jsonOrder["id_order"]);
                    order.Remove();
                    return;
                }
                //order.FraisExpedition = orderMagento.ExtensionAttributes.ShippingAssignments[0].Shipping.Total.ShippingAmount;
                order.Write();
                addOrderToLocalDB(orderMagento.EntityId.ToString(), order.Client.CT_Num, order.DO_Piece, order.DO_Ref, orderMagento.IncrementId);
                // TODO updateOrderFlag using custom PHP script

                var jsonFlag = JsonConvert.SerializeObject(UpdateOrderFlag(orderMagento.EntityId.ToString(), orderMagento.IncrementId.ToString(), "0"));
                UtilsWebservices.SendDataJson(jsonFlag, @"rest/all/V1/orders", "POST");
            }
            catch (Exception s)
            {
                var jsonFlag2 = JsonConvert.SerializeObject(UpdateOrderFlag(orderMagento.EntityId.ToString(), orderMagento.IncrementId.ToString(), "2"));
                UtilsWebservices.SendDataJson(jsonFlag2, @"rest/all/V1/orders", "POST");
                StringBuilder sb = new StringBuilder();
                sb.Append(DateTime.Now + s.Message + Environment.NewLine);
                sb.Append(DateTime.Now + s.StackTrace + Environment.NewLine);
                UtilsMail.SendErrorMail(DateTime.Now + s.Message + Environment.NewLine + s.StackTrace + Environment.NewLine, "COMMANDE LIGNE");
                File.AppendAllText("Log\\order.txt", sb.ToString());
                sb.Clear();
                // UtilsWebservices.SendDataNoParse(UtilsConfig.BaseUrl + EnumEndPoint.Commande.Value, "errorOrder&orderID=" + jsonOrder["id_order"]);
                return;
            }
        }
        
        public static void CreateDevis(DataGridView devisList)
        {
            string clientCtNum="";
            string clienttype = "";
            foreach (DataGridViewRow item in devisList.Rows)
            {
                if (item.Cells[0].Value.ToString().Equals("True"))
                {

                    string currentIdDevis = item.Cells[1].Value.ToString(); //orderSearch.Items[i].EntityId.ToString();
                    var DevisSearch = Object.Devis.Devis.FromJson(UtilsWebservices.GetMagentoData("rest/V1/amasty_quote/search"+ UtilsWebservices.SearchOrderCriteria("quote_id",currentIdDevis,"eq")));
                    if (DevisSearch.TotalCount > 0 & !String.IsNullOrEmpty(DevisSearch.Items[0].Customer.Id.ToString()))
                    {
                        CustomerSearch client = UtilsWebservices.GetClientCtNum(DevisSearch.Items[0].Customer.Id.ToString());
                        try
                        {
                            for (int j = 0; j < client.CustomAttributes.Count; j++)
                            {
                                if (client.CustomAttributes[j].AttributeCode.Equals("sage_number"))
                                {
                                    clientCtNum = client.CustomAttributes[j].Value.ToString();
                                }
                                    if (client.CustomAttributes[j].AttributeCode.Equals("customer_type"))
                                    {
                                        clienttype = client.CustomAttributes[j].Value.ToString();
                                    }
                            }
                        }
                        catch (Exception e)
                        {

                            clientCtNum = "";
                        }
                        if (ControllerClient.CheckIfClientExist(clientCtNum))
                        {

                            // si le client existe on associé la devis à son compte
                            AddNewDevisForCustomer(DevisSearch.Items[0], clientCtNum, client);

                        }
                        else
                        {/*
                            // si le client n'existe pas on récupère les info de magento et on le crée dans la base sage 
                            //string client = UtilsWebservices.SendDataNoParse(UtilsConfig.BaseUrl + EnumEndPoint.Client.Value, "getClient&clientID=" + order["id_customer"]);
                            string ct_num = ControllerClient.CreateNewClientDevis(client, DevisSearch);
                            Object.Customer customerMagento = new Object.Customer();
                            var jsonClient = JsonConvert.SerializeObject(customerMagento.UpdateCustomer(ct_num, clienttype, client.Id.ToString()));
                            UtilsWebservices.SendDataJson(jsonClient, @"rest/all/V1/customers/" + client.Id.ToString(), "PUT");
                            if (!String.IsNullOrEmpty(ct_num))
                            {
                                // le client à bien été crée on peut intégrer la commande sur son compte sage
                                AddNewDevisForCustomer(DevisSearch.Items[0], ct_num, client);
                            }*/


                            CustomerSearchByEmail ClientSearch = CustomerSearchByEmail.FromJson(UtilsWebservices.GetMagentoData("rest/V1/customers/search" + UtilsWebservices.SearchOrderCriteria("email", client.Email, "eq")));
                            Customer customerMagento = new Customer();
                            string clientSageObj = ControllerClient.CheckIfClientEmailExist(client.Email);
                            if (!String.IsNullOrEmpty(clientSageObj))
                            {
                                IBOClient3 customerSage = SingletonConnection.Instance.Gescom.CptaApplication.FactoryClient.ReadNumero(clientSageObj);
                                Client ClientData = new Client(customerSage);
                                var jsonClient = JsonConvert.SerializeObject(customerMagento.UpdateCustomer(clientSageObj, clienttype, client));
                                UtilsWebservices.SendDataJson(jsonClient, @"rest/all/V1/customers/" + ClientSearch.Items[0].Id.ToString(), "PUT");
                                AddNewDevisForCustomer(DevisSearch.Items[0], clientSageObj, client);
                            }
                            else
                            {
                                string ct_num = ControllerClient.CreateNewClientDevis(client, DevisSearch);//.Items[0]);

                                if (!String.IsNullOrEmpty(ct_num))
                                {
                                    var jsonClient = JsonConvert.SerializeObject(customerMagento.UpdateCustomer(ct_num, clienttype, client));
                                    UtilsWebservices.SendDataJson(jsonClient, @"rest/all/V1/customers/" + client.Id.ToString(), "PUT");
                                    // le client à bien été crée on peut intégrer la commande sur son compte sage
                                    AddNewDevisForCustomer(DevisSearch.Items[0], ct_num, client);
                                }
                            }
                        }
                    }
 
                }   
               
            }
        }

        private static void AddNewDevisForCustomer(DevisItem devisItem, string ct_num, CustomerSearch client)
        {
            var gescom = SingletonConnection.Instance.Gescom;

            // création de l'entête de la commande 

            IBOClient3 customer = gescom.CptaApplication.FactoryClient.ReadNumero(ct_num);
            IBODocumentVente3 order = gescom.FactoryDocumentVente.CreateType(DocumentType.DocumentTypeVenteDevis);
            order.SetDefault();
            order.SetDefaultClient(customer);
            order.DO_Date = DateTime.Now;
            order.Souche = gescom.FactorySoucheVente.ReadIntitule(UtilsConfig.Souche);
            order.DO_Ref = "WEB " + devisItem.Id.ToString();//orderMagento.EntityId.ToString();
            order.SetDefaultDO_Piece();

            order.Write();
            // création des lignes de la commandes
            try
            {
                foreach (Object.Devis.ItemItem product in devisItem.Items)
                {
                    if (product.ProductType.Equals("configurable"))
                    {
                        continue;
                    }
                    IBODocumentLigne3 docLigne = (IBODocumentLigne3)order.FactoryDocumentLigne.Create();
                    var ArticleExist = gescom.FactoryArticle.ExistReference(product.Sku);
                    if (ArticleExist)
                    {
                        IBOArticle3 article1 = gescom.FactoryArticle.ReadReference(product.Sku.ToString());
                        Article CondArticle = new Article(article1);
                        if (CondArticle.conditionnements.Count > 0)
                        {
                            //String[] SKU = product.Sku.Split('|');
                            //IBOArticle3 article1 = gescom.FactoryArticle.ReadReference(SKU[0].ToString());

                            IBOArticleCond3 articleCond3 = ControllerArticle.GetArticleConditionnementEnum(article1);
                            docLigne.SetDefaultArticleConditionnement(articleCond3, Int32.Parse(product.Qty.ToString()));
                        }
                        else
                        {
                            docLigne.DL_PrixUnitaire = double.Parse(product.Price.ToString(), System.Globalization.CultureInfo.InvariantCulture);
                            // produit simple
                            docLigne.SetDefaultArticle(gescom.FactoryArticle.ReadReference(product.Sku), Int32.Parse(product.Qty.ToString()));
                        }
                        //SHipping price

                        /*if (product["product_ref"].ToString().Equals("TRANSPORT"))
                        {
                            docLigne.DL_PrixUnitaire = Convert.ToDouble(orderMagento.ShippingAmount.ToString().Replace('.', ','));
                        }
                        else if (product["product_ref"].ToString().Equals("REMISE"))
                        {
                            docLigne.DL_PrixUnitaire = Convert.ToDouble(product.Price.ToString().Replace('.', ','));
                        }*/
                    }
                    else
                    {
                        // on récupère la chaine de gammages d'un produit
                        string product_attribut_string = GetParentProductDetails(product.Sku).ToString();
                        String[] subgamme = product_attribut_string.Split('|');
                        IBOArticle3 article = gescom.FactoryArticle.ReadReference(subgamme[0].ToString());
                        if (subgamme.Length == 3)
                        {
                            // produit à simple gamme
                            IBOArticleGammeEnum3 articleEnum = ControllerArticle.GetArticleGammeEnum1(article, new Gamme(subgamme[1], subgamme[2]));
                            docLigne.SetDefaultArticleMonoGamme(articleEnum, Int32.Parse(product.Qty.ToString()));
                        }
                        else if (subgamme.Length == 5)
                        {
                            // produit à double gamme
                            IBOArticleGammeEnum3 articleEnum = ControllerArticle.GetArticleGammeEnum1(article, new Gamme(subgamme[1], subgamme[2], subgamme[3], subgamme[4]));
                            IBOArticleGammeEnum3 articleEnum2 = ControllerArticle.GetArticleGammeEnum2(article, new Gamme(subgamme[1], subgamme[2], subgamme[3], subgamme[4]));
                            docLigne.SetDefaultArticleDoubleGamme(articleEnum, articleEnum2, Int32.Parse(product.Qty.ToString()));
                        }
                    }
                    docLigne.Write();
                }
                
            }
            catch (Exception e)
            {
                //UtilsWebservices.UpdateOrderFlag(order.EntityId.ToString(), "2");
                StringBuilder sb = new StringBuilder();
                sb.Append(DateTime.Now + e.Message + Environment.NewLine);
                sb.Append(DateTime.Now + e.StackTrace + Environment.NewLine);
                //UtilsMail.SendErrorMail(DateTime.Now + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine, "COMMANDE LIGNE");
                File.AppendAllText("Log\\Devis.txt", sb.ToString());
                sb.Clear();
                // UtilsWebservices.SendDataNoParse(UtilsConfig.BaseUrl + EnumEndPoint.Commande.Value, "errorOrder&orderID=" + jsonOrder["id_order"]);
                order.Remove();
                return;
            }
        }

        private static void addOrderToLocalDB(string orderID, string CT_Num, string DO_piece, string DO_Ref, string incremented_id)
        {
            // Open database (or create if doesn't exist)
            using (var db = new LiteDatabase(@"MyData.db"))
            {
                // Get a collection (or create, if doesn't exist)
                var col = db.GetCollection<LinkedCommandeDB>("Commande");

                // Create your new customer instance
                var commande = new LinkedCommandeDB
                {
                    OrderID = orderID,
                    OrderType = "DocumentTypeVenteLivraison",
                    CT_Num = CT_Num,
                    DO_piece = DO_piece,
                    DO_Ref = DO_Ref,
                    incremented_id = incremented_id

                };
                col.Insert(commande);
            }
        }

        public static string GetPrestaOrderStatutFromMapping(DocumentType orderSageType)
        {
            string prestaType;
            if(UtilsConfig.OrderMapping.TryGetValue(orderSageType.ToString(), out prestaType))
            {
                return prestaType;
            }
            else
            {
                return null;
            }
        }

        public static string UpdateStatusOnMagento(string orderID, string incremented_id , string status)
        {
            var UpdateOrder = new
            {
                entity = new
                {
                    entity_id = orderID,
                    increment_id = incremented_id,
                    status = status
                }
            };
            return JsonConvert.SerializeObject(UpdateOrder);
        }
        public static object UpdateOrderFlag(string orderID, string incremented_id, string flag)
        {
            var updateFlag = new
            {
                entity = new
                {
                    entity_id = orderID,
                    increment_id = incremented_id,
                    extension_attributes = new
                    {
                        order_flag = flag
                    }
                }
            };
            return updateFlag;
        }
        public static StringBuilder GetParentProductDetails(string sku)
        {
            var gescom = SingletonConnection.Instance.Gescom;
            var articlesSageObj = gescom.FactoryArticle.List;
            StringBuilder results = new StringBuilder();
            results.Append("");
            foreach (IBOArticle3 articleSage in articlesSageObj)
            {
                // on check si l'article est cocher en publier sur le site marchand
                if (!articleSage.AR_Publie)
                    continue;
                Article article = new Article(articleSage);
                if (article.isGamme)
                {
                    foreach (Gamme doubleGamme in article.Gammes)
                    {
                        if (article.IsDoubleGamme)
                        {
                            if (doubleGamme.Reference.Equals(sku))
                            {
                                results.Append(article.Reference);
                                results.Append("|");
                                results.Append(doubleGamme.Intitule);
                                results.Append("|");
                                results.Append(doubleGamme.Value_Intitule);
                                results.Append("|");
                                results.Append(doubleGamme.Intitule2);
                                results.Append("|");
                                results.Append(doubleGamme.Value_Intitule2);
                                return results;
                            }
                        }
                        else
                        {
                            if (doubleGamme.Reference.Equals(sku))
                            {
                                results.Append(article.Reference);
                                results.Append("|");
                                results.Append(doubleGamme.Intitule);
                                results.Append("|");
                                results.Append(doubleGamme.Value_Intitule);
                                return results;
                            }
                        }
                    }
                }
            }
            return results;
        }
        public static bool HasMethod(this object objectToCheck, string methodName)
        {
            var type = objectToCheck.GetType();
            return type.GetMethod(methodName) != null;
        }

        public static void createBtoCShippingAdress(OrderSearchItem orderMagento, IBOClient3 customer, string AdressType,string expedition)
        {
            File.AppendAllText("Log\\test.txt", DateTime.Now + " begin create shipping adress : " + orderMagento.IncrementId.ToString() + Environment.NewLine);
            int AdressNumber = 0;
            SqlDataReader DbAdressNumber = SingletonConnection.Instance.dB.Select("select count(*) from [" + System.Configuration.ConfigurationManager.AppSettings["DBNAME"].ToString() + "].[dbo].[F_LIVRAISON] where LI_EMail like '" + orderMagento.CustomerEmail.ToString() + "'");
            while (DbAdressNumber.Read())
            {
                AdressNumber = Int32.Parse(DbAdressNumber.GetValue(0).ToString());
            }
            DbAdressNumber.Close();
            // requete SQL
            if (AdressNumber == 0)
            {
                AdressNumber = 1;
            }
            else
            {
                AdressNumber++;
            }
            IBOClientLivraison3 adress = (IBOClientLivraison3)customer.FactoryClientLivraison.Create();
            adress.SetDefault();
            string intitule;
            if (AdressNumber > 9)
            {
                intitule = orderMagento.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Lastname + " " + orderMagento.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Firstname.ToUpper() + " " + AdressNumber.ToString();
            }
            else
            {
                intitule = orderMagento.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Lastname + " " + orderMagento.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Firstname.ToUpper() + " 0" + AdressNumber.ToString();
            }
            adress.LI_Intitule = intitule;
            adress.LI_Contact = AdressType;

            if (orderMagento.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Street[0].Length > 35)
            {
                adress.Adresse.Adresse = orderMagento.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Street[0].Substring(0, 35);
            }
            else
            {
                adress.Adresse.Adresse = orderMagento.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Street[0];
            }


            adress.Telecom.EMail = orderMagento.CustomerEmail.ToString();
            adress.Telecom.Telephone = orderMagento.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Telephone.ToString();
            adress.Adresse.CodePostal = orderMagento.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Postcode.ToString();
            adress.Adresse.Ville = orderMagento.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.City.ToString();
            if (HasMethod(orderMagento.ExtensionAttributes.ShippingAssignments[0].Shipping.Address, "Region"))
            {
                adress.Adresse.CodeRegion = orderMagento.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.Region.ToString();
            }
            adress.Adresse.Pays = ISO3166.FromAlpha2(orderMagento.ExtensionAttributes.ShippingAssignments[0].Shipping.Address.CountryId.ToString()).Name;
            adress.Expedition = SingletonConnection.Instance.Gescom.FactoryExpedition.ReadIntitule(UtilsConfig.OrderCarrierMapping[expedition]);
            try
            {
                adress.Write();
            }
            catch (Exception ex)
            {

            }
            File.AppendAllText("Log\\test.txt", DateTime.Now + " end create shipping adress : " + intitule + Environment.NewLine);
        }

        public static void createBtoCBillingAdress(OrderSearchItem orderMagento, IBOClient3 customer, string AdressType, string expedition)
        {
            File.AppendAllText("Log\\test.txt", DateTime.Now + " begin create billing adress : " + orderMagento.IncrementId.ToString() + Environment.NewLine);
            int AdressNumber = 0;
            SqlDataReader DbAdressNumber = SingletonConnection.Instance.dB.Select("select count(*) from [" + System.Configuration.ConfigurationManager.AppSettings["DBNAME"].ToString() + "].[dbo].[F_LIVRAISON] where LI_EMail like '" + orderMagento.BillingAddress.Email.ToString() + "'");
            while (DbAdressNumber.Read())
            {
                AdressNumber = Int32.Parse(DbAdressNumber.GetValue(0).ToString());
            }
            DbAdressNumber.Close();
            // requete SQL
            if (AdressNumber == 0)
            {
                AdressNumber = 1;
            }
            else
            {
                AdressNumber++;
            }
            IBOClientLivraison3 adress = (IBOClientLivraison3)customer.FactoryClientLivraison.Create();
            adress.SetDefault();
            string intitule = "";
            if (AdressNumber > 9)
            {
                intitule = orderMagento.BillingAddress.Lastname + " " + orderMagento.BillingAddress.Firstname.ToUpper() + " " + AdressNumber.ToString();
            }
            else
            {
                intitule = orderMagento.BillingAddress.Lastname + " " + orderMagento.BillingAddress.Firstname.ToUpper() + " 0" + AdressNumber.ToString();
            }
            adress.LI_Intitule = intitule;
            adress.LI_Contact = AdressType;


            if (orderMagento.BillingAddress.Street[0].Length > 35)
            {
                adress.Adresse.Adresse = orderMagento.BillingAddress.Street[0].Substring(0, 35);
            }
            else
            {
                adress.Adresse.Adresse = orderMagento.BillingAddress.Street[0];
            }

            
            
                
            adress.Telecom.EMail = orderMagento.BillingAddress.Email.ToString();
            adress.Telecom.Telephone = orderMagento.BillingAddress.Telephone.ToString();
            adress.Adresse.CodePostal = orderMagento.BillingAddress.Postcode.ToString();
            adress.Adresse.Ville = orderMagento.BillingAddress.City.ToString();
            if (HasMethod(orderMagento.BillingAddress, "Region"))
            {
                adress.Adresse.CodeRegion = orderMagento.BillingAddress.Region.ToString();
            }
            adress.Adresse.Pays = ISO3166.FromAlpha2(orderMagento.BillingAddress.CountryId.ToString()).Name;
            adress.Expedition = SingletonConnection.Instance.Gescom.FactoryExpedition.ReadIntitule(UtilsConfig.OrderCarrierMapping[expedition]);
            try
            {
                adress.Write();
            }
            catch (Exception ex) {

            }
            
            File.AppendAllText("Log\\test.txt", DateTime.Now + " end create billing adress : " + intitule + Environment.NewLine);
        }

        public static void createBtoCAdress(OrderSearchItem orderMagento, IBOClient3 customer, string AdressType, string expedition)
        {
            int AdressNumber = 0;
            SqlDataReader DbAdressNumber = SingletonConnection.Instance.dB.Select("select count(*) from [" + System.Configuration.ConfigurationManager.AppSettings["DBNAME"].ToString() + "].[dbo].[F_LIVRAISON] where LI_EMail like '" + orderMagento.BillingAddress.Email.ToString() + "'");
            while (DbAdressNumber.Read())
            {
                AdressNumber = Int32.Parse(DbAdressNumber.GetValue(0).ToString());
            }
            DbAdressNumber.Close();
            // requete SQL
            if (AdressNumber == 0)
            {
                AdressNumber = 1;
            }
            else
            {
                AdressNumber++;
            }
            IBOClientLivraison3 adress = (IBOClientLivraison3)customer.FactoryClientLivraison.Create();
            adress.SetDefault();
            string intitule = "";
            if (AdressNumber > 9)
            {
                intitule = orderMagento.BillingAddress.Lastname + " " + orderMagento.BillingAddress.Firstname.ToUpper() + " " + AdressNumber.ToString();
            }
            else
            {
                intitule = orderMagento.BillingAddress.Lastname + " " + orderMagento.BillingAddress.Firstname.ToUpper() + " 0" + AdressNumber.ToString();
            }
            adress.LI_Intitule = intitule;
            adress.LI_Contact = AdressType;


            if (orderMagento.BillingAddress.Street[0].Length > 35)
            {
                adress.Adresse.Adresse = orderMagento.BillingAddress.Street[0].Substring(0, 35);
            }
            else
            {
                adress.Adresse.Adresse = orderMagento.BillingAddress.Street[0];
            }

            
            
            adress.Telecom.EMail = orderMagento.BillingAddress.Email.ToString();
            adress.Telecom.Telephone = orderMagento.BillingAddress.Telephone.ToString();
            adress.Adresse.CodePostal = orderMagento.BillingAddress.Postcode.ToString();
            adress.Adresse.Ville = orderMagento.BillingAddress.City.ToString();
            if (HasMethod(orderMagento.BillingAddress, "Region"))
            {
                adress.Adresse.CodeRegion = orderMagento.BillingAddress.Region.ToString();
            }
            adress.Adresse.Pays = ISO3166.FromAlpha2(orderMagento.BillingAddress.CountryId.ToString()).Name;
            adress.Expedition = SingletonConnection.Instance.Gescom.FactoryExpedition.ReadIntitule(UtilsConfig.OrderCarrierMapping[expedition]);
            try {
                adress.Write();
            }
            catch(Exception ex)
            {

            }
            

        }

        public static Boolean ExistAdress(string adress, string email)
        {
            string sql = "SELECT [LI_No] FROM [" + System.Configuration.ConfigurationManager.AppSettings["DBNAME"].ToString() + "].[dbo].[F_LIVRAISON] where CT_Num = '" + UtilsConfig.CtNumClient + "' and LI_Adresse ='" + adress.Replace("'", "''") + "' and LI_EMail = '" + email + "'";
            File.AppendAllText("Log\\test.txt", DateTime.Now + " requete SQL exist adress: " + sql + Environment.NewLine);
            SqlDataReader AddressLiNo = SingletonConnection.Instance.dB.Select(sql);
            while (AddressLiNo.Read())
            {
                return true;
            }
            return false;
        }
        public static string GetIntitulAdress(string adress, string email)
        {
            string sql = "SELECT [LI_Intitule] FROM [" + System.Configuration.ConfigurationManager.AppSettings["DBNAME"].ToString() + "].[dbo].[F_LIVRAISON] where CT_Num = '"+UtilsConfig.CtNumClient+"' and LI_Adresse ='" + adress.Replace("'", "''") + "' and LI_EMail = '" + email + "'";
            File.AppendAllText("Log\\test.txt", DateTime.Now + " requete SQL get intitule : " + sql + Environment.NewLine);
            SqlDataReader AddressLiNo = SingletonConnection.Instance.dB.Select(sql);
            while (AddressLiNo.Read())
            {
                return AddressLiNo.GetValue(0).ToString();
            }
            return "";
        }
    }
    
}
