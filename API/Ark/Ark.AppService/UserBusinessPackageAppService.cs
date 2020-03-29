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
        public TblUserDepositRequest Create(UserBusinessPackageBO userBusinessPackage, ArkContext db = null)
        {
            if (db != null)
            {
                using var transaction = db.Database.BeginTransaction();
                WalletTypeRepository walletTypeRepository = new WalletTypeRepository();
                TblWalletType walletType = walletTypeRepository.Get(new UserWalletBO { WalletCode = userBusinessPackage.DepositStatus == DepositStatus.Paid ? userBusinessPackage.FromWalletCode : "TLI", WalletTypeId = 0 }, db);

                UserWalletAppService userWalletAppService = new UserWalletAppService();
                UserWalletBO userWallet = userWalletAppService.GetBO(new UserWalletBO { UserAuthId = userBusinessPackage.Id, WalletTypeId = walletType.Id }, db);

                CurrencyTypeRepository currencyTypeRepository = new CurrencyTypeRepository();
                TblCurrency currency = currencyTypeRepository.Get(new TblCurrency { CurrencyIsoCode3 = userBusinessPackage.FromCurrencyIso3 }, db);

                BusinessPackageRepository businessPackageRepository = new BusinessPackageRepository();
                TblBusinessPackage businessPackage = businessPackageRepository.Get(new TblBusinessPackage { Id = userBusinessPackage.BusinessPackageID }, db);

                ExchangeRateRepository exchangeRateRepository = new ExchangeRateRepository();
                ExchangeRateBO exchangeRateBO = exchangeRateRepository.Get(new TblExchangeRate { SourceCurrencyId = (long)walletType.CurrencyId, TargetCurrencyId = (long)businessPackage.CurrencyId }, db);

                decimal _amountPaid = userBusinessPackage.AmountPaid;

                if (_amountPaid < businessPackage.ValueFrom && userBusinessPackage.DepositStatus == DepositStatus.Paid)
                {
                    throw new ArgumentException("Payment is below the minimum package requirements");
                }

                if (userWallet.BalanceFiat >= _amountPaid || userBusinessPackage.DepositStatus == DepositStatus.PendingPayment)
                {
                    UserDepositRequestRepository userDepositRequestRepository = new UserDepositRequestRepository();
                    TblUserDepositRequest userDepositRequest = new TblUserDepositRequest
                    {
                        Address = userBusinessPackage.PaymentAddress,
                        Amount = _amountPaid,
                        DepositStatus = (short)userBusinessPackage.DepositStatus,
                        CreatedOn = DateTime.Now,
                        SourceCurrencyId = currency.Id,
                        TargetWalletTypeId = walletType.Id,
                        UserAuthId = userBusinessPackage.Id,
                        Remarks = userBusinessPackage.Remarks
                    };

                    TblUserDepositRequest x = userDepositRequestRepository.Create(userDepositRequest, db);

                    UserBusinessPackageRepository userBusinessPackageRepository = new UserBusinessPackageRepository();
                    TblUserBusinessPackage tblUserBusinessPackage = new TblUserBusinessPackage
                    {
                        IsEnabled = true,
                        CreatedOn = DateTime.Now,
                        ActivationDate = DateTime.Now,
                        BusinessPackageId = businessPackage.Id,
                        BusinessPackage = businessPackage,
                        UserAuthId = userBusinessPackage.Id,
                        PackageStatus = userBusinessPackage.DepositStatus == DepositStatus.PendingPayment ? PackageStatus.PendingActivation : PackageStatus.Activated,
                        UserDepositRequestId = x.Id,
                        ExpiryDate = businessPackage.ExpiryDate
                    };
                    userBusinessPackageRepository.Create(tblUserBusinessPackage, db);

                    if (userBusinessPackage.DepositStatus == DepositStatus.Paid)
                    {
                        userWalletAppService.Decrement(new UserWalletBO { UserAuthId = userWallet.UserAuthId, WalletCode = userWallet.WalletType.Code, WalletTypeId = userWallet.WalletTypeId }, new WalletTransactionBO { Amount = (_amountPaid * exchangeRateBO.OppositeValue) }, db);
                        UserIncomeAppService userIncomeAppService = new UserIncomeAppService();
                        userIncomeAppService.ExecuteIncomeDistribution(new TblUserAuth { Id = userBusinessPackage.Id }, tblUserBusinessPackage, _amountPaid, db);
                    }

                    db.SaveChanges();
                    transaction.Commit();
                    return x;
                }
                else { throw new ArgumentException("Insufficient wallet funds"); }
            }
            else
            {
                using (db = new ArkContext())
                {
                    using var transaction = db.Database.BeginTransaction();
                    WalletTypeRepository walletTypeRepository = new WalletTypeRepository();
                    TblWalletType walletType = walletTypeRepository.Get(new UserWalletBO { WalletCode = userBusinessPackage.DepositStatus == DepositStatus.Paid ? userBusinessPackage.FromWalletCode : "TLI", WalletTypeId = 0 }, db);

                    UserWalletAppService userWalletAppService = new UserWalletAppService();
                    UserWalletBO userWallet = userWalletAppService.GetBO(new UserWalletBO { UserAuthId = userBusinessPackage.Id, WalletTypeId = walletType.Id }, db);

                    CurrencyTypeRepository currencyTypeRepository = new CurrencyTypeRepository();
                    TblCurrency currency = currencyTypeRepository.Get(new TblCurrency { CurrencyIsoCode3 = userBusinessPackage.FromCurrencyIso3 }, db);

                    BusinessPackageRepository businessPackageRepository = new BusinessPackageRepository();
                    TblBusinessPackage businessPackage = businessPackageRepository.Get(new TblBusinessPackage { Id = userBusinessPackage.BusinessPackageID }, db);

                    ExchangeRateRepository exchangeRateRepository = new ExchangeRateRepository();
                    ExchangeRateBO exchangeRateBO = exchangeRateRepository.Get(new TblExchangeRate { SourceCurrencyId = (long)walletType.CurrencyId, TargetCurrencyId = (long)businessPackage.CurrencyId }, db);

                    decimal _amountPaid = userBusinessPackage.AmountPaid;

                    if (_amountPaid < businessPackage.ValueFrom && userBusinessPackage.DepositStatus == DepositStatus.Paid)
                    {
                        throw new ArgumentException("Payment is below the minimum package requirements");
                    }

                    if (userWallet.BalanceFiat >= _amountPaid || userBusinessPackage.DepositStatus == DepositStatus.PendingPayment)
                    {
                        UserDepositRequestRepository userDepositRequestRepository = new UserDepositRequestRepository();
                        TblUserDepositRequest userDepositRequest = new TblUserDepositRequest
                        {
                            Address = userBusinessPackage.PaymentAddress,
                            Amount = _amountPaid,
                            DepositStatus = (short)userBusinessPackage.DepositStatus,
                            CreatedOn = DateTime.Now,
                            SourceCurrencyId = currency.Id,
                            TargetWalletTypeId = walletType.Id,
                            UserAuthId = userBusinessPackage.Id,
                            Remarks = userBusinessPackage.Remarks
                        };

                        TblUserDepositRequest x = userDepositRequestRepository.Create(userDepositRequest, db);

                        UserBusinessPackageRepository userBusinessPackageRepository = new UserBusinessPackageRepository();

                        List<TblUserBusinessPackage> _checkUBP = userBusinessPackageRepository.GetAllUserPackages(new TblUserAuth { Id = userBusinessPackage.Id }, db);

                        if (_checkUBP.Count > 0)
                        {
                            throw new ArgumentException("You already purchased an enterprise package");
                        }

                        TblUserBusinessPackage tblUserBusinessPackage = new TblUserBusinessPackage
                        {
                            IsEnabled = true,
                            CreatedOn = DateTime.Now,
                            ActivationDate = DateTime.Now,
                            BusinessPackageId = businessPackage.Id,
                            UserAuthId = userBusinessPackage.Id,
                            PackageStatus = userBusinessPackage.DepositStatus == DepositStatus.PendingPayment ? PackageStatus.PendingActivation : PackageStatus.Activated,
                            UserDepositRequestId = x.Id,
                            ExpiryDate = businessPackage.ExpiryDate
                        };
                        userBusinessPackageRepository.Create(tblUserBusinessPackage, db);

                        if (userBusinessPackage.DepositStatus == DepositStatus.Paid)
                        {
                            userWalletAppService.Decrement(new UserWalletBO { UserAuthId = userWallet.UserAuthId, WalletCode = userWallet.WalletType.Code, WalletTypeId = userWallet.WalletTypeId }, new WalletTransactionBO { Amount = (_amountPaid * exchangeRateBO.OppositeValue) }, db);
                            UserIncomeAppService userIncomeAppService = new UserIncomeAppService();
                            userIncomeAppService.ExecuteIncomeDistribution(new TblUserAuth { Id = userBusinessPackage.Id }, tblUserBusinessPackage, _amountPaid, db);
                        }

                        db.SaveChanges();
                        transaction.Commit();
                        x.SourceCurrencyId = tblUserBusinessPackage.Id;
                        return x;
                    }
                    else { throw new ArgumentException("Insufficient wallet funds"); }
                }
            }




        }
        public TblUserAuth Update(UserBusinessPackageBO userBusinessPackage, ArkContext db = null)
        {
            if (db != null)
            {
                using var transaction = db.Database.BeginTransaction();
                decimal _amountPaid = userBusinessPackage.AmountPaid;

                UserAuthRepository userAuthRepository = new UserAuthRepository();

                UserBusinessPackageRepository userBusinessPackageRepository = new UserBusinessPackageRepository();
                TblUserBusinessPackage tblUserBusinessPackage = userBusinessPackageRepository.Get(new TblUserBusinessPackage { Id = userBusinessPackage.UserPackageID }, db);

                if (tblUserBusinessPackage.PackageStatus != PackageStatus.Activated)
                {
                    tblUserBusinessPackage.PackageStatus = PackageStatus.Activated;
                    tblUserBusinessPackage.ModifiedOn = DateTime.Now;

                    TblUserAuth userAuth = userAuthRepository.GetByID((long)tblUserBusinessPackage.UserAuthId, db);
                    userBusinessPackageRepository.Update(tblUserBusinessPackage, db);

                    UserDepositRequestRepository userDepositRequestRepository = new UserDepositRequestRepository();
                    TblUserDepositRequest userDepositRequest = userDepositRequestRepository.Get(new TblUserDepositRequest { Id = (long)tblUserBusinessPackage.UserDepositRequestId }, db);
                    userDepositRequest.DepositStatus = (short)DepositStatus.Paid;
                    userDepositRequest.ModifiedOn = DateTime.Now;

                    userDepositRequestRepository.Update(userDepositRequest, db);
                    tblUserBusinessPackage = userBusinessPackageRepository.Get(new TblUserBusinessPackage { Id = userBusinessPackage.UserPackageID }, db);


                    WalletTypeRepository walletTypeRepository = new WalletTypeRepository();
                    TblWalletType walletType = walletTypeRepository.Get(new UserWalletBO { WalletCode = userBusinessPackage.DepositStatus == DepositStatus.Paid ? userBusinessPackage.FromWalletCode : "TLI", WalletTypeId = 0 }, db);

                    UserWalletAppService userWalletAppService = new UserWalletAppService();
                    UserWalletBO userWallet = userWalletAppService.GetBO(new UserWalletBO { UserAuthId = (long)tblUserBusinessPackage.UserAuthId, WalletTypeId = walletType.Id }, db);

                    userWalletAppService.Increment(new UserWalletBO { UserAuthId = userWallet.UserAuthId, WalletCode = userWallet.WalletType.Code, WalletTypeId = userWallet.WalletTypeId }, new WalletTransactionBO { Amount = _amountPaid }, db);
                    UserIncomeAppService userIncomeAppService = new UserIncomeAppService();
                    userIncomeAppService.ExecuteIncomeDistribution(new TblUserAuth { Id = (long)tblUserBusinessPackage.UserAuthId }, tblUserBusinessPackage, _amountPaid, db);

                    ShopAppService shopAppService = new ShopAppService();
                    shopAppService.UpdateUserWallet(new ShopUserCommissionItemBO { ShopUserId = userAuth.ShopUserId, Reward = (decimal)tblUserBusinessPackage.BusinessPackage.Consumables, Remarks = "Package Consumables" });

                    db.SaveChanges();
                    transaction.Commit();

                    return userAuth;
                }
                else
                {
                    throw new ArgumentException("Package Aleardy Activated");
                }
            }
            else
            {
                using (db = new ArkContext())
                {
                    using var transaction = db.Database.BeginTransaction();
                    decimal _amountPaid = userBusinessPackage.AmountPaid;

                    UserAuthRepository userAuthRepository = new UserAuthRepository();

                    UserBusinessPackageRepository userBusinessPackageRepository = new UserBusinessPackageRepository();
                    TblUserBusinessPackage tblUserBusinessPackage = userBusinessPackageRepository.Get(new TblUserBusinessPackage { Id = userBusinessPackage.UserPackageID }, db);

                    if (tblUserBusinessPackage.PackageStatus != PackageStatus.Activated)
                    {
                        if (userBusinessPackage.DepositStatus == DepositStatus.Paid)
                        {
                            tblUserBusinessPackage.PackageStatus = PackageStatus.Activated;
                            tblUserBusinessPackage.ModifiedOn = DateTime.Now;

                            TblUserAuth userAuth = userAuthRepository.GetByID((long)tblUserBusinessPackage.UserAuthId, db);
                            userBusinessPackageRepository.Update(tblUserBusinessPackage, db);

                            UserDepositRequestRepository userDepositRequestRepository = new UserDepositRequestRepository();
                            TblUserDepositRequest userDepositRequest = userDepositRequestRepository.Get(new TblUserDepositRequest { Id = (long)tblUserBusinessPackage.UserDepositRequestId }, db);
                            userDepositRequest.DepositStatus = (short)DepositStatus.Paid;
                            userDepositRequest.ModifiedOn = DateTime.Now;

                            userDepositRequestRepository.Update(userDepositRequest, db);
                            tblUserBusinessPackage = userBusinessPackageRepository.Get(new TblUserBusinessPackage { Id = userBusinessPackage.UserPackageID }, db);


                            WalletTypeRepository walletTypeRepository = new WalletTypeRepository();
                            TblWalletType walletType = walletTypeRepository.Get(new UserWalletBO { WalletCode = "TLI", WalletTypeId = 0 }, db);

                            UserWalletAppService userWalletAppService = new UserWalletAppService();
                            UserWalletBO userWallet = userWalletAppService.GetBO(new UserWalletBO { UserAuthId = (long)tblUserBusinessPackage.UserAuthId, WalletTypeId = walletType.Id }, db);

                            userWalletAppService.Increment(new UserWalletBO { UserAuthId = userWallet.UserAuthId, WalletCode = userWallet.WalletType.Code, WalletTypeId = userWallet.WalletTypeId }, new WalletTransactionBO { Amount = _amountPaid }, db);
                            UserIncomeAppService userIncomeAppService = new UserIncomeAppService();
                            userIncomeAppService.ExecuteIncomeDistribution(new TblUserAuth { Id = (long)tblUserBusinessPackage.UserAuthId }, tblUserBusinessPackage, _amountPaid, db);

                            ShopAppService shopAppService = new ShopAppService();
                            shopAppService.UpdateUserWallet(new ShopUserCommissionItemBO { ShopUserId = userAuth.ShopUserId, Reward = (decimal)tblUserBusinessPackage.BusinessPackage.Consumables, Remarks = "Package Consumables" });

                            db.SaveChanges();
                            transaction.Commit();

                            return userAuth;
                        }
                        else
                        {
                            Cancel(userBusinessPackage);
                            throw new ArgumentException("Package Cancelled");
                        }
                       
                    }
                    else
                    {
                        throw new ArgumentException("Package Aleardy Activated");
                    }


                }
            }
        }
        public TblUserAuth Cancel(UserBusinessPackageBO userBusinessPackage, ArkContext db = null)
        {
            if (db != null)
            {
                using var transaction = db.Database.BeginTransaction();
                decimal _amountPaid = userBusinessPackage.AmountPaid;

                UserAuthRepository userAuthRepository = new UserAuthRepository();

                UserBusinessPackageRepository userBusinessPackageRepository = new UserBusinessPackageRepository();
                TblUserBusinessPackage tblUserBusinessPackage = userBusinessPackageRepository.Get(new TblUserBusinessPackage { Id = userBusinessPackage.UserPackageID }, db);

                if (tblUserBusinessPackage.PackageStatus != PackageStatus.Activated)
                {
                    tblUserBusinessPackage.PackageStatus = PackageStatus.Cancelled;
                    tblUserBusinessPackage.ModifiedOn = DateTime.Now;

                    TblUserAuth userAuth = userAuthRepository.GetByID((long)tblUserBusinessPackage.UserAuthId, db);
                    userBusinessPackageRepository.Update(tblUserBusinessPackage, db);

                    UserDepositRequestRepository userDepositRequestRepository = new UserDepositRequestRepository();
                    TblUserDepositRequest userDepositRequest = userDepositRequestRepository.Get(new TblUserDepositRequest { Id = (long)tblUserBusinessPackage.UserDepositRequestId }, db);
                    userDepositRequest.DepositStatus = (short)DepositStatus.Cancelled;
                    userDepositRequest.ModifiedOn = DateTime.Now;

                    userDepositRequestRepository.Update(userDepositRequest, db);

                    db.SaveChanges();
                    transaction.Commit();

                    return userAuth;
                }
                else
                {
                    throw new ArgumentException("Package Aleardy Activated");
                }

            }
            else
            {
                using (db = new ArkContext())
                {
                    using var transaction = db.Database.BeginTransaction();
                    decimal _amountPaid = userBusinessPackage.AmountPaid;

                    UserAuthRepository userAuthRepository = new UserAuthRepository();

                    UserBusinessPackageRepository userBusinessPackageRepository = new UserBusinessPackageRepository();
                    TblUserBusinessPackage tblUserBusinessPackage = userBusinessPackageRepository.Get(new TblUserBusinessPackage { Id = userBusinessPackage.UserPackageID }, db);

                    if (tblUserBusinessPackage.PackageStatus != PackageStatus.Activated)
                    {
                        tblUserBusinessPackage.PackageStatus = PackageStatus.Cancelled;
                        tblUserBusinessPackage.ModifiedOn = DateTime.Now;

                        TblUserAuth userAuth = userAuthRepository.GetByID((long)tblUserBusinessPackage.UserAuthId, db);
                        userBusinessPackageRepository.Delete(tblUserBusinessPackage, db);

                        UserDepositRequestRepository userDepositRequestRepository = new UserDepositRequestRepository();
                        TblUserDepositRequest userDepositRequest = userDepositRequestRepository.Get(new TblUserDepositRequest { Id = (long)tblUserBusinessPackage.UserDepositRequestId }, db);
                        userDepositRequest.DepositStatus = (short)DepositStatus.Cancelled;
                        userDepositRequest.ModifiedOn = DateTime.Now;

                        userDepositRequestRepository.Update(userDepositRequest, db);
                       
                        db.SaveChanges();
                        transaction.Commit();

                        return userAuth;
                    }
                    else
                    {
                        throw new ArgumentException("Package Aleardy Activated");
                    }


                }
            }
        }
        public List<TblUserDepositRequest> GetUserDepositRequests(TblUserAuth userAuth, DepositStatus depositStatus, ArkContext db = null)
        {
            if (db != null)
            {
                UserDepositRequestRepository userDepositRequestRepository = new UserDepositRequestRepository();
                return userDepositRequestRepository.GetAll(userAuth, depositStatus, db);
            }
            else
            {
                using (db = new DataAccessLayer.ArkContext())
                {
                    using var transaction = db.Database.BeginTransaction();
                    UserDepositRequestRepository userDepositRequestRepository = new UserDepositRequestRepository();
                    return userDepositRequestRepository.GetAll(userAuth, depositStatus, db);
                }
            }
        }
        public List<TblUserBusinessPackage> GetAll(TblUserAuth userAuth, ArkContext db = null)
        {

            if (db != null)
            {
                UserBusinessPackageRepository userBusinessPackageRepository = new UserBusinessPackageRepository();
                return userBusinessPackageRepository.GetAllUserPackages(userAuth, db);
            }
            else
            {
                using (db = new ArkContext())
                {
                    using var transaction = db.Database.BeginTransaction();
                    UserBusinessPackageRepository userBusinessPackageRepository = new UserBusinessPackageRepository();
                    return userBusinessPackageRepository.GetAllUserPackages(userAuth, db);
                }
            }
        }
        public List<TblBusinessPackage> GetAllBusinessPackages(TblUserAuth userAuth, DataAccessLayer.ArkContext db = null)
        {

            if (db != null)
            {
                UserMapRepository userMapRepository = new UserMapRepository();
                UserBusinessPackageRepository userBusinessPackageRepository = new UserBusinessPackageRepository();
                TblUserMap userMap = userMapRepository.Get(userAuth, db);

                return userBusinessPackageRepository.GetAll(userMap, db);
            }
            else
            {
                using (db = new DataAccessLayer.ArkContext())
                {
                    using var transaction = db.Database.BeginTransaction();

                    UserMapRepository userMapRepository = new UserMapRepository();
                    UserBusinessPackageRepository userBusinessPackageRepository = new UserBusinessPackageRepository();
                    TblUserMap userMap = userMapRepository.Get(userAuth, db);

                    return userBusinessPackageRepository.GetAll(userMap, db);
                }
            }
        }
    }
}
