using System;
using System.Collections.Generic;
using System.Text;

namespace Ark.ExternalUtilities.Models
{
   public class PaynamicsSettings
    {
        public Uri ApiUrl_Test { get; set; }
        public Uri ApiUrl_Production { get; set; }
        public string Merchant_ID { get; set; }
        public string Merchant_Key { get; set; }
        public string Notification_URL { get; set; }
        public string Response_URL { get; set; }
        public string Cancel_URL { get; set; }
    }
}
