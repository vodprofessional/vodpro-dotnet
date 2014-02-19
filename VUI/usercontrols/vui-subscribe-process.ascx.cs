using System;
using System.Linq;
using System.Text;
using System.Xml;
using umbraco.cms.businesslogic.member;
using umbraco.cms.businesslogic.web;
using umbraco.MacroEngines;
using VUI.classes;

namespace VUI.usercontrols
{
    public partial class vui_subscribe_process : System.Web.UI.UserControl
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(vui_subscribe_process));

        string ACTION = String.Empty;

        string PRODUCTCODE = String.Empty;

        Member m = null;

        const string REDIRECT_HOME = "HOME";
        const string REDIRECT_SUBSCRIBE = "SUBSCRIBE";
        const string REDIRECT_THANKYOU = "THANKYOU";
        const string REDIRECT_LOGIN = "LOGIN";
        const string REDIRECT_CHECKOUT = "CHECKOUT";
        const string REDIRECT_FAIL = "FAIL";

        private bool bDenyIfLoggedIn = false;

        public string DenyIfLoggedIn { set { if (value.ToLower().Equals("yes")) { bDenyIfLoggedIn = true; } else { bDenyIfLoggedIn = false; }; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            m = VUIfunctions.CurrentUser();

            if (Request["VUIACTION"] != null)
            {
                ACTION = Request["VUIACTION"].ToString();
            }

            if (String.IsNullOrEmpty(ACTION))
            {
                Redirect(REDIRECT_SUBSCRIBE);
            }
            
            log.Debug("ACTION" + ACTION);

            // BUY is triggered when the Buy Button on the Subscribe page is clicked
            if (ACTION.Equals("BUY"))
            {
                log.Debug("Buy Actions");
                if (Request["CODE"] != null)
                {
                    PRODUCTCODE = Request["CODE"].ToString().ToUpper();


                    log.Debug("PRODUCTCODE:" + PRODUCTCODE);

                    DynamicNode node = new DynamicNode(VUIfunctions.VUI_product_list);
                    int productCount = 0;
                    try
                    {
                        productCount = node.Descendants("VUISubscriptionProduct").Items.Where(n => n.GetProperty("productCode").Value.ToUpper().Equals(PRODUCTCODE)).ToList().Count();
                        log.Debug("IS VALID CODE? " + productCount);
                        if (productCount > 0)
                        {
                            Session["VUI_PRODUCT_CODE"] = PRODUCTCODE;
                        }
                        else
                        {
                            Redirect(VUIfunctions.VUI_subscribe_page);
                        }
                    }
                    catch (Exception ex1)
                    {
                        Redirect(VUIfunctions.VUI_subscribe_page);
                    }
                }

                // Logged in
                if (m != null)
                {
                    string user_status = VUIfunctions.MemberVUIStatus(m);

                    log.Debug("USER STATUS " + user_status);

                    // THe user is already a VUI User or Administrator (Full Mode)
                    if (bDenyIfLoggedIn && VUIfunctions.MemberVUIFullMode(user_status))
                    {
                        Redirect("/vui");
                    }
                    // The user has registered / logged in but is not a VUI subscriber yet.
                    // They will be redirected to the checkout page
                    else
                    {

                        DynamicNode product = (new DynamicNode(VUIfunctions.VUI_product_list)).Descendants("VUISubscriptionProduct").Items.Where(n => n.GetProperty("productCode").Value.ToUpper().Equals(PRODUCTCODE)).ToList().First();
                        double priceBeforeDiscount = double.Parse(product.GetProperty("price").ToString());
                        
                        // This is always zero at this stage
                        int discountPercentage = 0;
                        int discountAmount = 0;
                        
                        double price = priceBeforeDiscount - discountAmount;
                        

                        string country = m.getProperty("companyCountry").Value.ToString();
                        double tax = 0.0;
                        if (!VUIfunctions.CountryIsVATExempt(country))
                        {
                            tax = price * 0.2;
                        }

                        string productName = product.GetProperty("productName").ToString();
                        string currency = product.GetProperty("currency").ToString();
                        if (String.IsNullOrEmpty(currency))
                        {
                            currency = "GBP";
                        }
                        string nextProduct = String.Empty;

                        nextProduct = product.GetProperty("linkedProduct").ToString();
                        if (!String.IsNullOrEmpty(nextProduct))
                        {
                            log.Debug("Found a linked Product: [" + nextProduct + "]");
                        }
                        else
                        {
                            log.Debug("No linked products");
                        }

                        // Open the VUI_transation log item                        
                        // Iterate the tx number and save
                        Document translog = new Document(VUIfunctions.VUI_transaction_log);
                        int transactionNumber = (Int32)(translog.getProperty("transactionNumber").Value);
                        transactionNumber += 1;
                        translog.getProperty("transactionNumber").Value = transactionNumber;
                        translog.Save();
                        translog.Publish(VUIfunctions.u);

                        // Create a new transaction Item and pass the ID as a session variable
                        Document transaction = Document.MakeNew("VUI" + transactionNumber, DocumentType.GetByAlias("VUI_Transaction_Item"), VUIfunctions.u, VUIfunctions.VUI_transaction_log);
                        transaction.getProperty("transactionNumber").Value = transactionNumber;
                        transaction.getProperty("user").Value = m.Id;
                        transaction.getProperty("status").Value = "INITIATED";
                        transaction.getProperty("productID").Value = PRODUCTCODE;
                        transaction.getProperty("productName").Value = productName;
                        transaction.getProperty("priceBeforeDiscount").Value = String.Format("{0:0.00}", priceBeforeDiscount);
                        transaction.getProperty("promotionDiscount").Value = String.Format("{0:0.00}", discountAmount);
                        transaction.getProperty("currency").Value = currency;
                        transaction.getProperty("price").Value = String.Format("{0:0.00}", price);
                        transaction.getProperty("tax").Value = String.Format("{0:0.00}", tax);
                        transaction.getProperty("transactionDate").Value = DateTime.Now;
                        if (!String.IsNullOrEmpty(nextProduct))
                        {
                            transaction.getProperty("linkedProduct").Value = nextProduct;
                        }

                        // Set any Affiliate Code on the transaction
                        // Precendence:
                        // 1. Affiliate Cookie
                        // 2. Affiliate used for registration
                        if (Request.Cookies["VUI_AFFILIATE_CODE"] != null && !String.IsNullOrEmpty(Request.Cookies["VUI_AFFILIATE_CODE"].Value))
                        {
                            int affiliateid;
                            if (Int32.TryParse(Request.Cookies["VUI_AFFILIATE_CODE"].Value, out affiliateid))
                            {
                                transaction.getProperty("affiliateId").Value = affiliateid;
                            }
                        }
                        else if (m.getProperty("affiliateId").Value != null)
                        {
                            transaction.getProperty("affiliateId").Value = m.getProperty("affiliateId").Value;
                        }
                        transaction.Save();
                        
                        // This will follow the user to the payPal step and then be deleted
                        Session["VUITXID"] = transaction.Id;

                        // Update the USer's personal transaction log
                        string userTransactions = m.getProperty("userTransactionLog").Value.ToString().Replace("{", "").Replace("}", "");
                        StringBuilder sb = new StringBuilder("");
                
                        int itemid = 0;
                        int sortOrder = 0;
                        if(!String.IsNullOrEmpty(userTransactions))
                        {
                         
                            XmlDocument xml = new XmlDocument();
                            xml.LoadXml(userTransactions);

                            XmlNodeList transList;
                            transList = xml.SelectSingleNode("items").SelectNodes("item");
                    
                            foreach (XmlNode item in transList)
                            {
                                sb.Append(item.OuterXml);
                                itemid = Int32.Parse(item.Attributes["id"].Value);
                                sortOrder = Int32.Parse(item.Attributes["sortOrder"].Value);
                            }
                        }
                        itemid++; sortOrder++;

                        sb.Append(@"<item id=""" + itemid + @""" sortOrder=""" + sortOrder + @""">");
                        sb.Append(@"<transactionNumber nodeName=""Transaction Number"" nodeType=""-51"">" + transactionNumber + @"</transactionNumber>");
                        sb.Append(@"<status nodeName=""Status"" nodeType=""-88"">INITIATED</status>");
                        sb.Append(@"<transactionDate nodeName=""Status"" nodeType=""-36"">" + DateTime.Now.ToString() + "</transactionDate>");
                        sb.Append(@"</item>");
                        m.getProperty("userTransactionLog").Value = @"<items>" + sb.ToString() + @"</items>";
                        m.Save();


                        log.Debug("TRANSACTION INITATED for " + m.LoginName + " #" + transactionNumber);


                        log.Debug("REDIRECT TO CHECKOUT");
                        Redirect(VUIfunctions.VUI_checkout_page);
                    }
                }
                // Not logged in - make them log in or register
                else
                {
                    log.Debug("NOT LOGGED IN!");
                    Redirect(VUIfunctions.VUI_loginregister_page);
                }
            }

            if (ACTION.Equals("EUVAT"))
            {// Logged in
                if (m != null)
                {
                    if (Session["VUITXID"] != null)
                    {
                        int transactionId;
                        if (Int32.TryParse(Session["VUITXID"].ToString(), out transactionId))
                        {
                            Document transaction = new Document(transactionId);
                            if (Request["CODE"] != null && !String.IsNullOrEmpty(Request["CODE"].ToString()))
                            {
                                transaction.getProperty("euVATNumber").Value = Request["CODE"].ToString();
                                transaction.getProperty("tax").Value = "0.00";
                                transaction.Save();
                            }
                        }
                    }
                    Redirect(VUIfunctions.VUI_checkout_page);
                }
                else
                {
                    log.Debug("NOT LOGGED IN!");
                    Redirect(VUIfunctions.VUI_loginregister_page);
                }
            }

            if (ACTION.Equals("PONUMBER"))
            {// Logged in
                if (m != null)
                {
                    if (Session["VUITXID"] != null)
                    {
                        int transactionId;
                        if (Int32.TryParse(Session["VUITXID"].ToString(), out transactionId))
                        {
                            Document transaction = new Document(transactionId);
                            if (Request["CODE"] != null && !String.IsNullOrEmpty(Request["CODE"].ToString()))
                            {
                                transaction.getProperty("purchaseOrderNumber").Value = Request["CODE"].ToString();
                                transaction.Save();
                            }
                        }
                    }
                    Redirect(VUIfunctions.VUI_checkout_page);
                }
                else
                {
                    log.Debug("NOT LOGGED IN!");
                    Redirect(VUIfunctions.VUI_loginregister_page);
                }
            }

            if (ACTION.Equals("PROMO"))
            {// Logged in
                if (m != null)
                {
                    if (Session["VUITXID"] != null)
                    {
                        int transactionId;
                        if (Int32.TryParse(Session["VUITXID"].ToString(), out transactionId))
                        {
                            Document transaction = new Document(transactionId);
                            if (Request["CODE"] != null && !String.IsNullOrEmpty(Request["CODE"].ToString()))
                            {
                                string PROMOCODE = Request["CODE"].ToString().ToUpper();
                                try
                                {
                                    log.Debug("User trying to add a Promo Code: " + PROMOCODE);
                                    DynamicNode promo = (new DynamicNode(VUIfunctions.VUI_information_root)).Descendants("VUIPromoCode").Items.Where(n => n.GetProperty("promotionCode").Value.ToUpper().Equals(PROMOCODE)).ToList().First();

                                    // Check whether promo is still valid

                                    if (VUIfunctions.PromotionIsValid(promo))
                                    {
                                        double priceBeforeDiscount = double.Parse(transaction.getProperty("priceBeforeDiscount").Value.ToString());
                                        double discountPercentage = 0.0;
                                        double discountAmount = 0.0;

                                        // Calculate the new prices before and after discounting
                                        if (double.TryParse(promo.GetProperty("discount").Value, out discountPercentage))
                                        {
                                            discountAmount = priceBeforeDiscount * (discountPercentage / 100);
                                        }
                                        double price = priceBeforeDiscount - discountAmount;
                                        double tax = price * 0.2;

                                        transaction.getProperty("promotionDiscount").Value = String.Format("{0:0.00}", discountAmount);
                                        transaction.getProperty("price").Value = String.Format("{0:0.00}", price);
                                        transaction.getProperty("tax").Value = String.Format("{0:0.00}", tax);
                                        transaction.getProperty("promotionCode").Value = PROMOCODE;

                                        transaction.Save();
                                    }
                                    else
                                    {
                                        Session.Add("PROMOINVALID", PROMOCODE);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Session.Add("PROMOINVALID", PROMOCODE);
                                }
                            }
                        }
                    }
                    Redirect(VUIfunctions.VUI_checkout_page);
                }
                else
                {
                    log.Debug("NOT LOGGED IN!");
                    Redirect(VUIfunctions.VUI_loginregister_page);
                }
            }


            // CHECKOUT is triggered when the PayPal Button on the Checkout page is clicked.
            if (ACTION.Equals("PAYPAL"))
            {
                log.Debug("STARTING REDIRECT TO PAYPAL");

                if (m == null)
                {
                    log.Debug("NOT LOGGED IN!");
                    Redirect(VUIfunctions.VUI_loginregister_page);
                }
                else
                {
                    string user_status = VUIfunctions.MemberVUIStatus(m);
                    log.Debug("USER STATUS " + user_status);

                    // THe user is already a VUI User or Administrator (Full Mode)
                    if (bDenyIfLoggedIn && VUIfunctions.MemberVUIFullMode(user_status))
                    {
                        Redirect("/vui");
                    }
                    // The user has registered / logged in but is not a VUI subscriber yet.
                    // Keep going
                    else
                    {

                        if(Session["VUITXID"] == null || String.IsNullOrEmpty(Session["VUITXID"].ToString()))
                        {
                            log.Debug("NO TRANSACTION NUMBER SPECIFIED");
                            Redirect(VUIfunctions.VUI_subscribe_page);
                        }

                        // Retreive the Transaction Document ID
                        // Then initiate variables used in the PayPal form
                        int txDocid = -1;
                        Int32.TryParse(Session["VUITXID"].ToString(), out txDocid);

                        log.Debug("Transaction document:" + txDocid);

                        // Instantly Remove the Session Variable. No refreshing!
                        Session.Remove("VUITXID");

                        string transactionNumber = String.Empty;
                        string price = String.Empty;
                        string tax = String.Empty;
                        string productName = String.Empty;
                        string productID = String.Empty;
                        string custom = String.Empty;
                        string currency = String.Empty;

                        Document transaction = new Document(txDocid);
                        transactionNumber = transaction.getProperty("transactionNumber").Value.ToString();
                        productName = transaction.getProperty("productName").Value.ToString();
                        productID = transaction.getProperty("productID").Value.ToString();
                        price = transaction.getProperty("price").Value.ToString();
                        tax = transaction.getProperty("tax").Value.ToString();
                        currency = transaction.getProperty("currency").Value.ToString();
                        if (String.IsNullOrEmpty(currency))
                        {
                            currency = "GBP";
                        }
                        // THis is our reference, passed back from PayPal to us which allows us to process the transaction
                        custom = txDocid.ToString();


                        // Update the status message
                        transaction.getProperty("status").Value = "TO PAYPAL";
                        transaction.Save();

                        // Update the User's personal log:
                        string userTransactions = m.getProperty("userTransactionLog").Value.ToString().Replace("{", "").Replace("}", "");
                        StringBuilder sb = new StringBuilder("");
                
                        int itemid = 0;
                        int sortOrder = 0;
                        if(!String.IsNullOrEmpty(userTransactions))
                        {
                         
                            XmlDocument xml = new XmlDocument();
                            xml.LoadXml(userTransactions);

                            XmlNodeList transList;
                            transList = xml.SelectSingleNode("items").SelectNodes("item");
                    
                            foreach (XmlNode item in transList)
                            {
                                sb.Append(item.OuterXml);
                                itemid = Int32.Parse(item.Attributes["id"].Value);
                                sortOrder = Int32.Parse(item.Attributes["sortOrder"].Value);
                            }
                        }
                        itemid++; sortOrder++;

                        sb.Append(@"<item id=""" + itemid + @""" sortOrder=""" + sortOrder + @""">");
                        sb.Append(@"<transactionNumber nodeName=""Transaction Number"" nodeType=""-51"">" + transactionNumber + @"</transactionNumber>");
                        sb.Append(@"<status nodeName=""Status"" nodeType=""-88"">TO PAYPAL</status>");
                        sb.Append(@"<transactionDate nodeName=""Status"" nodeType=""-36"">" + DateTime.Now.ToString() + "</transactionDate>");
                        sb.Append(@"</item>");
                        m.getProperty("userTransactionLog").Value = @"<items>" + sb.ToString() + @"</items>";
                        m.Save();

                        
                        log.Debug("Transaction updated and form ready to be generated");
                        
                        Document formCode = new Document(VUIfunctions.VUI_paypal_form);
                        string formHTML = formCode.getProperty("code").Value.ToString();
                        if (VUIfunctions.VUI_paypal_use_sandbox.Equals("YES"))
                        {
                            formHTML = formHTML.Replace("#SANDBOX#", "sandbox.");
                        }
                        else
                        {
                            formHTML = formHTML.Replace("#SANDBOX#", "");
                        }
                        formHTML = formHTML.Replace("#PAYPAL_AC#",VUIfunctions.VUI_paypal_ac);
                        formHTML = formHTML.Replace("#PRODUCT_NAME#",productName);
                        formHTML = formHTML.Replace("#PRODUCT_NUMBER#",productID);
                        formHTML = formHTML.Replace("#PRICE#",price);
                        formHTML = formHTML.Replace("#GBP#", currency);
                        formHTML = formHTML.Replace("#TAX#",tax);
                        formHTML = formHTML.Replace("#RETURNURL#",VUIfunctions.VUI_paypal_return);
                        formHTML = formHTML.Replace("#NOTIFYURL#",VUIfunctions.VUI_paypal_notify_url);
                        formHTML = formHTML.Replace("#TRANSACTIONNUMBER#", txDocid.ToString()); // The Transaction Document ID is passed through as a return parameter
                        formHTML = formHTML.Replace("#CUSTOM#", transactionNumber + ";" + txDocid);

                        litPayPalForm.Text = formHTML;

                        log.Debug("SENDING TO PAYPAL " + m.LoginName + " #" + transactionNumber);
                        PayPalForm.Visible = true;
                    }
                }
            }

        }


        protected void Redirect(string page)
        {
            log.Debug("REDIRECTING TO: " + page);
            Response.Redirect(page);
            Response.End();
        }

        protected void RedirectToPayPal()
        {

        }

        protected void InitiateBuy()
        {

        }

        protected void IsInBuyProcess()
        {

        }

        protected void DetermineProduct()
        {

        }


    }
}