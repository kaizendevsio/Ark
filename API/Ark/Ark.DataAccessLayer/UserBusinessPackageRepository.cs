using System.Text;
using System.Linq;
using System.Security.Cryptography;
using Ark.Entities.DTO;
using Ark.Entities.BO;
using Ark.Entities.Enums;
using AutoMapper;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Ark.DataAccessLayer
{
    public class UserBusinessPackageRepository
    {
        public bool Create(TblUserBusinessPackage tblUserBusinessPackage, ArkContext db)
        {
            db.TblUserBusinessPackage.Add(tblUserBusinessPackage);
            db.SaveChanges();

            return true;
        }

        public TblUserBusinessPackage Get(TblUserBusinessPackage tblUserBusinessPackage, ArkContext db)
        {
            //TblUserBusinessPackage userBusinessPackage = db.TblUserBusinessPackage.FirstOrDefault(item => item.Id == tblUserBusinessPackage.Id);
            //return userBusinessPackage;

            var _qUi = from a in db.TblUserAuth
                       join b in db.TblUserBusinessPackage on a.Id equals b.UserAuthId
                       join c in db.TblUserDepositRequest on b.UserDepositRequestId equals c.Id
                       join d in db.TblBusinessPackage on b.BusinessPackageId equals d.Id
                       join e in db.TblBusinessPackageType on d.PackageTypeId equals e.Id
                       join f in db.TblCurrency on c.SourceCurrencyId equals f.Id

                       join h in db.TblUserMap on a.Id equals h.Id

                       where a.IsEnabled == true && b.Id == tblUserBusinessPackage.Id

                       orderby a.CreatedOn descending
                       select new TblUserBusinessPackage
                       {
                           Id = b.Id,
                           CreatedOn = b.CreatedOn,
                           IsEnabled = b.IsEnabled,
                           ActivationDate = b.ActivationDate,
                           BusinessPackage = new TblBusinessPackage
                           {
                               Id = d.Id,
                               PackageType = e,
                               PackageName = d.PackageName,
                               PackageCode = d.PackageCode,
                               CreatedOn = d.CreatedOn,
                               IsEnabled = d.IsEnabled,
                               ValueFrom = h.SponsorUserId == 2 ? h.IdNavigation.UserInfo.CountryIsoCode2 == "ARKPH2020" ? d.ValueFrom : d.ValueFrom : h.IdNavigation.UserInfo.CountryIsoCode2 == "ARKPH2020" ? d.ValueFrom : (d.ValueFrom - d.DiscountValue),
                               ValueTo = h.SponsorUserId == 2 ? h.IdNavigation.UserInfo.CountryIsoCode2 == "ARKPH2020" ? d.ValueFrom : d.ValueTo : h.IdNavigation.UserInfo.CountryIsoCode2 == "ARKPH2020" ? d.ValueFrom : (d.ValueTo - d.DiscountValue),
                               PackageDescription = d.PackageDescription,
                               NetworkValue = d.NetworkValue,
                               Consumables = h.SponsorUserId == 2 ? h.IdNavigation.UserInfo.CountryIsoCode2 == "ARKPH2020" ? d.Consumables3 : d.Consumables : h.IdNavigation.UserInfo.CountryIsoCode2 == "ARKPH2020" ? d.Consumables3 : d.Consumables2,
                           },
                           BusinessPackageId = b.BusinessPackageId,
                           CancellationDate = b.CancellationDate,
                           CreatedBy = b.CreatedBy,
                           ExpiryDate = b.ExpiryDate,
                           UserDepositRequestId = b.UserDepositRequestId,
                           UserDepositRequest = c,
                           PackageStatus = b.PackageStatus,
                           ModifiedOn = b.ModifiedOn,
                           UserAuthId = b.UserAuthId
                       };

            TblUserBusinessPackage _ubp = _qUi.AsNoTracking().FirstOrDefault();

            return _ubp;
        }
        public TblUserBusinessPackage Get(TblUserAuth userAuth, ArkContext db)
        {
            //TblUserBusinessPackage userBusinessPackage = db.TblUserBusinessPackage.FirstOrDefault(item => item.Id == tblUserBusinessPackage.Id);
            //return userBusinessPackage;

            var _qUi = from a in db.TblUserBusinessPackage
                       join b in db.TblBusinessPackage on a.BusinessPackageId equals b.Id
                       join c in db.TblUserDepositRequest on a.UserDepositRequestId equals c.Id

                       where a.IsEnabled == true && a.UserAuthId == userAuth.Id

                       orderby a.CreatedOn descending
                       select new TblUserBusinessPackage
                       {
                           Id = a.Id,
                           CreatedOn = a.CreatedOn,
                           IsEnabled = a.IsEnabled,
                           ActivationDate = a.ActivationDate,
                           BusinessPackage = b,
                           BusinessPackageId = a.BusinessPackageId,
                           CancellationDate = a.CancellationDate,
                           CreatedBy = a.CreatedBy,
                           ExpiryDate = a.ExpiryDate,
                           UserDepositRequestId = a.UserDepositRequestId,
                           UserDepositRequest = c,
                           PackageStatus = a.PackageStatus,
                           ModifiedOn = a.ModifiedOn,
                           UserAuthId = a.UserAuthId
                       };

            TblUserBusinessPackage _ubp = _qUi.AsNoTracking().FirstOrDefault();

            return _ubp;
        }
        public TblUserBusinessPackage GetByDepId(long depId, ArkContext db)
        {
            //TblUserBusinessPackage userBusinessPackage = db.TblUserBusinessPackage.FirstOrDefault(item => item.Id == tblUserBusinessPackage.Id);
            //return userBusinessPackage;

            var _qUi = from a in db.TblUserBusinessPackage
                       join b in db.TblBusinessPackage on a.BusinessPackageId equals b.Id
                       join c in db.TblUserDepositRequest on a.UserDepositRequestId equals c.Id

                       where a.IsEnabled == true && a.UserDepositRequestId == depId

                       orderby a.CreatedOn descending
                       select new TblUserBusinessPackage
                       {
                           Id = a.Id,
                           CreatedOn = a.CreatedOn,
                           IsEnabled = a.IsEnabled,
                           ActivationDate = a.ActivationDate,
                           BusinessPackage = b,
                           BusinessPackageId = a.BusinessPackageId,
                           CancellationDate = a.CancellationDate,
                           CreatedBy = a.CreatedBy,
                           ExpiryDate = a.ExpiryDate,
                           UserDepositRequestId = a.UserDepositRequestId,
                           UserDepositRequest = c,
                           PackageStatus = a.PackageStatus,
                           ModifiedOn = a.ModifiedOn,
                           UserAuthId = a.UserAuthId
                       };

            TblUserBusinessPackage _ubp = _qUi.AsNoTracking().FirstOrDefault();

            return _ubp;
        }
        public void Update(TblUserBusinessPackage tblUserBusinessPackage, ArkContext db)
        {
            tblUserBusinessPackage.BusinessPackage = null;
            db.TblUserBusinessPackage.Update(tblUserBusinessPackage);
            db.SaveChanges();
        }

        public void Delete(TblUserBusinessPackage tblUserBusinessPackage, ArkContext db)
        {
            tblUserBusinessPackage.BusinessPackage = null;
            db.TblUserBusinessPackage.Remove(tblUserBusinessPackage);
            db.SaveChanges();
        }

        public List<TblUserBusinessPackage> GetAllUserPackages(TblUserAuth userAuth, ArkContext db)
        {
            var _qUi = from a in db.TblUserAuth
                       join b in db.TblUserBusinessPackage on a.Id equals b.UserAuthId
                       join c in db.TblUserDepositRequest on b.UserDepositRequestId equals c.Id
                       join d in db.TblBusinessPackage on b.BusinessPackageId equals d.Id
                       join e in db.TblBusinessPackageType on d.PackageTypeId equals e.Id
                       join f in db.TblCurrency on c.SourceCurrencyId equals f.Id

                       join h in db.TblUserMap on a.Id equals h.Id


                       //join g in db.TblWalletType on c.TargetWalletTypeId equals g.Id
                       orderby b.CreatedOn descending
                       where a.Id == userAuth.Id && b.PackageStatus != PackageStatus.Cancelled && b.PackageStatus != PackageStatus.Expired &&  b.ExpiryDate > DateTime.Now
                       select new TblUserBusinessPackage
                       {
                         Id = b.Id,
                         ActivationDate = b.ActivationDate,
                         UserDepositRequest = new TblUserDepositRequest { Address = c.Address, Amount = c.Amount, CreatedOn = c.CreatedOn, DepositStatus = c.DepositStatus, ExpiryDate = c.ExpiryDate, Id = c.Id, IsEnabled = c.IsEnabled, Remarks = c.Remarks, SourceCurrency = f},
                         IsEnabled = b.IsEnabled,
                         CreatedOn = b.CreatedOn,
                         PackageStatus = b.PackageStatus,
                         CancellationDate = b.CancellationDate,
                           BusinessPackage = new TblBusinessPackage
                           {
                               PackageType = e,
                               PackageName = d.PackageName,
                               PackageCode = d.PackageCode,
                               CreatedOn = d.CreatedOn,
                               IsEnabled = d.IsEnabled,
                               ValueFrom = h.SponsorUserId == 2 ? h.IdNavigation.UserInfo.CountryIsoCode2 == "ARKPH2020" ? d.ValueFrom : d.ValueFrom : h.IdNavigation.UserInfo.CountryIsoCode2 == "ARKPH2020" ? d.ValueFrom : (d.ValueFrom - d.DiscountValue),
                               ValueTo = h.SponsorUserId == 2 ? h.IdNavigation.UserInfo.CountryIsoCode2 == "ARKPH2020" ? d.ValueFrom : d.ValueTo : h.IdNavigation.UserInfo.CountryIsoCode2 == "ARKPH2020" ? d.ValueFrom : (d.ValueTo - d.DiscountValue),
                               PackageDescription = d.PackageDescription,
                               NetworkValue = d.NetworkValue,
                               Consumables = h.SponsorUserId == 2 ? h.IdNavigation.UserInfo.CountryIsoCode2 == "ARKPH2020" ? d.Consumables3 : d.Consumables : h.IdNavigation.UserInfo.CountryIsoCode2 == "ARKPH2020" ? d.Consumables3 : d.Consumables2,
                           }
                       };

            List<TblUserBusinessPackage> _ubp = _qUi.AsNoTracking().ToList<TblUserBusinessPackage>();

            return _ubp;
        }
        public List<TblBusinessPackage> GetAll(TblUserMap tblUserMap, ArkContext db)
        {
            var _qUi = from a in db.TblBusinessPackage
                       join b in db.TblCurrency on a.CurrencyId equals b.Id
                       //join c in db.TblUserMap on tblUserMap.Id equals c.Id into _um

                       //from um in _um.DefaultIfEmpty()
                       orderby a.Id descending
                       where a.IsEnabled == true

                       select new TblBusinessPackage
                       {
                           Id = a.Id,
                           CalculationMethod = a.CalculationMethod,
                           CreatedOn = a.CreatedOn,
                           DiscountType = a.DiscountType,
                           DiscountValue = a.DiscountValue,
                           IsEnabled = a.IsEnabled,
                           NetworkValue = a.NetworkValue,
                           PackageCode = a.PackageCode,
                           PackageName = a.PackageName,
                           ValueFrom = tblUserMap.SponsorUserId == 2 ? tblUserMap.IdNavigation.UserInfo.CountryIsoCode2 == "ARKPH2020" ? a.ValueFrom : a.ValueFrom : tblUserMap.IdNavigation.UserInfo.CountryIsoCode2 == "ARKPH2020" ? a.ValueFrom : (a.ValueFrom - a.DiscountValue),
                           ValueTo = tblUserMap.SponsorUserId == 2 ? tblUserMap.IdNavigation.UserInfo.CountryIsoCode2 == "ARKPH2020" ? a.ValueFrom : a.ValueTo : tblUserMap.IdNavigation.UserInfo.CountryIsoCode2 == "ARKPH2020" ? a.ValueFrom : (a.ValueTo - a.DiscountValue),
                           ImageFile = tblUserMap.SponsorUserId == 2 ? tblUserMap.IdNavigation.UserInfo.CountryIsoCode2 == "ARKPH2020" ? a.ImageFilePromo : a.ImageFileOriginal : tblUserMap.IdNavigation.UserInfo.CountryIsoCode2 == "ARKPH2020" ? a.ImageFilePromo : a.ImageFileDiscounted,
                           PackageDescription = a.PackageDescription,
                           Currency = b,
                           Consumables = a.Consumables
                       };

            List<TblBusinessPackage> _ubp = _qUi.AsNoTracking().ToList<TblBusinessPackage>();

            return _ubp;
        }
    }
}
