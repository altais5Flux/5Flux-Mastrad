using Objets100cLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using WebservicesSage.Singleton;
using WebservicesSage.Utils;
using Newtonsoft.Json.Linq;
using WebservicesSage.Object.CatTarSearch;
using Newtonsoft.Json;
using System.IO;

namespace WebservicesSage.Object
{
    class Customer
    {
        public long GroupId { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public long Gender { get; set; }
        public long StoreId { get; set; }
        public long WebsiteId { get; set; }
        public List<Address> Addresses { get; set; }
        public List<CustomAttribute> CustomAttributes { get; set; }
        public partial class CustomAttribute
        {
            public string attribute_code { get; set; }
            public string value { get; set; }
        }
        public partial class Address
        {
            /*public Region Region { get; set; }
            public long RegionId { get; set; }*/
            public string CountryId { get; set; }
            public List<string> Street { get; set; }
            public string Telephone { get; set; }
            public string Postcode { get; set; }
            public string City { get; set; }
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public bool DefaultShipping { get; set; }
            public bool DefaultBilling { get; set; }
            
        }

        public partial class Region
        {
            public object RegionCode { get; set; }
            public object RegionRegion { get; set; }
            public long RegionId { get; set; }
        }

        public Customer()
        {

        }
        public object UpdateCustomer(string ct_Num,string clienttype,CustomerSearch.CustomerSearch customerSearch)
        {
            var compta = SingletonConnection.Instance.Gescom.CptaApplication;
            var clientsSageObj = compta.FactoryClient.ReadNumero(ct_Num);

            Client client = new Client(clientsSageObj);
            CustomAttribute custom_attribute = new CustomAttribute();
            CustomAttribute custom_attribute2 = new CustomAttribute();
            CustomAttributes = new List<CustomAttribute>();
            custom_attribute.attribute_code = "sage_number";
            custom_attribute.value = client.CT_NUM.ToString();
            if (String.IsNullOrEmpty(clienttype))
            {
                IBIFields infolibreField = Singleton.SingletonConnection.Instance.Compta.FactoryTiers.InfoLibreFields;
                List<InfoLibre> infoLibre = new List<InfoLibre>();
                int compteur = 1;
                //CustomAttribute custom_attribute2 = new CustomAttribute();
                //custom_attribute2.attribute_code = "customer_type";
                foreach (var infoLibreValue in clientsSageObj.InfoLibre)
                {
                    if (infolibreField[compteur].Name.Equals("Type_client"))
                    {
                        if (infoLibreValue.ToString().Equals("PRIMAIRE/MATERNELLE"))
                        {
                            custom_attribute2.value = "1";
                        }
                        if (infoLibreValue.ToString().Equals("COLLEGE"))
                        {
                            custom_attribute2.value = "2";
                        }
                        if (infoLibreValue.ToString().Equals("LYCEE"))
                        {
                            custom_attribute2.value = "3";
                        }
                        if (infoLibreValue.ToString().Equals("POST-BAC"))
                        {
                            custom_attribute2.value = "4";
                        }
                        if (infoLibreValue.ToString().Equals("GROUPE SCOLAIRE"))
                        {
                            custom_attribute2.value = "5";
                        }
                        if (infoLibreValue.ToString().Equals("INSTITUT SPECIALISE"))
                        {
                            custom_attribute2.value = "6";
                        }
                        if (infoLibreValue.ToString().Equals("CENTRE DE FORMATION"))
                        {
                            custom_attribute2.value = "7";
                        }
                        if (infoLibreValue.ToString().Equals("SOCIETE"))
                        {
                            custom_attribute2.value = "8";
                        }
                        if (infoLibreValue.ToString().Equals("PARTICULIER"))
                        {
                            custom_attribute2.value = "9";
                        }
                        if (infoLibreValue.ToString().Equals("ASSOCIATION"))
                        {
                            custom_attribute2.value = "10";
                        }
                        if (infoLibreValue.ToString().Equals("ADMINISTRATION"))
                        {
                            custom_attribute2.value = "11";
                        }
                        if (infoLibreValue.ToString().Equals("CENTRALE D'ACHATS"))
                        {
                            custom_attribute2.value = "12";
                        }
                    }
                    infoLibre.Add(new InfoLibre(infolibreField[compteur].Name, infoLibreValue.ToString()));
                    compteur++;
                }
                custom_attribute2.attribute_code = "customer_type";
                CustomAttributes.Add(custom_attribute2);
            }
            else
            {
                custom_attribute2.attribute_code = "customer_type";
                custom_attribute2.value = clienttype;
                CustomAttributes.Add(custom_attribute2);
            }
            CustomAttributes.Add(custom_attribute);
            string group_id = "";
            /*
            WebservicesSage.Object.CustomerSearch.CustomerSearch Customer = UtilsWebservices.GetClientCtNum(customerSearch.Id.ToString());
            var tarifsSearch = UtilsWebservices.GetMagentoData("rest/V1/customerGroups/search" + UtilsWebservices.SearchCategoriesCriteria("code", client.GroupeTarifaireIntitule, "eq"));
            File.AppendAllText("Log\\GetCustomer.txt", tarifsSearch.ToString() + Environment.NewLine);
            CatTarifSearch catTarifSearch = CatTarifSearch.FromJson(tarifsSearch);
            
            if (catTarifSearch.TotalCount > 0 )
            {
                if (.GroupId.ToString().Equals(catTarifSearch.Items[0].Id.ToString()))
                {
                    group_id = catTarifSearch.Items[0].Id.ToString();
                }
                else
                {
                    group_id = Customer.GroupId.ToString();
                }
            }
            else
            {
                */
            /*
            var groupeJson = new
            {
                group = new
                {
                    code = client.GroupeTarifaireIntitule.ToString(),
                    tax_class_id = 3
                }

            };
            var jsonClient = JsonConvert.SerializeObject(groupeJson);
            UtilsWebservices.SendDataJson(jsonClient, @"rest/V1/customerGroups/");
            var tarifsSearch1 = UtilsWebservices.GetMagentoData("rest/V1/customerGroups/search" + UtilsWebservices.SearchCategoriesCriteria("code", client.GroupeTarifaireIntitule, "eq"));
            CatTarifSearch catTarifSearch1 = CatTarifSearch.FromJson(tarifsSearch1);
        if (catTarifSearch1.TotalCount >0)
        {
            group_id = catTarifSearch1.Items[0].Id.ToString();
        }
        else
        {
            group_id = customerSearch.GroupId.ToString();
        }
            */
            //on ne modifie pas le groupe ID
            group_id = customerSearch.GroupId.ToString();
            //}
            var customerJson = new
            {
                customer = new
                {
                    group_id = group_id.ToString(),
                    email = customerSearch.Email.ToString(),
                    id = customerSearch.Id.ToString(),
                    firstname = customerSearch.Firstname,
                    lastname = customerSearch.Lastname,
                    website_id = Utils.UtilsConfig.Store.ToString(),
                    disable_auto_group_change = "0",
                    extension_attributes = new { is_subscribed = false },
                    custom_attributes = CustomAttributes
                }
                
            };
            File.AppendAllText("Log\\updateCustomer.txt", customerJson.ToString()+ " CT_num : "+ custom_attribute.value.ToString() +  Environment.NewLine);
            return customerJson;
        }
        public object NewCustomer(Client client, IBOClient3 clientsg, CustomerSearchByEmail.CustomerSearchByEmail customerSearchByEmail=null)
        {
            string group_id = "";
            if (customerSearchByEmail != null)
            {
                
                var tarifsSearch = UtilsWebservices.GetMagentoData("rest/V1/customerGroups/search" + UtilsWebservices.SearchCategoriesCriteria("code", client.GroupeTarifaireIntitule, "eq"));
                CatTarifSearch catTarifSearch = CatTarifSearch.FromJson(tarifsSearch);
                
                if (catTarifSearch.TotalCount > 0)
                {
                    if (customerSearchByEmail.Items[0].GroupId.ToString().Equals(catTarifSearch.Items[0].Id.ToString()))
                    {
                        group_id = catTarifSearch.Items[0].Id.ToString();
                    }
                    else
                    {
                        group_id = customerSearchByEmail.Items[0].GroupId.ToString();
                    }
                }
            }
            else
            {
                var tarifsSearch = UtilsWebservices.GetMagentoData("rest/V1/customerGroups/search" + UtilsWebservices.SearchCategoriesCriteria("code", client.GroupeTarifaireIntitule, "eq"));
                CatTarifSearch catTarifSearch = CatTarifSearch.FromJson(tarifsSearch);
                if (catTarifSearch.TotalCount > 0)
                {
                    group_id = catTarifSearch.Items[0].Id.ToString();
                }
                else
                {
                    var groupeJson = new
                    {
                        group = new
                        {
                            code = client.GroupeTarifaireIntitule.ToString(),
                            tax_class_id = 3
                        }

                    };
                    var jsonClient = JsonConvert.SerializeObject(groupeJson);
                    UtilsWebservices.SendDataJson(jsonClient, @"rest/V1/customerGroups/");
                    var tarifsSearch1 = UtilsWebservices.GetMagentoData("rest/V1/customerGroups/search" + UtilsWebservices.SearchCategoriesCriteria("code", client.GroupeTarifaireIntitule, "eq"));
                    CatTarifSearch catTarifSearch1 = CatTarifSearch.FromJson(tarifsSearch1);
                    group_id = catTarifSearch1.Items[0].Id.ToString();
                    //create customer groupe
                }
            }
            IBIFields infolibreField = Singleton.SingletonConnection.Instance.Compta.FactoryTiers.InfoLibreFields;
            List<InfoLibre> infoLibre = new List<InfoLibre>();
            int compteur = 1;
            CustomAttribute custom_attribute2 = new CustomAttribute();
            custom_attribute2.attribute_code = "customer_type";
            foreach (var infoLibreValue in clientsg.InfoLibre)
            {
                if (infolibreField[compteur].Name.Equals("Type_client"))
                {
                    if (infoLibreValue.ToString().Equals("PRIMAIRE/MATERNELLE"))
                    {
                        custom_attribute2.value = "1";
                    }
                    if (infoLibreValue.ToString().Equals("COLLEGE"))
                    {
                        custom_attribute2.value = "2";
                    }
                    if (infoLibreValue.ToString().Equals("LYCEE"))
                    {
                        custom_attribute2.value = "3";
                    }
                    if (infoLibreValue.ToString().Equals("POST-BAC"))
                    {
                        custom_attribute2.value = "4";
                    }
                    if (infoLibreValue.ToString().Equals("GROUPE SCOLAIRE"))
                    {
                        custom_attribute2.value = "5";
                    }
                    if (infoLibreValue.ToString().Equals("INSTITUT SPECIALISE"))
                    {
                        custom_attribute2.value = "6";
                    }
                    if (infoLibreValue.ToString().Equals("CENTRE DE FORMATION"))
                    {
                        custom_attribute2.value = "7";
                    }
                    if (infoLibreValue.ToString().Equals("SOCIETE"))
                    {
                        custom_attribute2.value = "8";
                    }
                    if (infoLibreValue.ToString().Equals("PARTICULIER"))
                    {
                        custom_attribute2.value = "9";
                    }
                    if (infoLibreValue.ToString().Equals("ASSOCIATION"))
                    {
                        custom_attribute2.value = "10";
                    }
                    if (infoLibreValue.ToString().Equals("ADMINISTRATION"))
                    {
                        custom_attribute2.value = "11";
                    }
                    if (infoLibreValue.ToString().Equals("CENTRALE D'ACHATS"))
                    {
                        custom_attribute2.value = "12";
                    }
                }
                infoLibre.Add(new InfoLibre(infolibreField[compteur].Name, infoLibreValue.ToString()));
                compteur++;
            }
            
            CustomAttributes = new List<CustomAttribute>();
            CustomAttribute custom_attribute = new CustomAttribute();
            custom_attribute.attribute_code = "sage_number";
            custom_attribute.value = client.CT_NUM.ToString();
            CustomAttributes.Add(custom_attribute);
            CustomAttributes.Add(custom_attribute2);
            Address address = new Address();
            Addresses = new List<Address>();
            if (clientsg.LivraisonPrincipal.LI_Contact.ToString().Length > 0)
            {
                string[] PrincAddress = clientsg.LivraisonPrincipal.LI_Contact.Split(null);
                if (PrincAddress.Length == 4)
                {
                    address.Firstname = PrincAddress[0];
                    address.Lastname = PrincAddress[2];
                }
                else if (PrincAddress.Length == 3)
                {
                    address.Firstname = PrincAddress[1];
                    address.Lastname = PrincAddress[2];
                }
                else if(PrincAddress.Length == 2)
                {
                    address.Firstname = PrincAddress[0];
                    address.Lastname = PrincAddress[1];
                }else
                {
                    address.Firstname = PrincAddress[0];
                    address.Lastname = PrincAddress[0];
                }
            }
            address.Telephone = clientsg.LivraisonPrincipal.Telecom.Telephone.ToString();
            address.Postcode = clientsg.LivraisonPrincipal.Adresse.CodePostal.ToString();
            address.City = clientsg.LivraisonPrincipal.Adresse.Ville.ToString();
            string pays = clientsg.LivraisonPrincipal.Adresse.Pays;
            var regions = CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(x => new RegionInfo(x.LCID));
            var englishRegion = regions.FirstOrDefault(region => region.EnglishName.ToLower().Contains(pays.ToLower()));
           /* var region = CultureInfo
                .GetCultures(CultureTypes.SpecificCultures)
                .Select(ci => new RegionInfo(ci.LCID))
                .FirstOrDefault(rg => rg.DisplayName == pays.ToLower().to);*/
            address.CountryId = englishRegion.TwoLetterISORegionName.ToString();
            address.DefaultBilling = true;
            address.DefaultShipping = true;
            string test = clientsg.LivraisonPrincipal.Adresse.Adresse.ToString();
            address.Street = new List<string>();
            address.Street.Add(test);
            if (clientsg.LivraisonPrincipal.Adresse.Complement.Length > 0)
            {
                address.Street.Add(clientsg.LivraisonPrincipal.Adresse.Complement);
            }

            Addresses.Add(address);
            if (client.clientLivraisonAdresses.Count >0)
            {
                foreach (ClientLivraisonAdress allAdress in client.clientLivraisonAdresses)
                {
                    if (!allAdress.Intitule.Equals(clientsg.LivraisonPrincipal.LI_Intitule) && allAdress.Telephone.Length > 0)
                    {
                        Address address1 = new Address();
                        if (allAdress.Intitule.ToString().Length > 0)
                        {
                            string[] PrincAddress = allAdress.Contact.Split(null);
                            if (PrincAddress.Length == 3)
                            {
                                address1.Firstname = PrincAddress[1];
                                address1.Lastname = PrincAddress[2];
                            }
                            else if(PrincAddress.Length == 2)
                            {
                                address1.Firstname = PrincAddress[0];
                                address1.Lastname = PrincAddress[1];
                            }
                            else
                            {
                                address1.Firstname = PrincAddress[0];
                                address1.Lastname = PrincAddress[0];
                            }
                        }
                        address1.Telephone = allAdress.Telephone;
                        address1.Postcode = allAdress.CodePostal;
                        address1.City = allAdress.Ville;
                        //country name must be lin lower case except first charactere
                        var region1 = CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(ci => new RegionInfo(ci.LCID)).FirstOrDefault(rg => rg.DisplayName == allAdress.Pays.ToUpper());
                        address1.CountryId = "";//region.TwoLetterISORegionName;
                        address1.DefaultBilling = false;
                        address1.DefaultShipping = false;
                        address1.Street = new List<string>();
                        address1.Street.Add(allAdress.Adresse);
                        if (allAdress.Complement.Length > 0)
                        {
                            address1.Street.Add(allAdress.Complement);
                        }
                        Addresses.Add(address1);
                    }

                }
            }
            


            string[] Name = client.Intitule.Split(' ');
            if (Name.Length == 3)
            {
                Firstname = Name[1];
                Lastname = Name[2];
            }
            else if (Name.Length ==2)
            {
                Firstname = Name[0];
                Lastname = Name[1];
            }
            else
            {
                Firstname = Name[0];
                Lastname = Name[0];
            }



            var customerJson = new
            {
                customer = new
                {
                    id= customerSearchByEmail.Items[0].Id.ToString(),
                    group_id = group_id,
                    email = client.Email.ToString(),
                    firstname = Firstname,
                    lastname = Lastname,
                    website_id = Utils.UtilsConfig.Store.ToString(),
                    addresses = Addresses,
                    disable_auto_group_change = "0",
                    extension_attributes = new { is_subscribed = false },
                    custom_attributes = CustomAttributes
                },
                password = "Password123"
            };
            return customerJson;
        }
    }
}
