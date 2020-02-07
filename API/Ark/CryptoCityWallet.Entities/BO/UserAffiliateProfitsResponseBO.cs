using Ark.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ark.Entities.BO
{
   public class UserAffiliateProfitsResponseBO : UserResponseBO
    {
        public List<TblDividend> TradeTransactions { get; set; }
    }
}
