using Ark.Entities.DTO;
using Ark.Entities.BO;
using Ark.Entities.Enums;
using Ark.DataAccessLayer;
using System.Collections.Generic;
using System;
using SendGrid;
using System.Threading.Tasks;

namespace Ark.AppService
{
   public class ExternalRecordsAppService
    {
        public int GetDirectPartners(TblUserAuth userAuth, dbWorldCCityContext db = null)
        {
            if (db != null)
            {
                ExternalRecordsRepository externalRecordsRepository = new ExternalRecordsRepository();
                return externalRecordsRepository.GetDirectPartners(userAuth, db);
            }
            else
            {
                using (db = new dbWorldCCityContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        ExternalRecordsRepository externalRecordsRepository = new ExternalRecordsRepository();
                        return externalRecordsRepository.GetDirectPartners(userAuth, db);
                    }
                }
            }
        }

        public decimal GetInvestmentSum(TblUserAuth userAuth, dbWorldCCityContext db = null)
        {
            if (db != null)
            {
                ExternalRecordsRepository externalRecordsRepository = new ExternalRecordsRepository();
                return externalRecordsRepository.GetInvestmentSum(userAuth, db);
            }
            else
            {
                using (db = new dbWorldCCityContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        ExternalRecordsRepository externalRecordsRepository = new ExternalRecordsRepository();
                        return externalRecordsRepository.GetInvestmentSum(userAuth, db);
                    }
                }
            }
        }
    }
}
