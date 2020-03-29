using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Ark.ExternalUtilities.Models;
using Ark.ExternalUtilities.Cryptography;
using Newtonsoft.Json;

namespace Ark.ExternalUtilities
{
    public class Paynamics
    {
        public PaynamicsEnvironment GetSettings()
        {

            PaynamicsEnvironment paynamicsEnvironment = new PaynamicsEnvironment
            {
                Production = new PaynamicsSettings
                {
                    Merchant_ID = "00000005032065BA9A30",
                    Merchant_Key = "44C7651DAED39DBB403F73C32141506B",
                    ApiUrl = new Uri("https://ptiapps.paynamics.net/webpayment/"),
                    Cancel_URL = "http://ark.com.ph/payment/cancel",
                    Notification_URL = "http://ark.com.ph/payment/callback",
                    Response_URL = "http://ark.com.ph/payment/response",
                    IPAddress = "52.55.94.8"
                },
                Staging = new PaynamicsSettings {
                    Merchant_ID = "00000005032065BA9A30",
                    Merchant_Key = "44C7651DAED39DBB403F73C32141506B",
                    ApiUrl = new Uri("https://ptiapps.paynamics.net/webpayment/"),
                    Cancel_URL = "http://nightly.ark.com.ph/payment/cancel",
                    Notification_URL = " http://nightly.ark.com.ph/payment/callback",
                    Response_URL = "http://nightly.ark.com.ph/payment/response",
                    IPAddress = "34.199.15.20"
                },
                Development = new PaynamicsSettings {
                    Merchant_ID = "000000161119730B1C0B",
                    Merchant_Key = "5EAC854C2266830C84D004C329B9B653",
                    ApiUrl = new Uri("https://testpti.payserv.net/webpayment/"),
                    Cancel_URL = "http://localhost/payment/cancel",
                    Notification_URL = "http://localhost/payment/callback",
                    Response_URL = "http://localhost/payment/response",
                    IPAddress = "34.199.15.20"
                }
            };
           
            return paynamicsEnvironment;
        }

        public PaynamicsRequestForm CreateRequest(PaynamicsRequest _paynamicsRequest, string Environment)
        {
            SHA _SHA = new SHA();

            PaynamicsEnvironment paynamicsEnvironment = GetSettings();
            PaynamicsSettings paynamicsSettings = ApiUriInit(paynamicsEnvironment, Environment);

            SeedString seedString = new SeedString();
            Guid g = Guid.NewGuid();

            PaynamicsRequest PaynamicsRequest = new PaynamicsRequest();
            _paynamicsRequest.Mid = paynamicsSettings.Merchant_ID;
            //_paynamicsRequest.Request_id = seedString.GenerateRandom(30);
            _paynamicsRequest.Notification_url = paynamicsSettings.Notification_URL;
            _paynamicsRequest.Response_url = paynamicsSettings.Response_URL;
            _paynamicsRequest.Cancel_url = paynamicsSettings.Cancel_URL;
            _paynamicsRequest.Ip_address = paynamicsSettings.IPAddress;
            _paynamicsRequest.Secure3d = "try3d";
            _paynamicsRequest.Trxtype = "sale";
            _paynamicsRequest.Currency = "PHP";

            PaynamicsSignatureBO paynamicsSignature = new PaynamicsSignatureBO {
                Mid = _paynamicsRequest.Mid,
                Request_id = _paynamicsRequest.Request_id,
                Ip_address = _paynamicsRequest.Ip_address,
                Notification_url = _paynamicsRequest.Notification_url,
                Response_url = _paynamicsRequest.Response_url,
                Fname = _paynamicsRequest.Fname,
                Lname = _paynamicsRequest.Lname,
                Mname = _paynamicsRequest.Mname,
                Address1 = _paynamicsRequest.Address1,
                Address2 = _paynamicsRequest.Address2,
                City = _paynamicsRequest.City,
                State = _paynamicsRequest.State,
                Country = _paynamicsRequest.Country,
                Zip = _paynamicsRequest.Zip,
                Email = _paynamicsRequest.Email,
                Phone = _paynamicsRequest.Phone,
                Client_ip = _paynamicsRequest.Client_ip,
                Amount = _paynamicsRequest.Amount,
                Currency = _paynamicsRequest.Currency,
                Secure3d = _paynamicsRequest.Secure3d,
                Merchant_Key = paynamicsSettings.Merchant_Key
            };

            string data = paynamicsSignature.ToString();
            string hashString = _SHA.GenerateSHA512String(data);

            _paynamicsRequest.Signature = hashString;



            string _xml = XmlSerialize(_paynamicsRequest);
            PaynamicsRequestForm paynamicsRequestForm = new PaynamicsRequestForm
            {
                paymentrequest = Base64Encode(_xml),
                RequestUrl = String.Format("{0}{1}", paynamicsSettings.ApiUrl.AbsoluteUri, "Default.aspx"),
                JsonData = JsonConvert.SerializeObject(_paynamicsRequest)
            };

            //HttpUtilities httpUtilities = new HttpUtilities();
            //HttpResponseBO _res = httpUtilities.PostAsyncXForm(paynamicsSettings.ApiUrl_Production, "Default.aspx", paynamicsRequestForm).Result;
            return paynamicsRequestForm;


        }

        public ShopOrderItemBO ProcessCallbackRequest(string paynamicsResponseString, string Environment)
        {
            PaynamicsResponse paynamicsResponse = XmlDeserialize(Base64Decode(paynamicsResponseString));
            ShopOrderItemBO shopOrderItem = new ShopOrderItemBO();
            shopOrderItem.RawDetails = JsonConvert.SerializeObject(paynamicsResponse);
            shopOrderItem.OrderID = paynamicsResponse.Application.Request_id;
            shopOrderItem.ResponseCode = paynamicsResponse.ResponseStatus.Response_code;

            return shopOrderItem;
        }

        public PaynamicsSettings ApiUriInit(PaynamicsEnvironment paynamicsEnvironment, string Environment)
        {
            PaynamicsSettings paynamicsSettings = new PaynamicsSettings();
            switch (Environment)
            {
                case "Production":
                    paynamicsSettings = paynamicsEnvironment.Production;
                    break;
                case "Development":
                    paynamicsSettings = paynamicsEnvironment.Development;
                    break;
                case "Staging":
                    paynamicsSettings = paynamicsEnvironment.Staging;
                    break;
                default:
                    break;
            }
            
            return paynamicsSettings;

        }

        public string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public string XmlSerialize(PaynamicsRequest subReq)
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(PaynamicsRequest));
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

        public PaynamicsResponse XmlDeserialize(string xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PaynamicsResponse));

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {

                    using (var reader = new StringReader(xmlString))
                    {
                        PaynamicsResponse paynamicsResponse = (PaynamicsResponse)serializer.Deserialize(reader);
                        return paynamicsResponse;
                    }
                }
            }
        }

    }
}
