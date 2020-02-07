using System;
using System.Collections.Generic;
using System.Text;
using Ark.Entities.DTO;

namespace Ark.Entities.BO
{
   public class UserBusinessPackageBO : TblUserAuth
    {
        public string FromCurrencyIso3{ get; set; }
        public string FromWalletCode { get; set; }
        public string AmountPaid { get; set; }
        public string PaymentAddress { get; set; }
        public string BusinessPackageID { get; set; }
    }
}
