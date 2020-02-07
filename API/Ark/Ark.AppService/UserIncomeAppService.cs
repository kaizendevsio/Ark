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
        public bool DirectRewards(TblUserAuth userAuth, dbWorldCCityContext db)
        {
            UserWalletRepository userWalletRepository = new UserWalletRepository();
            IncomeTypeRepository incomeTypeRepository = new IncomeTypeRepository();
            UserMapRepository userMapRepository = new UserMapRepository();

            UserWalletAppService userWalletAppService = new UserWalletAppService();

            TblUserMap userMap = new TblUserMap();
            userMap.Id = userAuth.Id;
            //userMap = userMapRepository.Get(userMap, db);

            UserWalletBO userWallet = new UserWalletBO();
            WalletTransactionBO walletTransaction = new WalletTransactionBO();

            walletTransaction.Amount = 0m;
            userWallet.UserAuthId = userMap.Id;
            //userWallet.WalletTypeId

            userWalletAppService.Increment(userWallet,walletTransaction,db);

            return true;
        }
        public bool UniLevelRewards()
        {
            throw new NotImplementedException();
        }
        public bool BinaryRewards()
        {
            throw new NotImplementedException();
        }
        public bool GlobalSalesRewards()
        {
            throw new NotImplementedException();
        }
    }
}
