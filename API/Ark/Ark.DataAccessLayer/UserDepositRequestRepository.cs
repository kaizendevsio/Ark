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
   public class UserDepositRequestRepository
    {
        public TblUserDepositRequest Create(TblUserDepositRequest tblUserDepositRequest,ArkContext db)
        {
            db.TblUserDepositRequest.Add(tblUserDepositRequest);
            db.SaveChanges();

            return tblUserDepositRequest;
        }
    }
}
