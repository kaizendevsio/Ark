using Ark.Entities.DTO;
using Ark.Entities.BO;
using Ark.Entities.Enums;
using Ark.DataAccessLayer;
using System.Collections.Generic;
using System;

namespace Ark.AppService
{
   public class UserIncomeAppService
    {
        public bool ExecuteIncomeDistribution(TblUserAuth userAuth, TblUserBusinessPackage userBusinessPackage, decimal amountPaid, ArkContext db)
        {
            BusinessPackageRepository businessPackageRepository = new BusinessPackageRepository();
            IncomeTypeRepository incomeTypeRepository = new IncomeTypeRepository();
            List<TblIncomeType> incomeTypes = businessPackageRepository.GetIncomeTypes(userBusinessPackage.BusinessPackage, db);


            foreach (var incomeType in incomeTypes)
            {
                switch (incomeType.IncomeTypeCode)
                {
                    case IncomeType.DSI:
                        DirectIncome(userAuth, incomeTypeRepository.GetDistribution(incomeType.IncomeTypeCode, userBusinessPackage.BusinessPackage, db), amountPaid, db);
                        break;
                    case IncomeType.USI:
                        UniLevelIncome(userAuth, incomeTypeRepository.GetDistribution(incomeType.IncomeTypeCode, userBusinessPackage.BusinessPackage, db), amountPaid, db);
                        break;
                    case IncomeType.MSI:
                        MatchingIncome(userAuth, incomeTypeRepository.GetDistribution(incomeType.IncomeTypeCode, userBusinessPackage.BusinessPackage, db), amountPaid, db);
                        break;
                    case IncomeType.TMI:
                        TrimatchSalesIncome(userAuth, incomeTypeRepository.GetDistribution(incomeType.IncomeTypeCode, userBusinessPackage.BusinessPackage, db), amountPaid, db);
                        break;
                    case IncomeType.GSI:
                        GlobalSalesIncome(userAuth, incomeTypeRepository.GetDistribution(incomeType.IncomeTypeCode, userBusinessPackage.BusinessPackage, db), amountPaid, db);
                        break;
                    default:
                        break;
                }
            }
            return true;
        }
        private bool DirectIncome(TblUserAuth userAuth,TblIncomeDistribution incomeDistribution, decimal amountPaid, ArkContext db)
        {
            UserAuthRepository userAuthRepository = new UserAuthRepository();
            UserWalletAppService userWalletAppService = new UserWalletAppService();
            UserMapRepository userMapRepository = new UserMapRepository();
            UserIncomeTransactionRepository userIncomeTransactionRepository = new UserIncomeTransactionRepository();

            userAuth = userAuthRepository.GetByID(userAuth.Id, db);
            TblUserMap userMap = userMapRepository.Get(userAuth, db);
            CalculateIncomeBO calculateIncome = CalculateIncome(incomeDistribution, amountPaid);

            UserWalletBO userWallet = new UserWalletBO {
                UserAuthId = (long)userMap.SponsorUserId,
                WalletTypeId = (long)WalletTypeID.ArkCash
            };

            WalletTransactionBO walletTransaction = new WalletTransactionBO
            {
                From = userAuth.Id.ToString(),
                Amount = calculateIncome.IncomeAmount,
                Remarks = calculateIncome.Remarks
             };

            userIncomeTransactionRepository.Create(userWallet, walletTransaction, incomeDistribution, db);
            userWalletAppService.Increment(userWallet,walletTransaction,db);

            return true;
        }
        private bool UniLevelIncome(TblUserAuth userAuth, TblIncomeDistribution incomeDistribution, decimal amountPaid, ArkContext db)
        {
            throw new NotImplementedException();
        }
        private bool MatchingIncome(TblUserAuth userAuth, TblIncomeDistribution incomeDistribution, decimal amountPaid, ArkContext db)
        {
            throw new NotImplementedException();
        }
        private bool GlobalSalesIncome(TblUserAuth userAuth, TblIncomeDistribution incomeDistribution, decimal amountPaid, ArkContext db)
        {
            throw new NotImplementedException();
        }
        private bool TrimatchSalesIncome(TblUserAuth userAuth, TblIncomeDistribution incomeDistribution, decimal amountPaid, ArkContext db)
        {

        }
        private CalculateIncomeBO CalculateIncome(TblIncomeDistribution incomeDistribution, decimal amountPaid)
        {
            CalculateIncomeBO calculateIncome = new CalculateIncomeBO(); ;

            switch (incomeDistribution.DistributionType)
            {
                case IncomeDistributionType.Fixed:
                    calculateIncome.IncomeAmount = incomeDistribution.Value;
                    calculateIncome.Remarks = String.Format("Value = ({0})", incomeDistribution.Value);
                    break;
                case IncomeDistributionType.Percentage:
                    calculateIncome.IncomeAmount = incomeDistribution.Value * amountPaid;
                    calculateIncome.Remarks = String.Format("Value = ({0}) * ({1})", incomeDistribution.Value, amountPaid);
                    break;
                default:
                    break;
            }
            return calculateIncome;
        }
    }
}
