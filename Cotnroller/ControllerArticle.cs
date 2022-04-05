using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebservicesSage.Singleton;
using WebservicesSage.Object;
using WebservicesSage.Object.Search;
using Objets100cLib;
using System.Windows.Forms;
using WebservicesSage.Utils;
using WebservicesSage.Utils.Enums;
using System.Timers;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using Attribute = WebservicesSage.Object.Attribute.Attribute;

namespace WebservicesSage.Cotnroller
{
    class ControllerArticle
    {

        public static void LaunchService()
        {/*
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(SendStockCrone);
            timer.Interval = UtilsConfig.CronTaskStock;
            timer.Enabled = true;*/
            /*System.Timers.Timer timerUpdateStatut = new System.Timers.Timer();
            timerUpdateStatut.Elapsed += new ElapsedEventHandler(SendAllProductsCron);
            timerUpdateStatut.Interval = 20000;
            timerUpdateStatut.Enabled = true;
            */
        }
        public static void SendAllProductsCron(object source, ElapsedEventArgs e)
        {
            DateTime now1 = DateTime.Now;
            DateTime NowNoSecond2 = new DateTime(now1.Year, now1.Month, now1.Day, now1.Hour, now1.Minute, 0, 0);
            DateTime firstRun2 = new DateTime(NowNoSecond2.Year, NowNoSecond2.Month, NowNoSecond2.Day, 0, 0, 0, 0);
            if (NowNoSecond2 == firstRun2)
            {
                UtilsConfig.CRONSYNCHROPRODUCTDONE = "FALSE";
            }
            string[] date = UtilsConfig.CRONSYNCHROPRODUCT.ToString().Split(':');
            int hours, minutes;
            hours = Int32.Parse(date[0]);
            minutes = Int32.Parse(date[1]);
            DateTime now = DateTime.Now;
            DateTime NowNoSecond = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, 0);
            DateTime firstRun = new DateTime(now.Year, now.Month, now.Day, hours, minutes, 0, 0);
            if (NowNoSecond == firstRun && UtilsConfig.CRONSYNCHROPRODUCTDONE.ToString().Equals("FALSE"))
            {
                UtilsConfig.CRONSYNCHROPRODUCTDONE = "TRUE";
                var gescom = SingletonConnection.Instance.Gescom;
                string RefErr = "";
                string JsonErr = "";
                    foreach (IBOArticle3 articleSageObj in gescom.FactoryArticle.List)
                    {
                        if (!articleSageObj.AR_Publie)
                        {
                            continue;
                        }
                        else
                        {
                        RefErr = articleSageObj.AR_Ref;
                        try
                        {
                            List<ArticleNomenclature> ArticleNomenclature = new List<ArticleNomenclature>();
                            Product product = new Product();
                            Article article;
                            article = new Article(articleSageObj);
                            ProductSearchCriteria productMagento = UtilsWebservices.GetMagentoProduct("rest/V1/products", UtilsWebservices.SearchCriteria("sku", article.Reference, "eq"));

                            string articleXML = UtilsSerialize.SerializeObject<Article>(article);
                            var jsonName = JsonConvert.SerializeObject(product.SimpleProductjson(article, null, null, null, productMagento, true));
                            //File.AppendAllText("Log\\data.txt", jsonName.ToString() + Environment.NewLine);
                            string responseName = UtilsWebservices.SendDataJson(jsonName, @"rest/all/V1/products/");
                            var json = JsonConvert.SerializeObject(product.SimpleProductjson(article, null, null, null, productMagento));
                            //File.AppendAllText("Log\\data.txt", json.ToString() + Environment.NewLine);
                            string response = UtilsWebservices.SendDataJson(json, @"rest/V1/products/");
                        }
                        catch (Exception s)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append(DateTime.Now + RefErr + Environment.NewLine);
                            sb.Append(DateTime.Now + s.Message + Environment.NewLine);
                            sb.Append(DateTime.Now + s.StackTrace + Environment.NewLine);
                            File.AppendAllText("Log\\SyncAll.txt", sb.ToString());
                            sb.Clear();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Permets de remonter toute la base articles de SAGE vers Prestashop
        /// Ne remonte que les articles coché en publier sur le site marchand !
        /// </summary>
        public static void SendAllArticles()
        {
            var gescom = SingletonConnection.Instance.Gescom;
            string RefErr = "";
            string JsonErr = "";
            try
            {
                foreach (IBOArticle3 articleSageObj in gescom.FactoryArticle.List)
                {
                    if (!articleSageObj.AR_Publie)
                    {
                        continue;
                    }
                    else
                    {
                        RefErr = articleSageObj.AR_Ref;
                        try
                        {
                            List<ArticleNomenclature> ArticleNomenclature = new List<ArticleNomenclature>();
                            Product product = new Product();
                            Article article;
                            article = new Article(articleSageObj);
                            ProductSearchCriteria productMagento = UtilsWebservices.GetMagentoProduct("rest/V1/products", UtilsWebservices.SearchCriteria("sku", article.Reference, "eq"));

                            string articleXML = UtilsSerialize.SerializeObject<Article>(article);
                            var jsonName = JsonConvert.SerializeObject(product.SimpleProductjson(article, null, null, null, productMagento, true));
                            File.AppendAllText("Log\\data.txt", jsonName.ToString() + Environment.NewLine);
                            string responseName = UtilsWebservices.SendDataJson(jsonName, @"rest/all/V1/products/");
                            var json = JsonConvert.SerializeObject(product.SimpleProductjson(article, null, null, null, productMagento));
                            File.AppendAllText("Log\\data.txt", json.ToString() + Environment.NewLine);
                            string response = UtilsWebservices.SendDataJson(json, @"rest/V1/products/");
                        }
                        catch (Exception s)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append(DateTime.Now + RefErr + Environment.NewLine);
                            sb.Append(DateTime.Now + s.Message + Environment.NewLine);
                            sb.Append(DateTime.Now + s.StackTrace + Environment.NewLine);
                            File.AppendAllText("Log\\SyncAll.txt", sb.ToString());
                            sb.Clear();
                        }
                        // SingletonUI.Instance.ProgressBar.Invoke((MethodInvoker)(() => SingletonUI.Instance.ProgressBar.Value = 100));

                    }

                }
                MessageBox.Show("end sync", "end",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        public static void SendCustomArticles(string reference)
        {
            try
            {
                List<ArticleNomenclature> ArticleNomenclature = new List<ArticleNomenclature>();
                var gescom = SingletonConnection.Instance.Gescom;
                Product product = new Product();
                Article article;

                var articleSageObj = gescom.FactoryArticle.ReadReference(reference);
                //var articletest = articleSageObj;
                if (!articleSageObj.AR_Publie)
                {
                    MessageBox.Show("Article non publie sur le site", "end", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    article = new Article(articleSageObj);
                    ProductSearchCriteria productMagento = UtilsWebservices.GetMagentoProduct("rest/V1/products", UtilsWebservices.SearchCriteria("sku", article.Reference, "eq"));
                    string articleXML = UtilsSerialize.SerializeObject<Article>(article);

                    var jsonName = JsonConvert.SerializeObject(product.SimpleProductjson(article, null, null, null, productMagento, true));
                    //File.AppendAllText("Log\\data.txt", jsonName.ToString() + Environment.NewLine);
                    string responseName = UtilsWebservices.SendDataJson(jsonName, @"rest/all/V1/products/");
                    var json = JsonConvert.SerializeObject(product.SimpleProductjson(article, null, null, null, productMagento));
                    //File.AppendAllText("Log\\data.txt", json.ToString() + Environment.NewLine);
                    string response = UtilsWebservices.SendDataJson(json, @"rest/V1/products/");
                }
                MessageBox.Show("end sync", "end", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(DateTime.Now + e.Message + Environment.NewLine);
                sb.Append(DateTime.Now + e.StackTrace + Environment.NewLine);
                File.AppendAllText("Log\\SynchArt.txt", sb.ToString());
                sb.Clear();
                MessageBox.Show(e.Message, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }
        }

        /// <summary>
        /// Permets de récupérer une liste d'articles propre depuis une liste d'artivcle SAGE
        /// Permets de gérer la configuration des produits
        /// </summary>
        /// <param name="articleSageObj">Liste d'article SAGE</param>
        /// <returns></returns>
        public static List<Article> GetListOfClientToProcess(IBICollection articleSageObj)
        {
            List<Article> articleToProcess = new List<Article>();
            string CurrentRefArticle = "";

                int incre = 0;
                foreach (IBOArticle3 articleSage in articleSageObj)
                {
                    CurrentRefArticle = articleSage.AR_Ref;
                    try
                    {
                        SingletonUI.Instance.ArticleNumber.Invoke((MethodInvoker)(() => SingletonUI.Instance.ArticleNumber.Text = "Fetching Data : " + incre));

                        // on check si l'article est cocher en publier sur le site marchand
                        if (!articleSage.AR_Publie)
                            continue;

                        Article article = new Article(articleSage);

                        if (!HandleArticleError(article))
                        {
                            articleToProcess.Add(article);
                        }
                    }
                    catch (Exception e)
                    {
                    UtilsMail.SendErrorMail(DateTime.Now + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine, "ARTICLE "+ CurrentRefArticle);
                    }
                incre++;
                }
            return articleToProcess;
        }

        /// <summary>
        /// Permet de vérifier si un article comporte des erreur ou non
        /// </summary>
        /// <param name="article">Article à tester</param>
        /// <returns></returns>
        private static bool HandleArticleError(Article article)
        {

            return false;
        }

        /// <summary>
        /// Permet de récupérer l'énuméré SAGE 1 d'un article 
        /// </summary>
        /// <param name="article"></param>
        /// <param name="gamme">Gamme sur laquelle nous devont chercher l'énuméré</param>
        /// <returns></returns>
        public static IBOArticleGammeEnum3 GetArticleGammeEnum1(IBOArticle3 article, Gamme gamme)
        {
            foreach(IBOArticleGammeEnum3 articleEnum in article.FactoryArticleGammeEnum1.List)
            {
                if (articleEnum.EG_Enumere.Equals(gamme.Value_Intitule))
                {
                    return articleEnum;
                }
            }

            return null;
        }
        public static IBOArticleCond3 GetArticleConditionnementEnum(IBOArticle3 article)
        {
            foreach (IBOArticleCond3 articleEnum in article.FactoryArticleCond.List)
            {
                if (!String.IsNullOrEmpty(articleEnum.EC_Enumere))
                {
                    return articleEnum;
                }
            }

            return null;
        }

        /// <summary>
        /// Permet de récupérer l'énuméré SAGE 2 d'un article 
        /// </summary>
        /// <param name="article"></param>
        /// <param name="gamme">Gamme sur laquelle nous devont chercher l'énuméré</param>
        /// <returns></returns>
        public static IBOArticleGammeEnum3 GetArticleGammeEnum2(IBOArticle3 article, Gamme gamme)
        {
            foreach (IBOArticleGammeEnum3 articleEnum in article.FactoryArticleGammeEnum1.List)
            {
                foreach (IBOArticleGammeEnumRef3 articleEnum2 in articleEnum.FactoryArticleGammeEnumRef.List)
                {
                    if (articleEnum.EG_Enumere.Equals(gamme.Value_Intitule) && articleEnum2.ArticleGammeEnum2.EG_Enumere.Equals(gamme.Value_Intitule2))
                    {
                        return articleEnum2.ArticleGammeEnum2;
                    }
                }
               
            }

            return null;
        }

        public static void SendStockCrone(object source, ElapsedEventArgs e)
        {
            string currentArticleRef = "";
            try
            {
                var gescom = SingletonConnection.Instance.Gescom;
                var articleSageObj = gescom.FactoryArticle.List;
                var articles = GetListOfClientToProcess(articleSageObj);

                int increm = 1;
                int tmpiter = articles.Count % 9;
                int iter = (articles.Count - tmpiter) / 9;

                foreach (Article article in articles)
                {
                    currentArticleRef = article.Reference;
                    SingletonUI.Instance.ArticleNumber.Invoke((MethodInvoker)(() => SingletonUI.Instance.ArticleNumber.Text = "Sending data : " + increm));

                    
                    string articleXML = UtilsSerialize.SerializeObject<Article>(article);
                    UtilsWebservices.SendData(UtilsConfig.BaseUrl + EnumEndPoint.Stock.Value, articleXML);
                    increm++;
                }

               // SingletonUI.Instance.ProgressBar.Invoke((MethodInvoker)(() => SingletonUI.Instance.ProgressBar.Value = 100));

            }
            catch (Exception t)
            {
                UtilsMail.SendErrorMail(DateTime.Now + t.Message + Environment.NewLine + t.StackTrace + Environment.NewLine, "CRON STOCK");
            }
        }

        public static void SendStock()  
        {
            string currentArticleRef = "";
            try
            {
                var gescom = SingletonConnection.Instance.Gescom;
                var articleSageObj = gescom.FactoryArticle.List;
                var articles = GetListOfClientToProcess(articleSageObj);

                int increm = 1;
                int tmpiter = articles.Count % 9;
                int iter = (articles.Count - tmpiter) / 9;

                foreach (Article article in articles)
                {
                    currentArticleRef = article.Reference;
                    Product product = new Product();
                    SingletonUI.Instance.ArticleNumber.Invoke((MethodInvoker)(() => SingletonUI.Instance.ArticleNumber.Text = "Sending data : " + increm));

                    if (article.isGamme)
                    {

                        foreach (Gamme gamme in article.Gammes)
                        {
                            string sku = "";
                            if (gamme.Reference != null)
                            {
                                sku = gamme.Reference;
                            }
                            else
                            {
                                if (article.IsDoubleGamme)
                                {
                                    sku = gamme.Value_Intitule + gamme.Value_Intitule2;
                                }
                                else
                                {
                                    sku = gamme.Value_Intitule;
                                }
                            }

                            var json = JsonConvert.SerializeObject(product.CustomProductStock(article, gamme));
                            ProductSearchCriteria productExist = UtilsWebservices.GetMagentoProduct(@"/rest/V1/products/", UtilsWebservices.SearchCriteria("sku", sku, "eq"));
                            if (productExist.TotalCount > 0 )
                            {
                                string response = UtilsWebservices.SendDataJson(json, "/rest/V1/products/" + sku, "PUT");
                            }
                            else
                            {
                                try
                                {
                                    if (article.isGamme)
                                    {
                                        var json1 = JsonConvert.SerializeObject(product.ConfigurableProductjson(article));
                                        string response = UtilsWebservices.SendDataJson(json1, "/rest/V1/products");
                                        if (article.IsDoubleGamme)
                                        {
                                            UtilsWebservices.CreateDoublesGammeProduct(article);
                                        }
                                        else
                                        {
                                            UtilsWebservices.CreateGammesProduct(article);
                                        }
                                    }
                                    else
                                    {
                                        var json1 = JsonConvert.SerializeObject(product.SimpleProductjson(article));
                                        string response = UtilsWebservices.SendDataJson(json1, @"rest/V1/products/");

                                    }
                                }
                                catch (Exception e)
                                {
                                    UtilsMail.SendErrorMail(DateTime.Now + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine, "Creation de Gamme Product : " + sku);
                                }
                            }

                        }
                    }
                    else
                    {

                        var json = JsonConvert.SerializeObject(product.CustomProductStock(article));
                        //ProductSearchCriteria productExist = UtilsWebservices.GetMagentoProduct(@"/rest/all/V1/products/", article.Reference);
                        ProductSearchCriteria productExist = UtilsWebservices.GetMagentoProduct(@"/rest/all/V1/products/", UtilsWebservices.SearchCriteria("sku", article.Reference, "eq"));
                        if (productExist.TotalCount > 0)
                        {
                            string response = UtilsWebservices.SendDataJson(json, "/rest/all/V1/products/" + article.Reference, "PUT");
                        }
                        else
                        {
                            try
                            {
                                var json1 = JsonConvert.SerializeObject(product.SimpleProductjson(article));
                                string response = UtilsWebservices.SendDataJson(json1, @"rest/all/V1/products/");
                            }
                            catch (Exception e)
                            {
                                UtilsMail.SendErrorMail(DateTime.Now + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine, "Creation de Product : " + currentArticleRef);
                            }
                        }
                    }
                    
                    increm++;
                }

                //SingletonUI.Instance.ProgressBar.Invoke((MethodInvoker)(() => SingletonUI.Instance.ProgressBar.Value = 100));
                MessageBox.Show("end sync", "end",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        public static void SendCustomStock(string reference)
        {
            try
            {
                var gescom = SingletonConnection.Instance.Gescom;
                var articleSageObj = gescom.FactoryArticle.ReadReference(reference);
                Article article = new Article(articleSageObj);

                int increm = 1;

                Product product = new Product();
                SingletonUI.Instance.ArticleNumber.Invoke((MethodInvoker)(() => SingletonUI.Instance.ArticleNumber.Text = "Sending data : " + increm));
                if (article.isGamme)
                {

                    foreach (Gamme gamme in article.Gammes)
                    {
                        string sku = "";
                        if (gamme.Reference != null)
                        {
                            sku = gamme.Reference;
                        }
                        else
                        {
                            if (article.IsDoubleGamme)
                            {
                                sku = gamme.Value_Intitule + gamme.Value_Intitule2;
                            }
                            else
                            {
                                sku = gamme.Value_Intitule;
                            }
                        }

                        var json = JsonConvert.SerializeObject(product.CustomProductStock(article, gamme));
                        ProductSearchCriteria productExist = UtilsWebservices.GetMagentoProduct(@"/rest/V1/products/", UtilsWebservices.SearchCriteria("sku",sku,"eq"));
                        if (productExist != null)
                        {
                            string response = UtilsWebservices.SendDataJson(json, "/rest/V1/products/" + sku, "PUT");
                        }
                        else
                        {
                            try
                            {
                                if (article.isGamme)
                                {
                                    var json1 = JsonConvert.SerializeObject(product.ConfigurableProductjson(article));
                                    string response = UtilsWebservices.SendDataJson(json1, "/rest/V1/products");
                                    if (article.IsDoubleGamme)
                                    {
                                        UtilsWebservices.CreateDoublesGammeProduct(article);
                                    }
                                    else
                                    {
                                        UtilsWebservices.CreateGammesProduct(article);
                                    }
                                }
                                else
                                {
                                    var json1 = JsonConvert.SerializeObject(product.SimpleProductjson(article));
                                    string response = UtilsWebservices.SendDataJson(json1, @"rest/V1/products/");

                                }
                            }
                            catch (Exception e)
                            {
                                UtilsMail.SendErrorMail(DateTime.Now + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine, "Creation de Gamme Product : " + reference);
                            }
                        }

                    }
                }
                else
                {

                    var json = JsonConvert.SerializeObject(product.CustomProductStock(article));
                    //ProductSearchCriteria productExist = UtilsWebservices.GetMagentoProduct(@"/rest/all/V1/products/", article.Reference);
                    ProductSearchCriteria productExist = UtilsWebservices.GetMagentoProduct(@"/rest/V1/products/", UtilsWebservices.SearchCriteria("sku", article.Reference, "eq"));
                    if (productExist.TotalCount >0)
                    {
                        string response = UtilsWebservices.SendDataJson(json, "/rest/V1/products/" + article.Reference, "PUT");
                    }
                    else
                    {
                        try
                        {
                            var json1 = JsonConvert.SerializeObject(product.SimpleProductjson(article));
                            string response = UtilsWebservices.SendDataJson(json1, @"rest/V1/products/");
                        }
                        catch (Exception e)
                        {
                            UtilsMail.SendErrorMail(DateTime.Now + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine, "Creation de Product : " + reference);
                        }
                    }

                }
                /*string articleXML = UtilsSerialize.SerializeObject<Article>(article);
                UtilsWebservices.SendData(UtilsConfig.BaseUrl + EnumEndPoint.Stock.Value, articleXML);      */

                //  SingletonUI.Instance.ProgressBar.Invoke((MethodInvoker)(() => SingletonUI.Instance.ProgressBar.Value = 100));
                MessageBox.Show("end sync", "end",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        public static void SendPrice()
        {
            try
            {
                var gescom = SingletonConnection.Instance.Gescom;
                var articleSageObj = gescom.FactoryArticle.List;
                var articles = GetListOfClientToProcess(articleSageObj);

                int increm = 1;
                int tmpiter = articles.Count % 9;
                int iter = (articles.Count - tmpiter) / 9;

                foreach (Article article in articles)
                {

                    SingletonUI.Instance.ArticleNumber.Invoke((MethodInvoker)(() => SingletonUI.Instance.ArticleNumber.Text = "Sending data : " + increm));

                    Product product = new Product();
                    if (article.isGamme)
                    {

                        foreach (Gamme gamme in article.Gammes)
                        {
                            string sku = "";
                            if (gamme.Reference != null)
                            {
                                sku = gamme.Reference;
                            }
                            else
                            {
                                if (article.IsDoubleGamme)
                                {
                                    sku = gamme.Value_Intitule + gamme.Value_Intitule2;
                                }
                                else
                                {
                                    sku = gamme.Value_Intitule;
                                }
                            }

                            var json = JsonConvert.SerializeObject(product.CustomProductPrice(article, gamme));
                            ProductSearchCriteria productExist = UtilsWebservices.GetMagentoProduct(@"/rest/V1/products/", UtilsWebservices.SearchCriteria("sku", sku, "eq"));
                            if (productExist.TotalCount > 0)
                            {
                                string response = UtilsWebservices.SendDataJson(json, "/rest/V1/products/" + sku, "PUT");
                            }
                            else
                            {
                                try
                                {
                                    if (article.isGamme)
                                    {
                                        var json1 = JsonConvert.SerializeObject(product.ConfigurableProductjson(article));
                                        string response = UtilsWebservices.SendDataJson(json1, "/rest/V1/products");
                                        if (article.IsDoubleGamme)
                                        {
                                            UtilsWebservices.CreateDoublesGammeProduct(article);
                                        }
                                        else
                                        {
                                            UtilsWebservices.CreateGammesProduct(article);
                                        }
                                    }
                                    else
                                    {
                                        var json1 = JsonConvert.SerializeObject(product.SimpleProductjson(article));
                                        string response = UtilsWebservices.SendDataJson(json1, @"rest/V1/products/");

                                    }
                                }
                                catch (Exception e)
                                {
                                    UtilsMail.SendErrorMail(DateTime.Now + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine, "Creation de Gamme Product : " + article.Reference);
                                }
                            }

                        }
                    }
                    else
                    {

                        var json = JsonConvert.SerializeObject(product.CustomProductPrice(article));
                        ProductSearchCriteria productExist = UtilsWebservices.GetMagentoProduct(@"/rest/V1/products/", UtilsWebservices.SearchCriteria("sku", article.Reference, "eq"));
                        if (productExist.TotalCount > 0)
                        {
                            string response = UtilsWebservices.SendDataJson(json, "/rest/V1/products/" + article.Reference, "PUT");
                        }
                        else
                        {
                            try
                            {
                                var json1 = JsonConvert.SerializeObject(product.SimpleProductjson(article));
                                string response = UtilsWebservices.SendDataJson(json1, @"rest/V1/products/");
                            }
                            catch (Exception e)
                            {
                                UtilsMail.SendErrorMail(DateTime.Now + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine, "Creation de Product : " + article.Reference);
                            }
                        }

                    }


                    increm++;
                }

               // SingletonUI.Instance.ProgressBar.Invoke((MethodInvoker)(() => SingletonUI.Instance.ProgressBar.Value = 100));
                MessageBox.Show("end sync", "end",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        public static void SendCustomPrice(string reference)
        {
            try
            {
                var gescom = SingletonConnection.Instance.Gescom;
                var articleSageObj = gescom.FactoryArticle.ReadReference(reference);
                Article article = new Article(articleSageObj);

                int increm = 1;

                SingletonUI.Instance.ArticleNumber.Invoke((MethodInvoker)(() => SingletonUI.Instance.ArticleNumber.Text = "Sending data : " + increm));
                Product product = new Product();
                if (article.isGamme)
                {

                    foreach (Gamme gamme in article.Gammes)
                    {
                        string sku = "";
                        if (gamme.Reference != null)
                        {
                            sku = gamme.Reference;
                        }
                        else
                        {
                            if (article.IsDoubleGamme)
                            {
                                sku = gamme.Value_Intitule + gamme.Value_Intitule2;
                            }
                            else
                            {
                                sku = gamme.Value_Intitule;
                            }
                        }

                        var json = JsonConvert.SerializeObject(product.CustomProductPrice(article, gamme));
                        ProductSearchCriteria productExist = UtilsWebservices.GetMagentoProduct(@"/rest/V1/products/", UtilsWebservices.SearchCriteria("sku", sku, "eq"));
                        if (productExist.TotalCount > 0 )
                        {
                            string response = UtilsWebservices.SendDataJson(json, "/rest/V1/products/" + sku, "PUT");
                        }
                        else
                        {
                            try
                            {
                                if (article.isGamme)
                                {
                                    var json1 = JsonConvert.SerializeObject(product.ConfigurableProductjson(article));
                                    string response = UtilsWebservices.SendDataJson(json1, "/rest/V1/products");
                                    if (article.IsDoubleGamme)
                                    {
                                        UtilsWebservices.CreateDoublesGammeProduct(article);
                                    }
                                    else
                                    {
                                        UtilsWebservices.CreateGammesProduct(article);
                                    }
                                }
                                else
                                {
                                    var json1 = JsonConvert.SerializeObject(product.SimpleProductjson(article));
                                    string response = UtilsWebservices.SendDataJson(json1, @"rest/V1/products/");

                                }
                            }
                            catch (Exception e)
                            {
                                UtilsMail.SendErrorMail(DateTime.Now + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine, "Creation de Gamme Product : " + reference);
                            }
                        }

                    }
                }
                else
                {

                    var json = JsonConvert.SerializeObject(product.CustomProductPrice(article));
                    ProductSearchCriteria productExist = UtilsWebservices.GetMagentoProduct(@"/rest/V1/products/", UtilsWebservices.SearchCriteria("sku", article.Reference, "eq"));
                    if (productExist.TotalCount > 0 )
                    {
                        string response = UtilsWebservices.SendDataJson(json, "/rest/V1/products/" + article.Reference, "PUT");
                    }
                    else
                    {
                        try
                        {
                            var json1 = JsonConvert.SerializeObject(product.SimpleProductjson(article));
                            string response = UtilsWebservices.SendDataJson(json1, @"rest/V1/products/");
                        }
                        catch (Exception e)
                        {
                            UtilsMail.SendErrorMail(DateTime.Now + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine, "Creation de Product : " + reference);
                        }
                    }

                }

                MessageBox.Show("end sync", "end",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }
    }
}







