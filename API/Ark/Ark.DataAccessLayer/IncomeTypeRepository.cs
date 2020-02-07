using System;
using System.Text;
using System.Linq;
using System.Security.Cryptography;
using Ark.Entities.DTO;
using Ark.Entities.BO;
using Ark.Entities.Enums;
using Ark.DataAccessLayer;
using System.Collections.Generic;

namespace Ark.DataAccessLayer
{
   public class IncomeTypeRepository
    {
        public bool Create()
        {
            throw new NotImplementedException();
        }
        public TblIncomeType Get(TblIncomeType incomeTypeQuery, dbWorldCCityContext db)
        {
            TblIncomeType incomeType = db.TblIncomeType.FirstOrDefault(item => item.Id == incomeTypeQuery.Id || item.IncomeShortName == incomeTypeQuery.IncomeShortName);
            return incomeType;
        }
    }
}
