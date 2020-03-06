using Ark.Entities.DTO;
using Ark.Entities.BO;
using Ark.Entities.Enums;
using Ark.DataAccessLayer;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Ark.ExternalUtilities;
using Ark.ExternalUtilities.Models;

namespace Ark.AppService
{
   public class ShopAppService
    {
        public HttpResponseBO UpdateUserWallet(ShopUserCommissionItemBO shopUser)
        {
            HttpUtilities httpUtilities = new HttpUtilities();
            HttpResponseBO _res = httpUtilities.PostAsyncXForm(new Uri("http://localhost/"), "wallet_update", shopUser).Result;

            return _res;
        }
    }
}
