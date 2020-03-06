using System;
using System.Collections.Generic;
using System.Text;

namespace Ark.Entities.BO
{
   public class ShopUserCommissionItemBO
    {
        public long ShopUserId { get; set; }
        public decimal Reward { get; set; }
        public string Remarks { get; set; }
    }
}
