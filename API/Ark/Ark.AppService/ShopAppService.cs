using Ark.Entities.DTO;
using Ark.Entities.BO;
using Ark.Entities.Enums;
using Ark.DataAccessLayer;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Ark.ExternalUtilities;
using Ark.ExternalUtilities.Models;
using Ark.ExternalUtilities.Enums;

namespace Ark.AppService
{
   public class ShopAppService
    {
        public HttpResponseBO UpdateUserWallet(ShopUserCommissionItemBO shopUser)
        {
            HttpUtilities httpUtilities = new HttpUtilities();
            HttpResponseBO _res = httpUtilities.PostAsyncXForm(new Uri("http://localhost/"), "wallet_update", shopUser).Result;

            return _res;
        }

        public ShopOrderItemBO UpdateOrderPayment(ShopOrderItemBO shopUser, string Environment)
        {
            UserDepositRequestRepository userDepositRequestRepository = new UserDepositRequestRepository();
            UserBusinessPackageRepository userBusinessPackageRepository = new UserBusinessPackageRepository();
            UserAuthRepository userAuthRepository = new UserAuthRepository();
            PaynamicsResponseRepository paynamicsResponseRepository = new PaynamicsResponseRepository();

            Paynamics paynamics = new Paynamics();
            ShopOrderItemBO shopOrderItemBO = paynamics.ProcessCallbackRequest(shopUser.RawBase64, Environment);

            using (var db = new ArkContext())
            {
                TblUserDepositRequest userDepositRequest = userDepositRequestRepository.GetByRef(new TblUserDepositRequest { ReferenceNo = shopOrderItemBO.OrderID }, db);
                TblUserBusinessPackage userBusinessPackage = userBusinessPackageRepository.GetByDepId(userDepositRequest.Id, db);

                if (userDepositRequest.DepositStatus == (short)DepositStatus.PendingPayment)
                {
                    userDepositRequest.DepositStatus = (short)DepositStatus.Paid;
                    userDepositRequest.RawResponseData = shopOrderItemBO.RawDetails;

                    TblPaynamicsResponse paynamicsResponse = paynamicsResponseRepository.Get(shopOrderItemBO.ResponseCode, db);
                    shopOrderItemBO.Status = paynamicsResponse.Status.ToString();

                    userDepositRequestRepository.Update(userDepositRequest, db);
                    shopOrderItemBO.TransactionType = "SHOP";

                    if (userBusinessPackage != null)
                    {
                        if (paynamicsResponse.Status == PaynamicsResponseStatus.Success)
                        {
                            shopOrderItemBO.TransactionType = "BP";
                            UserBusinessPackageBO _packageBO = new UserBusinessPackageBO { UserPackageID = userBusinessPackage.Id, AmountPaid = (decimal)userDepositRequest.Amount, FromCurrencyIso3 = "PHP", DepositStatus = DepositStatus.Paid };
                            UserBusinessPackageAppService userBusinessPackageAppService = new UserBusinessPackageAppService();
                            userBusinessPackageAppService.Update(_packageBO);
                        }
                        else if (paynamicsResponse.Status == PaynamicsResponseStatus.Cancelled)
                        {
                            shopOrderItemBO.TransactionType = "BP";
                            UserBusinessPackageBO _packageBO = new UserBusinessPackageBO { UserPackageID = userBusinessPackage.Id, AmountPaid = (decimal)userDepositRequest.Amount, FromCurrencyIso3 = "PHP", DepositStatus = DepositStatus.Paid };
                            UserBusinessPackageAppService userBusinessPackageAppService = new UserBusinessPackageAppService();
                            userBusinessPackageAppService.Cancel(_packageBO);
                        }
                        else if (paynamicsResponse.Status == PaynamicsResponseStatus.Error)
                        {
                            shopOrderItemBO.TransactionType = "BP";
                            UserBusinessPackageBO _packageBO = new UserBusinessPackageBO { UserPackageID = userBusinessPackage.Id, AmountPaid = (decimal)userDepositRequest.Amount, FromCurrencyIso3 = "PHP", DepositStatus = DepositStatus.Paid };
                            UserBusinessPackageAppService userBusinessPackageAppService = new UserBusinessPackageAppService();
                            userBusinessPackageAppService.Cancel(_packageBO);
                        }
                    }
                }
                else
                {
                    shopOrderItemBO.Status = "Duplicate";
                }

                TblUserAuth userAuth = userAuthRepository.GetByID((long)userDepositRequest.UserAuthId, db);

                shopOrderItemBO.ShopUserId = userAuth.ShopUserId;

                return shopOrderItemBO;
            }
              
        }
        public bool CancelOrderPayment(string requestId, string Environment)
        {
            UserDepositRequestRepository userDepositRequestRepository = new UserDepositRequestRepository();
            UserBusinessPackageRepository userBusinessPackageRepository = new UserBusinessPackageRepository();
            UserAuthRepository userAuthRepository = new UserAuthRepository();
            PaynamicsResponseRepository paynamicsResponseRepository = new PaynamicsResponseRepository();

            Paynamics paynamics = new Paynamics();

            using (var db = new ArkContext())
            {
                TblUserDepositRequest userDepositRequest = userDepositRequestRepository.GetByRef(new TblUserDepositRequest { ReferenceNo = paynamics.Base64Decode(requestId) }, db);
                TblUserBusinessPackage userBusinessPackage = userBusinessPackageRepository.GetByDepId(userDepositRequest.Id, db);

                if (userDepositRequest.DepositStatus == (short)DepositStatus.PendingPayment)
                {
                    userDepositRequest.DepositStatus = (short)DepositStatus.Paid;

                    if (userBusinessPackage != null)
                    {
                        UserBusinessPackageBO _packageBO = new UserBusinessPackageBO { UserPackageID = userBusinessPackage.Id, AmountPaid = (decimal)userDepositRequest.Amount, FromCurrencyIso3 = "PHP", DepositStatus = DepositStatus.Paid };
                        UserBusinessPackageAppService userBusinessPackageAppService = new UserBusinessPackageAppService();
                        userBusinessPackageAppService.Cancel(_packageBO);
                    }
                }
                else
                {
                }
                return true;
            }

        }
    }
}

