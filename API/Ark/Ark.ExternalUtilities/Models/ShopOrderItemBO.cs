using System;
using System.Collections.Generic;
using System.Text;

namespace Ark.ExternalUtilities.Models
{
   public class ShopOrderItemBO
    {
        public string RawBase64 { get; set; }
        public string OrderID { get; set; }
        public string RawDetails { get; set; }
        public string Status { get; set; }
        public long ShopUserId { get; set; }
        public string ResponseCode { get; set; }
        public string TransactionType { get; set; }
    }
}
