using System.Text;
using System.Linq;
using System.Security.Cryptography;
using Ark.Entities.DTO;
using Ark.Entities.BO;
using Ark.Entities.Enums;
using AutoMapper;
using System;

namespace Ark.DataAccessLayer
{
   public class UserIncomeTransactionRepository
    {
        public TblUserIncomeTransaction Create(UserWalletBO userWallet, WalletTransactionBO walletTransaction, TblIncomeDistribution incomeDistribution, DataAccessLayer.ArkContext db)
        {
            TblUserIncomeTransaction userIncomeTransaction = new TblUserIncomeTransaction
            {
                UserAuthId = userWallet.UserAuthId,
                CreatedOn = DateTime.Now,
                IncomeTypeId = incomeDistribution.IncomeTypeId,
                TriggeredByUbpId = long.Parse(walletTransaction.From),
                TransactionType = (short)TransactionType.Received,
                IncomeStatus = (short)TransactionStatus.Completed
            };

            db.TblUserIncomeTransaction.Add(userIncomeTransaction);

            db.SaveChanges();

            return userIncomeTransaction;

        }
    }
}
