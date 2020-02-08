using System.Linq;
using Ark.Entities.DTO;
using Ark.Entities.BO;
using Ark.Entities.Enums;
using System;
using System.Collections.Generic;

namespace Ark.DataAccessLayer
{
    public class UserInfoRepository
    {
        public TblUserInfo Create(UserBO userBO, ArkContext db)
        {
            TblUserInfo _userInfo = new TblUserInfo();
            Guid g = Guid.NewGuid();
            Random random = new Random();

            string r = String.Format("{0}{1}{2}", userBO.UserName, random.Next(999), Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65))));

            _userInfo.FirstName = userBO.FirstName;
            _userInfo.LastName = userBO.LastName;
            _userInfo.PhoneNumber = userBO.PhoneNumber;
            _userInfo.Email = userBO.Email;
            _userInfo.Dob = userBO.Dob;
            _userInfo.CountryIsoCode2 = userBO.CountryIsoCode2;
            _userInfo.Gender = userBO.Gender;
            _userInfo.Uid = r;//g.ToString();
            _userInfo.IsEnabled = true;
            _userInfo.EmailStatus = (short)EmailStatus.Unverified;

            db.TblUserInfo.Add(_userInfo);
            db.SaveChanges();

            return _userInfo;
        }

        public TblUserInfo Get(TblUserAuth userAuth, ArkContext db)
        {
            var _qUi = from a in db.TblUserInfo
                       join b in db.TblUserAuth on a.Id equals b.UserInfoId
                       where a.Id == userAuth.UserInfoId || b.UserName == userAuth.UserName
                       select new TblUserInfo
                       {
                           FirstName = a.FirstName,
                           LastName = a.LastName,
                           Dob = a.Dob,
                           Email = a.Email,
                           PhoneNumber = a.PhoneNumber,
                           Gender = a.Gender,
                           Uid = a.Uid,
                           EmailStatus = a.EmailStatus,
                           CreatedOn = a.CreatedOn,
                           CountryIsoCode2 = a.CountryIsoCode2,
                           CompanyName = a.CompanyName

                       };

            TblUserInfo _tblUserInfo = _qUi.FirstOrDefault();
            return _tblUserInfo;
        }

        public List<UserBO> GetAll(ArkContext db)
        {
            var _qUi = from a in db.TblUserInfo
                       join b in db.TblUserAuth on a.Id equals b.UserInfoId
                       //where a.Id == userAuth.UserInfoId || b.UserName == userAuth.UserName
                       select new UserBO
                       {
                           FirstName = a.FirstName,
                           LastName = a.LastName,
                           Dob = a.Dob,
                           Email = a.Email,
                           PhoneNumber = a.PhoneNumber,
                           Gender = a.Gender,
                           Uid = a.Uid,
                           EmailStatus = a.EmailStatus,
                           CreatedOn = a.CreatedOn,
                           CountryIsoCode2 = a.CountryIsoCode2,
                           CompanyName = a.CompanyName,
                           UserName = b.UserName
                       };

            List<UserBO> _users = _qUi.ToList();
            return _users;
        }
    }
}
