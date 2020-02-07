﻿using System;
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
   public class UserDividendRepository
    {
        public List<TblDividend> GetAll(TblUserAuth userAuth, dbWorldCCityContext db)
        {
            var _q = from a in db.TblDividend
                     join b in db.TblUserAuth on a.DividendUserAuthId equals b.Id
                     where a.DividendUserAuthId == userAuth.Id

                     orderby a.DividendDate descending
                     select new TblDividend
                     {
                         Id = a.Id,
                         CreatedOn = a.CreatedOn,
                         DividendBonusCd = a.DividendBonusCd,
                         DividendCd = a.DividendCd,
                         DividendCloseCd = a.DividendCloseCd,
                         DividendDate = a.DividendDate,
                         DividendDateRegistered = a.DividendDateRegistered,
                         DividendOrderCd = a.DividendOrderCd,
                         DividendPrice = a.DividendPrice,
                         DividendRankCd = a.DividendRankCd,
                         DividendRate = a.DividendRate,
                         DividendUserAuthId = a.DividendUserAuthId,
                         DividendUserCd = a.DividendUserCd,
                         IsEnabled = a.IsEnabled
                     };

            List<TblDividend> dividends = _q.ToList<TblDividend>();

            return dividends;
        }
    }
}
