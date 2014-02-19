using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using umbraco.cms.businesslogic.member;
using umbraco.cms.businesslogic.web;
using umbraco.MacroEngines;
using VUI.classes;
using System.Web.Security;
using System.Xml.XPath;

namespace VUI.usercontrols
{
    public partial class paypal_ipn_processor : System.Web.UI.UserControl
    {

        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(vui_subscribe_process));

        protected void Page_Load(object sender, EventArgs e)
        {
            //Post back to either sandbox or live
            string strSandbox = "https://www.sandbox.paypal.com/cgi-bin/webscr";
            string strLive = "https://www.paypal.com/cgi-bin/webscr";
            HttpWebRequest req;
            if (VUIfunctions.VUI_paypal_use_sandbox.Equals("YES"))
            {
                req = (HttpWebRequest)WebRequest.Create(strSandbox);
            }
            else
            {
                req = (HttpWebRequest)WebRequest.Create(strLive);
            }


            log.Debug("Processing message from PayPal");

            //Set values for the request back
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            byte[] param = Request.BinaryRead(HttpContext.Current.Request.ContentLength);
            string strRequest = Encoding.ASCII.GetString(param);

            log.Debug("Message:" + strRequest);

            strRequest += "&cmd=_notify-validate";
            req.ContentLength = strRequest.Length;


            //for proxy
            //WebProxy proxy = new WebProxy(new Uri("http://url:port#"));
            //req.Proxy = proxy;

            //Send the request to PayPal and get the response
            StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII);
            streamOut.Write(strRequest);
            streamOut.Close();


            StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
            string strResponse = streamIn.ReadToEnd();
            streamIn.Close();


            log.Debug("Response:" + strResponse);


            if (strResponse == "VERIFIED")
            {


                System.Collections.Specialized.NameValueCollection qs = HttpUtility.ParseQueryString(strRequest);

                string paymentStatus = qs.Get("payment_status");
                string reason = String.Empty;

                if (paymentStatus.Equals("Pending"))
                {
                    reason = qs.Get("pending_reason");
                    paymentStatus = paymentStatus + " " + reason;
                }

                log.Debug("Payment Status: " + paymentStatus);

                string receiver_email = qs.Get("receiver_email");
                string payer_email = qs.Get("payer_email");
                string txn_id = qs.Get("txn_id");
                string custom = qs.Get("custom");  //TtransactionNumber;transactionDocId"
                
                
                string transactionNumber = String.Empty;
                int txDocId;

                if(String.IsNullOrEmpty(custom))
                {
                    log.Debug("Can't process empty custom token");
                    return;
                }
                else
                {
                    try
                    {
                        string[] customTokens = custom.Split(';');
                        transactionNumber = customTokens[0];
                        Int32.TryParse(customTokens[1], out txDocId);

                        // Transaction
                        log.Debug("Opening transaction document:" + txDocId);
                        Document tx = new Document(txDocId);
                        int userId = (Int32)(tx.getProperty("user").Value);
                        string PRODUCTCODE = tx.getProperty("productID").Value.ToString();
                        log.Debug("Product Code:" + PRODUCTCODE);
                        tx.getProperty("status").Value = "PAYPAL " + paymentStatus;
                        tx.getProperty("payPalTXN").Value = txn_id;
                        tx.getProperty("payerEmail").Value = payer_email;
                        tx.getProperty("payPalReturnMessage").Value = strRequest;
                        tx.Save();

                        log.Debug("Saved transaction");

                        

                        // Member
                        Member m = new Member(userId);

                        // Transaction Log
                        string userTransactions = m.getProperty("userTransactionLog").Value.ToString().Replace("{", "").Replace("}", "");
                        StringBuilder sb = new StringBuilder("");

                        int itemid = 0;
                        int sortOrder = 0;
                        if (!String.IsNullOrEmpty(userTransactions))
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
                        sb.Append(@"<status nodeName=""Status"" nodeType=""-88"">PAYPAL:" + paymentStatus + @"</status>");
                        sb.Append(@"<transactionDate nodeName=""Transaction Date"" nodeType=""-36"">" + DateTime.Now.ToString() + "</transactionDate>");
                        sb.Append(@"</item>");
                        m.getProperty("userTransactionLog").Value = @"<items>" + sb.ToString() + @"</items>";

                        // If Payment is Complete, or is a multi-currency_pending
                        if (paymentStatus.Equals("Completed") || paymentStatus.Equals("Pending multi_currency"))
                        {
                            log.Debug("Getting product details: " + PRODUCTCODE);
                            DynamicNode product = (new DynamicNode(VUIfunctions.VUI_product_list)).Descendants("VUISubscriptionProduct").Items.Where(n => n.GetProperty("productCode").Value.ToUpper().Equals(PRODUCTCODE)).ToList().First();
                            log.Debug("Found product" + product.Id);

                            m.getProperty("activeSubscriptionTransaction").Value = transactionNumber;
                            m.getProperty("vuiFullyPaidUp").Value = 1;
                            m.getProperty("vuiJoinDate").Value = DateTime.Today;
                            m.getProperty("vUINumberOfUsersAllowed").Value = product.GetProperty("basicNumberOfUsers").Value;
                            m.getProperty("vuiSubscriptionPackage").Value = PRODUCTCODE + ":" + product.GetProperty("productName").Value;
                            Roles.AddUserToRole(m.LoginName, "vui_administrator");
                            log.Debug("Congratulations " + m.LoginName + " on becoming a subscriber!");

                            // TODO - SEND EMAILS and RAISE INVOICE

                            Document translog = new Document(VUIfunctions.VUI_transaction_log);
                            int invoiceNumber = (Int32)(translog.getProperty("invoiceNumber").Value);
                            invoiceNumber += 1;
                            translog.getProperty("invoiceNumber").Value = invoiceNumber;
                            translog.Save();
                            translog.Publish(VUIfunctions.u);
                            log.Debug("CYcled the Invoice number to: " + invoiceNumber);

                            // This is horrible - get the PreVal for "Paid by PayPal"
                            string status = String.Empty;
                            XPathNodeIterator statusRoot = umbraco.library.GetPreValues(VUIfunctions.VUI_invoicestatus_list);
                            statusRoot.MoveNext(); //move to first
                            XPathNodeIterator preValues = statusRoot.Current.SelectChildren("preValue", "");
                            while (preValues.MoveNext())
                            {
                                if (preValues.Current.Value.ToLower().Equals("paid by paypal"))
                                {
                                    status = preValues.Current.GetAttribute("id", "");
                                    break;
                                }
                            }


                            double price = double.Parse(tx.getProperty("price").Value.ToString());
                            double tax = double.Parse(tx.getProperty("tax").Value.ToString());

                            Document invoice = Document.MakeNew("INV-" + invoiceNumber, DocumentType.GetByAlias("VUI_Invoice"), VUIfunctions.u, VUIfunctions.VUI_transaction_log);
                            invoice.getProperty("invoiceUser").Value = m.Id;
                            invoice.getProperty("invoiceNumber").Value = invoiceNumber;
                            invoice.getProperty("invoiceStatus").Value = status;
                            invoice.getProperty("invoiceDate").Value = DateTime.Today;
                            invoice.getProperty("associatedTransaction").Value = transactionNumber;
                            invoice.getProperty("invoiceFor").Value = tx.getProperty("productName").Value.ToString();
                            
                            if (tx.getProperty("purchaseOrderNumber").Value != null)
                                invoice.getProperty("purchaseOrderNumber").Value = tx.getProperty("purchaseOrderNumber").Value.ToString();
                            
                            if (tx.getProperty("promotionCode").Value != null)
                                invoice.getProperty("promotionCode").Value = tx.getProperty("promotionCode").Value.ToString();

                            invoice.getProperty("invoiceAmountNet").Value = tx.getProperty("priceBeforeDiscount").Value.ToString();
                            invoice.getProperty("promotionDiscount").Value = tx.getProperty("promotionDiscount").Value.ToString();
                            invoice.getProperty("invoiceAmountAfterDiscount").Value = tx.getProperty("price").Value.ToString();
                            invoice.getProperty("invoiceAmountGross").Value = String.Format("{0:0.00}", price + tax);           
                            invoice.getProperty("invoiceTax").Value = tx.getProperty("tax").Value.ToString();
                            invoice.getProperty("invoiceRecipient").Value = m.getProperty("fullName").Value.ToString();
                            invoice.getProperty("invoiceCompany").Value = m.getProperty("companyName").Value.ToString();
                            string address = String.Empty;
                            address += m.getProperty("companyAddress1").Value.ToString();
                            if (!String.IsNullOrEmpty(m.getProperty("companyAddress2").Value.ToString()))
                            {
                                address += "\n" + m.getProperty("companyAddress2").Value.ToString();
                            }
                            if (!String.IsNullOrEmpty(m.getProperty("companyAddress3").Value.ToString()))
                            {
                                address += "\n" + m.getProperty("companyAddress3").Value.ToString();
                            }
                            if (!String.IsNullOrEmpty(m.getProperty("companyTown").Value.ToString()))
                            {
                                address += "\n" + m.getProperty("companyTown").Value.ToString();
                            }
                            if (!String.IsNullOrEmpty(m.getProperty("companyState").Value.ToString()))
                            {
                                address += "\n" + m.getProperty("companyState").Value.ToString();
                            }
                            if (!String.IsNullOrEmpty(m.getProperty("companyPostcodeZip").Value.ToString()))
                            {
                                address += "\n" + m.getProperty("companyPostcodeZip").Value.ToString();
                            }
                            if (!String.IsNullOrEmpty(m.getProperty("companyCountry").Value.ToString()))
                            {
                                address += "\n" + m.getProperty("companyCountry").Value.ToString();
                            }
                            invoice.getProperty("invoiceRecipientAddress").Value = address;

                            // Affiliate ID if there is one - take it from the transaction item
                            if (tx.getProperty("affiliateId").Value != null)
                            {
                                invoice.getProperty("affiliateId").Value = tx.getProperty("affiliateId").Value;
                            }



                            invoice.Save();

                            log.Debug("Saved Invoice: " + invoiceNumber);
                            invoice.Publish(VUIfunctions.u);
                            umbraco.library.UpdateDocumentCache(invoice.Id);
                            log.Debug("Published invoice: " + invoiceNumber);


                            // Add the invoice to the user log
                            string userInvoices = String.Empty;
                            if (m.getProperty("userInvoiceLog") != null)
                            {
                                userInvoices = m.getProperty("userInvoiceLog").Value.ToString().Replace("{", "").Replace("}", "");
                            }
                            sb = new StringBuilder("");

                            itemid = 0;
                            sortOrder = 0;
                            if (!String.IsNullOrEmpty(userInvoices))
                            {

                                XmlDocument xml = new XmlDocument();
                                xml.LoadXml(userInvoices);

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
                            sb.Append(@"<invoiceNumber nodeName=""Invoice Number"" nodeType=""-51"">" + invoiceNumber + @"</invoiceNumber>");
                            sb.Append(@"<invoiceDate nodeName=""Invoice Date"" nodeType=""-41"">" + DateTime.Today.ToString() + "</invoiceDate>");
                            sb.Append(@"<invoiceItem nodeName=""Invoice Item"" nodeType=""1034"">" + invoice.Id + @"</invoiceItem>");
                            sb.Append(@"</item>");
                            m.getProperty("userInvoiceLog").Value = @"<items>" + sb.ToString() + @"</items>";
                            log.Debug("Added Invoice log entry to: " + m.LoginName);

                            VUIfunctions.SendSubsConfirmEmail(m);

                        }
                        m.Save();
                        log.Debug("Member saved: " + m.LoginName);
                    }
                    catch (Exception ex)
                    {
                        log.Error("Error processing payment:",ex);

                        VUIfunctions.SendErrorEmail("Error in the PayPal subscription process: " + strRequest, ex);
                    }
                }

                /*
                 * mc_gross=3354.00&protection_eligibility=Eligible&address_status=confirmed&payer_id=PWBSZR2LUF84A&
                 * tax=559.00&address_street=1+Main+St&payment_date=04%3A49%3A51+Aug+31%2C+2012+PDT
                 * &payment_status=Pending
                 * &charset=windows-1252
                 * &address_zip=95131
                 * &first_name=Oliver
                 * &option_selection1=11-50+employees
                 * &address_country_code=US
                 * &address_name=Oliver+Wood
                 * &notify_version=3.6&custom=
                 * &payer_status=verified&
                 * business=oliver_1346325251_biz%40vodprofessional.com
                 * &address_country=United+States
                 * &address_city=San+Jose
                 * &quantity=1
                 * &verify_sign=Ap3FC719E7Kr4AYVtEg6NGgQt6O8AwMSbIpYSgHWgX4PtaBFLGXs6pSx
                 * &payer_email=nice_1345646315_per%40vodprofessional.com
                 * &option_name1=Company+size
                 * &txn_id=6LA12424N5363721F
                 * &payment_type=instant
                 * &last_name=Wood
                 * &address_state=CA
                 * &receiver_email=oliver_1346325251_biz%40vodprofessional.com
                 * &receiver_id=9WZPQ6XAT9UWE
                 * &pending_reason=multi_currency
                 * &txn_type=web_accept
                 * &item_name=VUI+Library+Subscription+%2812+Months%29
                 * &mc_currency=GBP
                 * &item_number=VUI001
                 * &residence_country=US
                 * &test_ipn=1
                 * &handling_amount=0.00
                 * &transaction_subject=VUI+Library+Subscription+%2812+Months%29&payment_gross=&shipping=0.00
                 * &ipn_track_id=b161232692cfa
                 */
                
                /*  txn_id = 61E67681CH3238416
                    payer_email = gm_1231902590_per@paypal.com
                    payer_id = LPLWNMTBWMFAY
                    payer_status = verified
                    first_name = Test
                    last_name = User
                    address_city = San Jose
                    address_country = United States
                    address_country_code = US
                    address_name = Test User
                    address_state = CA
                    address_status = confirmed
                    mc_fee = 0.88
                    mc_gross = 19.95
                    payment_date = 20:12:59 Jan 13, 2009 PST
                    payment_fee = 0.88
                    payment_gross = 19.95
                    payment_status = Completed Status, which determines whether the transaction is complete
                    payment_type = instant Kind of payment
                    protection_eligibility = Eligible
                    quantity = 1
                    shipping = 0.00
                    tax = 0.00
                    custom = Your custom field
                    handling_amount = 0.00
                    item_name =
                    item_number =
                    mc_currency = USD
                    mc_fee = 0.88
                    mc_gross = 19.95
                    payment_date = 20:12:59 Jan 13, 2009 PST
                    payment_fee = 0.88
                    payment_gross = 19.95
                    payment_status = Completed Status, which determines whether the transaction is
                    complete
                    payment_type = instant Kind of payment
                    protection_eligibility = Eligible
                    quantity = 1
                 */



            }
            else if (strResponse == "INVALID")
            {
                log.Debug(strResponse + " Something went wrong with the IPN conversation");
            }
            else
            {
                //log response/ipn data for manual investigation
                log.Debug(strResponse + " Something went wrong with the IPN conversation");
            }
        }
    }
}

