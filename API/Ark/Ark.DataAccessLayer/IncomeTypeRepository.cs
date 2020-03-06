using System;
using System.Text;
using System.Linq;
using System.Security.Cryptography;
using Ark.Entities.DTO;
using Ark.Entities.BO;
using Ark.Entities.Enums;
using Ark.DataAccessLayer;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Ark.DataAccessLayer
{
   public class IncomeTypeRepository
    {
        public bool Create()
        {
            throw new NotImplementedException();
        }
        public TblIncomeType Get(TblIncomeType incomeTypeQuery, ArkContext db)
        {
            TblIncomeType incomeType = db.TblIncomeType.AsNoTracking().FirstOrDefault(item => item.Id == incomeTypeQuery.Id || item.IncomeTypeShortName == incomeTypeQuery.IncomeTypeShortName);
            return incomeType;
        }

        public TblIncomeDistribution GetDistribution(IncomeType incomeType, TblBusinessPackage businessPackage, ArkContext db)
        {
            var _qUi = from a in db.TblIncomeDistribution
                       join b in db.TblBusinessPackage on a.BusinessPackageId equals b.Id
                       join c in db.TblIncomeType on a.IncomeTypeId equals c.Id

                       where c.IncomeTypeCode == incomeType && a.BusinessPackageId == businessPackage.Id

                       orderby a.CreatedOn descending
                       select new TblIncomeDistribution
                       {
                           Id = a.Id,
                           CreatedOn = a.CreatedOn,
                           IsEnabled = a.IsEnabled,
                           BusinessPackage = b,
                           BusinessPackageId = a.BusinessPackageId,
                           DistributionType = a.DistributionType,
                           IncomeTypeId = a.IncomeTypeId,
                           IncomeType = c,
                           Value = a.Value
                       };

            TblIncomeDistribution _ubp = _qUi.AsNoTracking().FirstOrDefault();

            return _ubp;
        }
    }
}
