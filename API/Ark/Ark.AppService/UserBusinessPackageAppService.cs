using System;
using System.Collections.Generic;
using System.Text;
using Ark.Entities.DTO;
using Ark.Entities.BO;
using Ark.Entities.Enums;
using Ark.DataAccessLayer;

namespace Ark.AppService
{
   public class UserBusinessPackageAppService
    {
        public bool Create(UserBusinessPackageBO userBusinessPackage, ArkContext db = null)
        {
            if (db != null)
            {
                using var transaction = db.Database.BeginTransaction();
                WalletTypeRepository walletTypeRepository = new WalletTypeRepository();
                TblWalletType walletType = walletTypeRepository.Get(new UserWalletBO { WalletCode = userBusinessPackage.FromWalletCode, WalletTypeId = 0 }, db);

                UserWalletAppService userWalletAppService = new UserWalletAppService();
                UserWalletBO userWallet = userWalletAppService.GetBO(new UserWalletBO { UserAuthId = userBusinessPackage.Id, WalletTypeId = walletType.Id }, db);

                CurrencyTypeRepository currencyTypeRepository = new CurrencyTypeRepository();
                TblCurrency currency = currencyTypeRepository.Get(new TblCurrency { CurrencyIsoCode3 = userBusinessPackage.FromCurrencyIso3 }, db);

                BusinessPackageRepository businessPackageRepository = new BusinessPackageRepository();
                TblBusinessPackage businessPackage = businessPackageRepository.Get(new TblBusinessPackage { Id = long.Parse(userBusinessPackage.BusinessPackageID) }, db);

                ExchangeRateRepository exchangeRateRepository = new ExchangeRateRepository();
                ExchangeRateBO exchangeRateBO = exchangeRateRepository.Get(new TblExchangeRate { SourceCurrencyId = (long)walletType.CurrencyId, TargetCurrencyId = (long)businessPackage.CurrencyId }, db);

                decimal _amountPaid = decimal.Parse(userBusinessPackage.AmountPaid);

                if (_amountPaid < businessPackage.ValueFrom)
                {
                    throw new ArgumentException("Payment is below the minimum package requirements");
                }

                if (userWallet.BalanceFiat >= _amountPaid)
                {
                    UserDepositRequestRepository userDepositRequestRepository = new UserDepositRequestRepository();
                    TblUserDepositRequest userDepositRequest = new TblUserDepositRequest
                    {
                        Address = userBusinessPackage.PaymentAddress,
                        Amount = _amountPaid,
                        DepositStatus = (short)DepositStatus.PendingPayment,
                        CreatedOn = DateTime.Now,
                        SourceCurrencyId = currency.Id,
                        TargetWalletTypeId = walletType.Id,
                        UserAuthId = userBusinessPackage.Id
                    };

                    TblUserDepositRequest x = userDepositRequestRepository.Create(userDepositRequest, db);

                    UserBusinessPackageRepository userBusinessPackageRepository = new UserBusinessPackageRepository();
                    TblUserBusinessPackage tblUserBusinessPackage = new TblUserBusinessPackage
                    {
                        IsEnabled = true,
                        CreatedOn = DateTime.Now,
                        ActivationDate = DateTime.Now,
                        BusinessPackageId = 1,
                        UserAuthId = userBusinessPackage.Id,
                        PackageStatus = PackageStatus.PendingActivation,
                        UserDepositRequestId = x.Id
                    };
                    userBusinessPackageRepository.Create(tblUserBusinessPackage, db);

                    userWalletAppService.Decrement(new UserWalletBO { UserAuthId = userWallet.UserAuthId, WalletCode = userWallet.WalletType.Code, WalletTypeId = userWallet.WalletTypeId }, new WalletTransactionBO { Amount = (_amountPaid * exchangeRateBO.OppositeValue) });

                    UserIncomeAppService userIncomeAppService = new UserIncomeAppService();
                    userIncomeAppService.ExecuteIncomeDistribution(new TblUserAuth { Id = userBusinessPackage.Id }, tblUserBusinessPackage, _amountPaid, db);

                    db.SaveChanges();

                    transaction.Commit();
                    return true;
                }
                else { throw new ArgumentException("Insufficient wallet funds"); }
            }
            else
            {
                using (db = new ArkContext())
                {
                    using var transaction = db.Database.BeginTransaction();
                    WalletTypeRepository walletTypeRepository = new WalletTypeRepository();
                    TblWalletType walletType = walletTypeRepository.Get(new UserWalletBO { WalletCode = userBusinessPackage.FromWalletCode, WalletTypeId = 0 }, db);

                    UserWalletAppService userWalletAppService = new UserWalletAppService();
                    UserWalletBO userWallet = userWalletAppService.GetBO(new UserWalletBO { UserAuthId = userBusinessPackage.Id, WalletTypeId = walletType.Id }, db);

                    CurrencyTypeRepository currencyTypeRepository = new CurrencyTypeRepository();
                    TblCurrency currency = currencyTypeRepository.Get(new TblCurrency { CurrencyIsoCode3 = userBusinessPackage.FromCurrencyIso3 }, db);

                    BusinessPackageRepository businessPackageRepository = new BusinessPackageRepository();
                    TblBusinessPackage businessPackage = businessPackageRepository.Get(new TblBusinessPackage { Id = long.Parse(userBusinessPackage.BusinessPackageID) }, db);

                    ExchangeRateRepository exchangeRateRepository = new ExchangeRateRepository();
                    ExchangeRateBO exchangeRateBO = exchangeRateRepository.Get(new TblExchangeRate { SourceCurrencyId = (long)walletType.CurrencyId, TargetCurrencyId = (long)businessPackage.CurrencyId }, db);

                    decimal _amountPaid = decimal.Parse(userBusinessPackage.AmountPaid);

                    if (_amountPaid < businessPackage.ValueFrom)
                    {
                        throw new ArgumentException("Payment is below the minimum package requirements");
                    }

                    if (userWallet.BalanceFiat >= _amountPaid)
                    {
                        UserDepositRequestRepository userDepositRequestRepository = new UserDepositRequestRepository();
                        TblUserDepositRequest userDepositRequest = new TblUserDepositRequest
                        {
                            Address = userBusinessPackage.PaymentAddress,
                            Amount = _amountPaid,
                            DepositStatus = (short)DepositStatus.PendingPayment,
                            CreatedOn = DateTime.Now,
                            SourceCurrencyId = currency.Id,
                            TargetWalletTypeId = walletType.Id,
                            UserAuthId = userBusinessPackage.Id
                        };

                        TblUserDepositRequest x = userDepositRequestRepository.Create(userDepositRequest, db);

                        UserBusinessPackageRepository userBusinessPackageRepository = new UserBusinessPackageRepository();
                        TblUserBusinessPackage tblUserBusinessPackage = new TblUserBusinessPackage
                        {
                            IsEnabled = true,
                            CreatedOn = DateTime.Now,
                            ActivationDate = DateTime.Now,
                            BusinessPackageId = 1,
                            BusinessPackage = businessPackage,
                            UserAuthId = userBusinessPackage.Id,
                            PackageStatus = PackageStatus.PendingActivation,
                            UserDepositRequestId = x.Id
                        };
                        userBusinessPackageRepository.Create(tblUserBusinessPackage, db);

                        userWalletAppService.Decrement(new UserWalletBO { UserAuthId = userWallet.UserAuthId, WalletCode = userWallet.WalletType.Code, WalletTypeId = userWallet.WalletTypeId }, new WalletTransactionBO { Amount = (_amountPaid * exchangeRateBO.OppositeValue) });

                        UserIncomeAppService userIncomeAppService = new UserIncomeAppService();
                        userIncomeAppService.ExecuteIncomeDistribution(new TblUserAuth { Id = userBusinessPackage.Id }, tblUserBusinessPackage, _amountPaid, db);

                        db.SaveChanges();

                        transaction.Commit();
                        return true;
                    }
                    else { throw new ArgumentException("Insufficient wallet funds"); }
                }
            }




        }

        public List<TblUserBusinessPackage> GetAll(TblUserAuth userAuth, DataAccessLayer.ArkContext db = null)
        {

            if (db != null)
            {
                UserBusinessPackageRepository userBusinessPackageRepository = new UserBusinessPackageRepository();
                return userBusinessPackageRepository.GetAll(userAuth, db);
            }
            else
            {
                using (db = new DataAccessLayer.ArkContext())
                {
                    using var transaction = db.Database.BeginTransaction();
                    UserBusinessPackageRepository userBusinessPackageRepository = new UserBusinessPackageRepository();
                    return userBusinessPackageRepository.GetAll(userAuth, db);
                }
            }
        }
    }
}
