using System;
using System.Collections.Generic;
using System.Text;

namespace Ark.ExternalUtilities.Models
{
   public class PaynamicsSignatureBO : PaynamicsRequest
    {
        public string Merchant_Key { get; set; }
        public override string ToString() => $"{Mid}{Request_id}{Ip_address}{Notification_url}{Response_url}{Fname}{Lname}{Mname}{Address1}{Address2}{City}{State}{Country}{Zip}{Email}{Phone}{Client_ip}{Amount}{Currency}{Secure3d}{Merchant_Key}";
    }
}
