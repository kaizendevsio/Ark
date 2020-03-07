using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Ark.ExternalUtilities.Models;
using Newtonsoft.Json;

namespace Ark.ExternalUtilities
{
   public class Paynamics
    {
        public PaynamicsSettings GetSettings()
        {
            PaynamicsSettings paynamicsSettings = new PaynamicsSettings();
            paynamicsSettings.Merchant_ID = "000000161119730B1C0B";
            paynamicsSettings.Merchant_Key = "5EAC854C2266830C84D004C329B9B653";
            paynamicsSettings.Notification_URL = "https://testpti.payserv.net/webpayment/default.aspx";
            paynamicsSettings.Response_URL = "https://testpti.payserv.net/webpayment/default.aspx";
            paynamicsSettings.Cancel_URL = "https://testpti.payserv.net/webpayment/default.aspx";
            paynamicsSettings.ApiUrl_Test = new Uri("https://testpti.payserv.net/webpayment/default.aspx");
            paynamicsSettings.ApiUrl_Production = new Uri("https://ptiapps.paynamics.net/webpayment/Default.aspx");

            return paynamicsSettings;
        }

        public HttpResponseBO CreateRequest(PaynamicsRequest _paynamicsRequest)
        {
            using (SHA512 shaM = new SHA512Managed())
            {
                PaynamicsSettings paynamicsSettings = GetSettings();

                PaynamicsRequest PaynamicsRequest = new PaynamicsRequest();
                _paynamicsRequest.Mid = paynamicsSettings.Merchant_ID;
                _paynamicsRequest.Request_id = "2851306488";
                _paynamicsRequest.Notification_url = paynamicsSettings.Notification_URL;
                _paynamicsRequest.Response_url = paynamicsSettings.Response_URL;
                _paynamicsRequest.Cancel_url = paynamicsSettings.Cancel_URL;
                _paynamicsRequest.Secure3d = "try3d";
                _paynamicsRequest.Trxtype = "sale";
                _paynamicsRequest.Currency = "PHP";


                string data = String.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}{19}{20}", _paynamicsRequest.Mid, _paynamicsRequest.Request_id, _paynamicsRequest.Ip_address, _paynamicsRequest.Notification_url, _paynamicsRequest.Response_url, _paynamicsRequest.Fname, _paynamicsRequest.Lname, _paynamicsRequest.Mname, _paynamicsRequest.Address1, _paynamicsRequest.Address2, _paynamicsRequest.City, _paynamicsRequest.State, _paynamicsRequest.Country, _paynamicsRequest.Zip, _paynamicsRequest.Email, _paynamicsRequest.Phone, _paynamicsRequest.Client_ip, _paynamicsRequest.Amount, _paynamicsRequest.Currency, _paynamicsRequest.Secure3d, paynamicsSettings.Merchant_Key);

                var hash = shaM.ComputeHash(Encoding.UTF8.GetBytes(data));
                string hashString = Encoding.Default.GetString(hash);

                _paynamicsRequest.Signature = hashString;

                string _xml = XmlSerialize(_paynamicsRequest);
                PaynamicsRequestForm paynamicsRequestForm = new PaynamicsRequestForm
                {
                    paymentrequest = Base64Encode(_xml)
                };

                HttpUtilities httpUtilities = new HttpUtilities();
                HttpResponseBO _res = httpUtilities.PostAsyncXForm(paynamicsSettings.ApiUrl_Test, "Default.aspx" ,paynamicsRequestForm).Result;
                return _res;
            }
            
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public string XmlSerialize(PaynamicsRequest obj)
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(PaynamicsRequest));
            var subReq = new PaynamicsRequest();
            var xml = "";

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, subReq);
                    xml = sww.ToString(); // Your XML

                    return xml;
                }
            }
        }

    }
}
