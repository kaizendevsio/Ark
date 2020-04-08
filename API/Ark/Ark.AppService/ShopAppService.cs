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
            var httpUtilities = new HttpUtilities();
            var _res = httpUtilities.PostAsyncXForm(new Uri("http://localhost/"), "wallet_update", shopUser).Result;

            return _res;
        }

        public ShopOrderItemBO UpdateOrderPayment(ShopOrderItemBO shopUser, string Environment)
        {
            var userDepositRequestRepository = new UserDepositRequestRepository();
            var userBusinessPackageRepository = new UserBusinessPackageRepository();
            var userAuthRepository = new UserAuthRepository();
            var paynamicsResponseRepository = new PaynamicsResponseRepository();

            var paynamics = new Paynamics();
            var shopOrderItemBO = paynamics.ProcessCallbackRequest(shopUser.RawBase64, Environment);

            using (var db = new ArkContext())
            {
                var userDepositRequest = userDepositRequestRepository.GetByRef(new TblUserDepositRequest { ReferenceNo = shopOrderItemBO.OrderID }, db);
                var userBusinessPackage = userBusinessPackageRepository.GetByDepId(userDepositRequest.Id, db);

                if (userDepositRequest.DepositStatus == (short)DepositStatus.PendingPayment)
                {
                    userDepositRequest.RawResponseData = shopOrderItemBO.RawDetails;
                    var paynamicsResponse = paynamicsResponseRepository.Get(shopOrderItemBO.ResponseCode, db);
                    shopOrderItemBO.Status = paynamicsResponse.Status.ToString();
                    shopOrderItemBO.TransactionType = "SHOP";

                    if (userBusinessPackage != null)
                    {
                        shopOrderItemBO.TransactionType = "BP";

                        // BUSINESS PACKAGE PURCHASE
                        switch (paynamicsResponse.Status)
                        {
                            case PaynamicsResponseStatus.Success:
                            {
                                userDepositRequest.DepositStatus = (short)DepositStatus.Paid;
                                var _packageBO = new UserBusinessPackageBO { UserPackageID = userBusinessPackage.Id, AmountPaid = (decimal)userDepositRequest.Amount, FromCurrencyIso3 = "PHP", DepositStatus = DepositStatus.Paid };
                                var userBusinessPackageAppService = new UserBusinessPackageAppService();
                                userBusinessPackageAppService.Update(_packageBO);
                                break;
                            }
                            case PaynamicsResponseStatus.Cancelled:
                            {
                                userDepositRequest.DepositStatus = (short)DepositStatus.Cancelled;
                                var _packageBO = new UserBusinessPackageBO { UserPackageID = userBusinessPackage.Id, AmountPaid = (decimal)userDepositRequest.Amount, FromCurrencyIso3 = "PHP", DepositStatus = DepositStatus.Paid };
                                var userBusinessPackageAppService = new UserBusinessPackageAppService();
                                userBusinessPackageAppService.Cancel(_packageBO);
                                break;
                            }
                            case PaynamicsResponseStatus.Error:
                            {
                                userDepositRequest.DepositStatus = (short)DepositStatus.InvalidPayment;
                                var _packageBO = new UserBusinessPackageBO { UserPackageID = userBusinessPackage.Id, AmountPaid = (decimal)userDepositRequest.Amount, FromCurrencyIso3 = "PHP", DepositStatus = DepositStatus.Paid };
                                var userBusinessPackageAppService = new UserBusinessPackageAppService();
                                userBusinessPackageAppService.Cancel(_packageBO);
                                break;
                            }
                            case PaynamicsResponseStatus.Pending:
                                userDepositRequest.DepositStatus = (short)DepositStatus.PendingPayment;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    else
                    {
                        // SHOP PAYMENT
                          switch (paynamicsResponse.Status)
                        {
                            case PaynamicsResponseStatus.Success:
                            {
                                userDepositRequest.DepositStatus = (short)DepositStatus.Paid;
                                break;
                            }
                            case PaynamicsResponseStatus.Cancelled:
                            {
                                userDepositRequest.DepositStatus = (short)DepositStatus.Cancelled;
                                break;
                            }
                            case PaynamicsResponseStatus.Error:
                            {
                                userDepositRequest.DepositStatus = (short)DepositStatus.InvalidPayment;
                                break;
                            }
                            case PaynamicsResponseStatus.Pending:
                                userDepositRequest.DepositStatus = (short)DepositStatus.PendingPayment;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    userDepositRequestRepository.Update(userDepositRequest, db);
                }
                else
                {
                    shopOrderItemBO.TransactionType = "SHOP";
                    if (userBusinessPackage != null)
                    {
                        shopOrderItemBO.TransactionType = "BP";
                    }
                        shopOrderItemBO.Status = "Duplicate";
                }

                var userAuth = userAuthRepository.GetByID((long)userDepositRequest.UserAuthId, db);

                shopOrderItemBO.ShopUserId = userAuth.ShopUserId;

                return shopOrderItemBO;
            }
              
        }
        public bool CancelOrderPayment(string requestId, string Environment)
        {
            var userDepositRequestRepository = new UserDepositRequestRepository();
            var userBusinessPackageRepository = new UserBusinessPackageRepository();
            var userAuthRepository = new UserAuthRepository();
            var paynamicsResponseRepository = new PaynamicsResponseRepository();

            var paynamics = new Paynamics();

            using (var db = new ArkContext())
            {
                var userDepositRequest = userDepositRequestRepository.GetByRef(new TblUserDepositRequest { ReferenceNo = paynamics.Base64Decode(requestId) }, db);
                var userBusinessPackage = userBusinessPackageRepository.GetByDepId(userDepositRequest.Id, db);

                if (userDepositRequest.DepositStatus == (short)DepositStatus.PendingPayment)
                {
                    userDepositRequest.DepositStatus = (short)DepositStatus.Paid;

                    if (userBusinessPackage != null)
                    {
                        var _packageBO = new UserBusinessPackageBO { UserPackageID = userBusinessPackage.Id, AmountPaid = (decimal)userDepositRequest.Amount, FromCurrencyIso3 = "PHP", DepositStatus = DepositStatus.Paid };
                        var userBusinessPackageAppService = new UserBusinessPackageAppService();
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

