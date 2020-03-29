using System;
using System.Text;
using System.Linq;
using System.Security.Cryptography;
using Ark.Entities.DTO;
using Ark.Entities.BO;
using Ark.Entities.Enums;
using Ark.DataAccessLayer;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Ark.DataAccessLayer
{
   public class PaynamicsResponseRepository
    {
        public TblPaynamicsResponse Get(string _ResponseCode, ArkContext db)
        {
            TblPaynamicsResponse paynamicsResponse = db.TblPaynamicsResponse.AsNoTracking().FirstOrDefault(item => item.ResponseCode == _ResponseCode);
            return paynamicsResponse;
        }
    }
}
