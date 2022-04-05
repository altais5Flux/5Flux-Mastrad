using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebservicesSage.Singleton;
using Objets100cLib;
using WebservicesSage.Utils;
using Newtonsoft.Json.Linq;
using WebservicesSage.Utils.Enums;

namespace WebservicesSage.Cotnroller
{
    public static class ControllerConfiguration
    {

        public static void SaveConfiguration()
        {
           
            UtilsConfig.CompteG = SingletonUI.Instance.ComptGConf.selectedValue;
            UtilsConfig.CatTarif = SingletonUI.Instance.CatTarifConf.selectedValue;
            UtilsConfig.CondLivraison = SingletonUI.Instance.CondLivraisonConf.selectedValue;
            UtilsConfig.Expedition = SingletonUI.Instance.ExpeditionConf.selectedValue;
            UtilsConfig.PrefixClient = SingletonUI.Instance.PrefixClientConf.Text;

            //UtilsConfig.ArrondiDigits = Convert.ToInt32(SingletonUI.Instance.ArticleConfigurationArrondiInput.Text);
        }


        public static void LoadConfiguration()
        {
            // compteG
            var compta = SingletonConnection.Instance.Compta;
            var gescom = SingletonConnection.Instance.Gescom;

            LoadArticleConfiguration();
            LoadOrderConfiguration();
            LoadClientConfiguration();
            LoadGeneralConfiguration();
            LoadMagentoConfiguration();
            #region CONFIGURATION CLIENT


            /* #region CONFIGURATION COMPTE GENERAUX
             string[] items = new string[compta.FactoryCompteG.List.Count];
             int count = 0;
             foreach (IBOCompteG3 t in compta.FactoryCompteG.List)
             {
                 items[count] = t.CG_Num + " || " + t.CG_Intitule;
                 count++;
             }
             SingletonUI.Instance.ComptGConf.Items = items;
             SingletonUI.Instance.ComptGConf.Refresh();

             // selectionne la value de la conf si existe

             if (!String.IsNullOrEmpty(UtilsConfig.CompteG))
             {
                 Console.WriteLine(UtilsConfig.CompteGnum);
                 int index = Array.IndexOf(SingletonUI.Instance.ComptGConf.Items, UtilsConfig.CompteG);
                 SingletonUI.Instance.ComptGConf.selectedIndex = index;
             }
             #endregion

             #region CONFIGURATION CAT TARRIFAIRE
             items = new string[gescom.FactoryCategorieTarif.List.Count];
             count = 0;
             foreach (IBPCategorieTarif t in gescom.FactoryCategorieTarif.List)
             {
                 items[count] = t.CT_Intitule;
                 count++;
             }

             SingletonUI.Instance.CatTarifConf.Items = items;
             SingletonUI.Instance.CatTarifConf.Refresh();

             // selectionne la value de la conf si existe

             if (!String.IsNullOrEmpty(UtilsConfig.CatTarif))
             {
                 int index = Array.IndexOf(SingletonUI.Instance.CatTarifConf.Items, UtilsConfig.CatTarif);
                 SingletonUI.Instance.CatTarifConf.selectedIndex = index;
             }
             #endregion

             #region CONFIGURATION CONDITION DE LIVRAISON

             items = new string[gescom.FactoryConditionLivraison.List.Count];
             count = 0;
             foreach (IBPConditionLivraison t in gescom.FactoryConditionLivraison.List)
             {
                 items[count] = t.C_Intitule;
                 count++;
             }

             SingletonUI.Instance.CondLivraisonConf.Items = items;
             SingletonUI.Instance.CondLivraisonConf.Refresh();

             // selectionne la value de la conf si existe

             if (!String.IsNullOrEmpty(UtilsConfig.CondLivraison))
             {
                 int index = Array.IndexOf(SingletonUI.Instance.CondLivraisonConf.Items, UtilsConfig.CondLivraison);
                 SingletonUI.Instance.CondLivraisonConf.selectedIndex = index;
             }
             #endregion

             #region CONFIGURATION MODE EXPEDITION
             items = new string[gescom.FactoryExpedition.List.Count];
             count = 0;
             foreach (IBPExpedition3 t in gescom.FactoryExpedition.List)
             {
                 items[count] = t.E_Intitule;
                 count++;
             }

             SingletonUI.Instance.ExpeditionConf.Items = items;
             SingletonUI.Instance.ExpeditionConf.Refresh();

             // selectionne la value de la conf si existe

             if (!String.IsNullOrEmpty(UtilsConfig.Expedition))
             {
                 int index = Array.IndexOf(SingletonUI.Instance.ExpeditionConf.Items, UtilsConfig.Expedition);
                 SingletonUI.Instance.ExpeditionConf.selectedIndex = index;
             }
             #endregion

             #region CONFIGURATION PREFIX CLIENT
             if (!String.IsNullOrEmpty(UtilsConfig.PrefixClient))
             {
                 SingletonUI.Instance.PrefixClientConf.Text = UtilsConfig.PrefixClient;
             }
             #endregion*/



            #endregion



        }
        public static void LoadMappingInfosLibre()
        {
            var infolibreField = Singleton.SingletonConnection.Instance.Gescom.FactoryArticle.InfoLibreFields;
            List<string> items = new List<string>();
            string defaultValue, defaultValueFeature;
            UtilsConfig.InfoLibre.TryGetValue("default", out defaultValue);
            string[] data = defaultValue.Split('_');

            for (int i = 1; i < infolibreField.Count + 1; i++)
            {
                items.Add(infolibreField[i].Name);
            }

            SingletonUI.Instance.SageInfos1.Items = items.ToArray();
            SingletonUI.Instance.SageInfos1.selectedIndex = Int32.Parse(data[0]) - 1;
            SingletonUI.Instance.SageInfos2.Items = items.ToArray();
            SingletonUI.Instance.SageInfos2.selectedIndex = Int32.Parse(data[1]) - 1;
            SingletonUI.Instance.SageInfos3.Items = items.ToArray();
            SingletonUI.Instance.SageInfos3.selectedIndex = Int32.Parse(data[2]) - 1;
            SingletonUI.Instance.SageInfos4.Items = items.ToArray();
            SingletonUI.Instance.SageInfos4.selectedIndex = Int32.Parse(data[3]) - 1;
            if (data.Count() > 8)
            {
                SingletonUI.Instance.SageInfos5.Items = items.ToArray();
                SingletonUI.Instance.SageInfos5.selectedIndex = Int32.Parse(data[4]) - 1;
            }
            else
            {
                SingletonUI.Instance.SageInfos5.Items = items.ToArray();
                SingletonUI.Instance.SageInfos5.selectedIndex = -1;
            }
            UtilsConfig.InfoLibreValue.TryGetValue("default", out defaultValueFeature);
            string[] dataFeature = defaultValueFeature.Split('_');
            SingletonUI.Instance.MagentoInfos1.Text = dataFeature[0];
            SingletonUI.Instance.MagentoInfos2.Text = dataFeature[1];
            SingletonUI.Instance.MagentoInfos3.Text = dataFeature[2];
            SingletonUI.Instance.MagentoInfos4.Text = dataFeature[3];
            if (dataFeature.Count() > 4)
            {
                SingletonUI.Instance.MagentoInfos5.Text = dataFeature[4];
            }
            else
            {
                SingletonUI.Instance.MagentoInfos5.Text = "";
            }


        }
        public static void LoadAllOrderStatutMappingConfiguration()
        {
            //var configFile = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
            //System.Configuration.ConfigurationManager.RefreshSection("OrderSetting/MagentoStatutId");
            //System.Configuration.ConfigurationManager.RefreshSection("OrderSetting/MagentoStatutId");
            var gescom = SingletonConnection.Instance.Gescom;
            List<string> items = new List<string>();
            string data = "";
            int res1, res2, res3;
            for (int i = 1; i < UtilsConfig.MagentoStatutId.Count; i++)
            {
                UtilsConfig.MagentoStatutId.TryGetValue(i.ToString(), out data);
                items.Add(data);
            }
            //DocumentType.DocumentTypeVenteReprise.;
            UtilsConfig.MagentoStatutId.TryGetValue("default", out data);
            SingletonUI.Instance.PrestaId1.Items = items.ToArray();
            //SingletonUI.Instance.PrestaId2.Items = items.ToArray();
            //SingletonUI.Instance.PrestaId3.Items = items.ToArray();
            string[] selected = data.Split('_');
            if (Int32.TryParse(selected[0], out res1))
            {
                SingletonUI.Instance.PrestaId1.selectedIndex = res1 - 1;
            }
            else
            {
                SingletonUI.Instance.PrestaId1.selectedIndex = -1;
            }
            SingletonUI.Instance.PrestaId1.Refresh();
            /*if (Int32.TryParse(selected[1], out res2))
            {
                SingletonUI.Instance.PrestaId2.selectedIndex = res2 - 1;
            }
            else
            {
                SingletonUI.Instance.PrestaId2.selectedIndex = -1;
            }
            if (Int32.TryParse(selected[2], out res3))
            {
                SingletonUI.Instance.PrestaId3.selectedIndex = res3 - 1;
            }
            else
            {
                SingletonUI.Instance.PrestaId3.selectedIndex = -1;
            }
            */
            // Get Status Commande
            List<string> Orderitems = new List<string>();
            string Orderdata = "";
            int Orderres1, Orderres2, Orderres3;
            for (int i = 1; i < UtilsConfig.OrderMapping.Count; i++)
            {
                UtilsConfig.OrderMapping.TryGetValue(i.ToString(), out Orderdata);
                string[] statut = Orderdata.Split('_');
                Orderitems.Add(statut[1]);
            }
            //DocumentType.DocumentTypeVenteReprise.;
            UtilsConfig.OrderMapping.TryGetValue("default", out Orderdata);
            SingletonUI.Instance.SageDoc1.Items = Orderitems.ToArray();
            //SingletonUI.Instance.SageDoc2.Items = Orderitems.ToArray();
            //SingletonUI.Instance.SageDoc3.Items = Orderitems.ToArray();
            string[] selectedData = Orderdata.Split('_');
            if (Int32.TryParse(selectedData[0], out Orderres1))
            {
                SingletonUI.Instance.SageDoc1.selectedIndex = Orderres1 - 1;
            }
            else
            {
                SingletonUI.Instance.SageDoc1.selectedIndex = -1;
            }
            SingletonUI.Instance.SageDoc1.Refresh();
            /*if (Int32.TryParse(selectedData[1], out Orderres2))
            {
                SingletonUI.Instance.SageDoc2.selectedIndex = Orderres2 - 1;
            }
            else
            {
                SingletonUI.Instance.SageDoc2.selectedIndex = -1;
            }
            if (Int32.TryParse(selectedData[2], out Orderres3))
            {
                SingletonUI.Instance.SageDoc3.selectedIndex = Orderres3 - 1;
            }
            else
            {
                SingletonUI.Instance.SageDoc3.selectedIndex = -1;
            }
            */

        }
        public static void LoadMagentoConfiguration()
        {
            SingletonUI.Instance.MagentoToken.Text = UtilsConfig.Token.ToString();
            SingletonUI.Instance.MagentoDefaultStore.Text = UtilsConfig.Store.ToString();
            SingletonUI.Instance.MagentoDefaultCategory.Text = UtilsConfig.Category.ToString();
            SingletonUI.Instance.DefaultRemise.Text = UtilsConfig.DefaultRemiseReference.ToString();
            SingletonUI.Instance.DefaultTransportReference.Text = UtilsConfig.DefaultTransportReference.ToString();
        }

        public static void LoadOrderConfiguration()
        {
            var compta = SingletonConnection.Instance.Compta;
            var gescom = SingletonConnection.Instance.Gescom;

            List<string> items = new List<string>();
            int count, selectedindex;

            //items = new string[];
            count = 0;
            selectedindex = 0;
            foreach (IBISouche souche in gescom.FactorySoucheVente.List)
            {
                if (!String.IsNullOrEmpty(souche.Intitule))
                {
                    items.Add(souche.Intitule);
                    if (souche.Intitule.Equals(UtilsConfig.Souche))
                    {
                        selectedindex = count;
                    }
                    count++;
                }
            }
            SingletonUI.Instance.SoucheDropdown.Items = items.ToArray();
            SingletonUI.Instance.SoucheDropdown.selectedIndex = selectedindex;
            SingletonUI.Instance.SoucheDropdown.Refresh();
        }

        public static void LoadArticleConfiguration()
        {/* OLD VERSION
            var compta = SingletonConnection.Instance.Compta;
            var gescom = SingletonConnection.Instance.Gescom;
            string[] items;
            int count;

            #region CONFIGURATION ARTICLE

            #region CONIGURATION DES DEPOTS

            items = new string[gescom.FactoryDepot.List.Count];
            count = 0;
            foreach (IBODepot3 depot in gescom.FactoryDepot.List)
            {
                items[count] = depot.DE_Intitule;
                count++;
            }
            SingletonUI.Instance.CatTarifConf.Items = items;
            SingletonUI.Instance.CatTarifConf.Refresh();

            #endregion


            #endregion*/

            List<string> items = new List<string>();
            string data = "";
            int res1, res2;
            for (int i = 1; i < UtilsConfig.MultiLangue.Count; i++)
            {
                UtilsConfig.MultiLangue.TryGetValue(i.ToString(), out data);
                items.Add(data);
            }
            //DocumentType.DocumentTypeVenteReprise.;
            UtilsConfig.MultiLangue.TryGetValue("default", out data);
            SingletonUI.Instance.Lang1.Items = items.ToArray();
            SingletonUI.Instance.Lang2.Items = items.ToArray();
            string[] selected = data.Split('_');
            if (Int32.TryParse(selected[0], out res1))
            {
                SingletonUI.Instance.Lang1.selectedIndex = res1 - 1;
            }
            else
            {
                SingletonUI.Instance.Lang1.selectedIndex = -1;
            }
            if (Int32.TryParse(selected[1], out res2))
            {
                SingletonUI.Instance.Lang2.selectedIndex = res2 - 1;
            }
            else
            {
                SingletonUI.Instance.Lang2.selectedIndex = -1;
            }
            SingletonUI.Instance.Store1.Text = selected[2];
            SingletonUI.Instance.Store2.Text = selected[3];
            SingletonUI.Instance.ArticleConfigurationArrondiInput.Text = UtilsConfig.ArrondiDigits.ToString();
            SingletonUI.Instance.ArticleConfigurationTVAInput.Text = UtilsConfig.TVA.ToString();
            if (UtilsConfig.DefaultStock.Equals("TRUE"))
            {
                SingletonUI.Instance.DefaultStock.Value = true;
            }
            else
            {
                SingletonUI.Instance.DefaultStock.Value = false;
            }
            if (UtilsConfig.LocalDB.Equals("TRUE"))
            {
                SingletonUI.Instance.LocalDB.Value = true;
            }
            else
            {
                SingletonUI.Instance.LocalDB.Value = false;
            }
        }
        public static void  UpdateArticleConfiguration()
        {
            int ArrondiDigits, TVA;
            if (int.TryParse(SingletonUI.Instance.ArticleConfigurationArrondiInput.Text, out ArrondiDigits))
            {
                UtilsConfig.ArrondiDigits = ArrondiDigits;
            }
            if (int.TryParse(SingletonUI.Instance.ArticleConfigurationTVAInput.Text, out TVA))
            {
                UtilsConfig.TVA = TVA;
            }
            if (SingletonUI.Instance.DefaultStock.Value)
            {
                UtilsConfig.DefaultStock = "TRUE";
            }
            else
            {
                UtilsConfig.DefaultStock = "FALSE";
            }
            if (SingletonUI.Instance.LocalDB.Value)
            {
                UtilsConfig.LocalDB = "TRUE";
            }
            else
            {
                UtilsConfig.LocalDB = "FALSE";
            }
            string SavedLang, valueLang;
            valueLang = (SingletonUI.Instance.Lang1.selectedIndex + 1) + "_" + (SingletonUI.Instance.Lang2.selectedIndex + 1) + "_" + SingletonUI.Instance.Store1.Text + "_" + SingletonUI.Instance.Store2.Text;
            if (Utils.UtilsConfig.MultiLangue.TryGetValue("default", out SavedLang))
            {
                if (!valueLang.Equals(SavedLang))
                {
                    Utils.UtilsConfig.UpdateNodeInCustomSection("MultiLangue", "default", valueLang);
                }
            }
            SingletonUI.Instance.ShowNotification("Enregistrement effectuer avec Succée");
        }
        public static void UpdateOrderConfiguration()
        {
            UtilsConfig.Souche = SingletonUI.Instance.SoucheDropdown.selectedValue.ToString();
            SingletonUI.Instance.ShowNotification("Enregistrement effectuer avec Succée");
        }
        public static void UpdateClientConfiguration()
        {
            UtilsConfig.PrefixClient = SingletonUI.Instance.PrefixClient.Text;
            UtilsConfig.ContactConfig = SingletonUI.Instance.AddContactConfig.SelectedIndex + 1;
            SingletonUI.Instance.ShowNotification("Enregistrement effectuer avec Succée");
        }
        public static void LoadClientConfiguration()
        {
            SingletonUI.Instance.AddContactConfig.SetItemChecked(UtilsConfig.ContactConfig - 1, true);
            SingletonUI.Instance.PrefixClient.Text = UtilsConfig.PrefixClient;
        }
        public static void LoadGeneralConfiguration()
        {
            SingletonUI.Instance.BaseURLConfiguration.Text = UtilsConfig.BaseUrl.ToString();
            SingletonUI.Instance.UserConfiguration.Text = UtilsConfig.User.ToString();
            SingletonUI.Instance.Gcm_User.Text = UtilsConfig.Gcm_User.ToString();
            SingletonUI.Instance.Mae_User.Text = UtilsConfig.Mae_User.ToString();
            SingletonUI.Instance.Gcm_Pass.Text = UtilsConfig.Gcm_Pass.ToString();
            SingletonUI.Instance.Gcm_Path.Text = UtilsConfig.Gcm_Path.ToString();
            SingletonUI.Instance.Mae_Pass.Text = UtilsConfig.Mae_Pass.ToString();
            SingletonUI.Instance.Mae_Path.Text = UtilsConfig.Mae_Path.ToString();
            SingletonUI.Instance.CronTaskCheckNewOrder.Text = TimeSpan.FromMilliseconds(UtilsConfig.CronTaskCheckForNewOrder).TotalMinutes.ToString("0.00");
            SingletonUI.Instance.CronTaskUpdateStatut.Text = TimeSpan.FromMilliseconds(UtilsConfig.CronTaskUpdateStatut).TotalMinutes.ToString("0.00");
            if (UtilsConfig.Mae_Set.ToString().Equals("TRUE"))
            {
                SingletonUI.Instance.MAE_set.Value = true;
            }
            else
            {
                SingletonUI.Instance.MAE_set.Value = false;
            }

            if (UtilsConfig.Gcm_Set.ToString().Equals("TRUE"))
            {
                SingletonUI.Instance.GCM_set.Value = true;
            }
            else
            {
                SingletonUI.Instance.GCM_set.Value = false;
            }


        }
        public static void UpdateGeneralConfiguration()
        {
            int result;
            Boolean error = false;
            String CronTaskCheckNewOrder = SingletonUI.Instance.CronTaskCheckNewOrder.Text.ToString(); ;
            String CronTaskUpdateStatut = SingletonUI.Instance.CronTaskUpdateStatut.Text.ToString(); ;
            UtilsConfig.BaseUrl = SingletonUI.Instance.BaseURLConfiguration.Text;
            UtilsConfig.User = SingletonUI.Instance.UserConfiguration.Text;
            UtilsConfig.Mae_Path = SingletonUI.Instance.Mae_Path.Text;
            UtilsConfig.Mae_User = SingletonUI.Instance.Mae_User.Text;
            UtilsConfig.Mae_Pass = SingletonUI.Instance.Mae_Pass.Text;
            UtilsConfig.Mae_Set = SingletonUI.Instance.MAE_set.Value.ToString().ToUpper();
            UtilsConfig.Gcm_Path = SingletonUI.Instance.Gcm_Path.Text;
            UtilsConfig.Gcm_User = SingletonUI.Instance.Gcm_User.Text;
            UtilsConfig.Gcm_Pass = SingletonUI.Instance.Gcm_Pass.Text;
            UtilsConfig.Gcm_Set = SingletonUI.Instance.GCM_set.Value.ToString().ToUpper();
            //Magento Configuration
            UtilsConfig.Token = SingletonUI.Instance.MagentoToken.Text.ToString();
            UtilsConfig.DefaultRemiseReference = SingletonUI.Instance.DefaultRemise.Text.ToString();
            UtilsConfig.Store = SingletonUI.Instance.MagentoDefaultStore.Text.ToString();
            UtilsConfig.Category = SingletonUI.Instance.MagentoDefaultCategory.Text.ToString() ;
            UtilsConfig.DefaultTransportReference = SingletonUI.Instance.DefaultTransportReference.Text.ToString();
            if (SingletonUI.Instance.CronTaskCheckNewOrder.Text.ToString().IndexOf(",")>0)
            {
                CronTaskCheckNewOrder = SingletonUI.Instance.CronTaskCheckNewOrder.Text.ToString().Substring(0, SingletonUI.Instance.CronTaskCheckNewOrder.Text.ToString().IndexOf(","));
            }else
            if (SingletonUI.Instance.CronTaskCheckNewOrder.Text.ToString().IndexOf(".") > 0)
            {
                CronTaskCheckNewOrder = SingletonUI.Instance.CronTaskCheckNewOrder.Text.ToString().Substring(0, SingletonUI.Instance.CronTaskCheckNewOrder.Text.ToString().IndexOf("."));
            }

            if (Int32.TryParse(CronTaskCheckNewOrder.ToString(), out result)&& result>0)
            {
                UtilsConfig.CronTaskCheckForNewOrder = Convert.ToInt32(TimeSpan.FromMinutes(result).TotalMilliseconds);
                SingletonUI.Instance.CronTaskCheckNewOrder.BackColor = System.Drawing.Color.White;
            }
            else
            {
                SingletonUI.Instance.ShowErrorNotification("Merci de saisie une valeur supérieur à 1");
                SingletonUI.Instance.CronTaskCheckNewOrder.BackColor = System.Drawing.Color.Red;
                error = true;
            }

            if (SingletonUI.Instance.CronTaskUpdateStatut.Text.ToString().IndexOf(",") > 0)
            {
                CronTaskUpdateStatut = SingletonUI.Instance.CronTaskUpdateStatut.Text.ToString().Substring(0, SingletonUI.Instance.CronTaskUpdateStatut.Text.ToString().IndexOf(","));
            }
            else
            if (SingletonUI.Instance.CronTaskUpdateStatut.Text.ToString().IndexOf(".") > 0)
            {
                CronTaskUpdateStatut = SingletonUI.Instance.CronTaskUpdateStatut.Text.ToString().Substring(0, SingletonUI.Instance.CronTaskUpdateStatut.Text.ToString().IndexOf("."));
            }

            if (Int32.TryParse(CronTaskUpdateStatut.ToString(), out result) && result >0)
            {
                UtilsConfig.CronTaskUpdateStatut = Convert.ToInt32(TimeSpan.FromMinutes(result).TotalMilliseconds);
                SingletonUI.Instance.CronTaskUpdateStatut.BackColor = System.Drawing.Color.White;
            }
            else
            {
                SingletonUI.Instance.ShowErrorNotification("Merci de saisie une valeur supérieur à 1");
                SingletonUI.Instance.CronTaskUpdateStatut.BackColor = System.Drawing.Color.Red;
                error = true;
            }
            if (!error)
            {
                SingletonUI.Instance.ShowNotification("Enregistrement effectuer avec Succée");
            }
        }
        /********************************* Test import from presta version *********************************/
        public static void loadDefaultOrderMappingConfiguration()
        {
            var gescom = SingletonConnection.Instance.Gescom;
            List<string> items = new List<string>();
            int count, defaultindex, indexT1, indexT2, indexT3 ;
            String defaultExpedition = "";
            String T1, T2,T3;
            Dictionary<string, string> orderCarrier = UtilsConfig.OrderCarrierMapping;
            orderCarrier.TryGetValue("Default", out defaultExpedition);
            orderCarrier.TryGetValue("1", out T1);
            orderCarrier.TryGetValue("2", out T2);
            orderCarrier.TryGetValue("3", out T3);
            count = 0;
            defaultindex = 0;
            indexT1 = indexT2 = indexT3 = 0;
            foreach (IBPExpedition3 expedition in gescom.FactoryExpedition.List)
            {

                if (!String.IsNullOrEmpty(expedition.E_Intitule))
                {
                    items.Add(expedition.E_Intitule);
                    if (expedition.E_Intitule.Equals(defaultExpedition))
                    {

                        defaultindex = count;
                    }
                    if (expedition.E_Intitule.Equals(T1))
                    {

                        indexT1 = count;
                    }
                    if (expedition.E_Intitule.Equals(T2))
                    {

                        indexT2 = count;
                    }
                    if (expedition.E_Intitule.Equals(T3))
                    {

                        indexT3 = count;
                    }
                    count++;
                }
            }
            SingletonUI.Instance.DefaultExpedition.Items = items.ToArray();
            SingletonUI.Instance.DefaultExpedition.selectedIndex = defaultindex;
            SingletonUI.Instance.ExpeditionSage1.Items = items.ToArray();
            SingletonUI.Instance.ExpeditionSage1.selectedIndex = indexT1;
            SingletonUI.Instance.ExpeditionSage2.Items = items.ToArray();
            SingletonUI.Instance.ExpeditionSage2.selectedIndex = indexT2;
            SingletonUI.Instance.ExpeditionSage3.Items = items.ToArray();
            SingletonUI.Instance.ExpeditionSage3.selectedIndex = indexT3;
        }

        public static void LoadAllOrderMappingConfiguration()
        {

            /*
            string response = UtilsWebservices.SendDataNoParse(UtilsConfig.BaseUrl + EnumEndPoint.Transporteur.Value, "all");
            JToken ExpeditionPrestashops = JToken.Parse(response);
            string test;
            int LabelY = 16;
            int count, selectedindex;


            foreach (JObject ExpeditionPrestashop in ExpeditionPrestashops)
            {
                count = 0;
                selectedindex = 0;
                string SelectedOrderCarrier;
                System.Drawing.Point point = new System.Drawing.Point();
                LabelY += 40;
                point.X = 36;
                point.Y = LabelY;
                test = ExpeditionPrestashop["name"].ToString();
                System.Windows.Forms.Label label = new System.Windows.Forms.Label();
                label.Location = point;
                label.Text = test;
                SingletonUI.Instance.ExpeditionOrderDataGrid.Controls.Add(label);
                var gescom = SingletonConnection.Instance.Gescom;
                List<string> items = new List<string>();
                Bunifu.Framework.UI.BunifuDropdown dropdown = new Bunifu.Framework.UI.BunifuDropdown();
                foreach (IBPExpedition3 expedition in gescom.FactoryExpedition.List)
                {
                    if (!String.IsNullOrEmpty(expedition.E_Intitule))
                    {
                        if (UtilsConfig.OrderCarrierMapping.TryGetValue(ExpeditionPrestashop["id_carrier"].ToString(), out SelectedOrderCarrier))
                        {
                            if (SelectedOrderCarrier.Equals(expedition.E_Intitule))
                            {
                                selectedindex = count;
                            }
                        }
                        items.Add(expedition.E_Intitule);
                        count++;
                    }
                }
                dropdown.Items = items.ToArray();
                dropdown.selectedIndex = selectedindex;
                System.Drawing.Point point2 = new System.Drawing.Point();
                point2.X = 400;
                point2.Y = LabelY;
                dropdown.Location = point2;
                SingletonUI.Instance.ExpeditionOrderDataGrid.Controls.Add(dropdown);
                SingletonUI.Instance.Transporteur.Add(ExpeditionPrestashop["name"].ToString(), ExpeditionPrestashop["id_carrier"].ToString());

            }*/
        }
        /********************************* Test import from presta version *********************************/
    }

}
