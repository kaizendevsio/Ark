using System;
using System.Collections.Generic;
using System.Text;

namespace Ark.ExternalUtilities.Models
{
   public class PaynamicsRequestForm
    {
        public string paymentrequest { get; set; }
        public string RequestUrl { get; set; }
        public string JsonData { get; set; }
    }
}
