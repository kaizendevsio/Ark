using Ark.Entities.DTO;
using Ark.Entities.BO;
using Ark.Entities.Enums;
using Ark.DataAccessLayer;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace Ark.AppService
{
   public class UserAffiliateAppService
    {
        public List<TblDividend> GetAllTradeProfits(TblUserAuth userAuth, dbWorldCCityContext db = null)
        {
            if (db != null)
            {
                UserDividendRepository userDividendRepository = new UserDividendRepository();
                return userDividendRepository.GetAll(userAuth, db);
            }
            else
            {
                using (db = new dbWorldCCityContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        UserDividendRepository userDividendRepository = new UserDividendRepository();
                        return userDividendRepository.GetAll(userAuth, db);
                    }
                }
            }
        }

    }
}
