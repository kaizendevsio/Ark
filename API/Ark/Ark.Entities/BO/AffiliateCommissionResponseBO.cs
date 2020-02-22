using System;
using System.Collections.Generic;
using System.Text;

namespace Ark.Entities.BO 
{
   public class AffiliateCommissionResponseBO : ApiResponseBO
    {
        public List<ShopUserCommissionItemBO> Commission { get; set; }
    }
}
