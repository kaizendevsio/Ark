using Ark.Entities.DTO;
using Ark.Entities.BO;
using Ark.Entities.Enums;
using Ark.DataAccessLayer;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace Ark.AppService
{
   public class AdminAccessAppService
    {
        public List<UserBO> GetAllUsers(ArkContext db = null)
        {
            using (db = new DataAccessLayer.ArkContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    UserInfoRepository userInfoRepository = new UserInfoRepository();
                    return userInfoRepository.GetAll(db);
                }
            }

        }

        public List<UserBO> GetAllDepositRequest(ArkContext db = null)
        {
            using (db = new ArkContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    UserInfoRepository userInfoRepository = new UserInfoRepository();
                    return userInfoRepository.GetAllDeposit(db);
                }
            }
        }

        public UserBO GetUserByShopID(UserBO userBO, ArkContext db = null)
        {
            using (db = new ArkContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    UserAuthRepository userAuthRepository = new UserAuthRepository();
                    TblUserAuth userAuth = userAuthRepository.GetByShopID(userBO.ShopUserId, db);

                    UserBO user = new UserBO
                    {
                        UserName = userAuth.UserName,
                        ShopUserId = userAuth.ShopUserId,
                        LoginStatus = (short)userAuth.LoginStatus,
                        FirstName = userAuth.UserInfo.FirstName,
                        LastName = userAuth.UserInfo.LastName,
                        Uid = userAuth.UserInfo.Uid,
                        Dob = userAuth.UserInfo.Dob,
                        CreatedOn = userAuth.CreatedOn,
                        Email = userAuth.UserInfo.Email,
                        Gender = userAuth.UserInfo.Gender,
                        CountryIsoCode2 = userAuth.UserInfo.CountryIsoCode2,
                        EmailStatus = userAuth.UserInfo.EmailStatus,
                        PhoneNumber = userAuth.UserInfo.PhoneNumber,
                        Id = userAuth.Id
                    };

                    return user;
                }
            }

        }

        public List<UserWalletBO> GetUserWallets(TblUserAuth userAuth, ArkContext db = null)
        {
            using (db = new ArkContext())
            {
                UserWalletRepository userWalletRepository = new UserWalletRepository();
                List<UserWalletBO> userWallet = userWalletRepository.GetAllBO(userAuth, db);

                return userWallet;
            }
        }

    }
}
