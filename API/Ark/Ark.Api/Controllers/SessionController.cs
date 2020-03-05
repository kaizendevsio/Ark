using System.Collections.Generic;
using Ark.Entities.DTO;
using Ark.Entities.BO;
using Ark.AppService;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;

namespace Ark.API.Controllers
{
    public class SessionController : ControllerBase
    {
        private Uri ApiUri_Live { get; set; } = new Uri("http://ark.com.ph/");
        private Uri ApiUri_Staging { get; set; } = new Uri("http://nightly.ark.com.ph/");
        private Uri ApiUri_Local { get; set; } = new Uri("http://localhost/");
        public Uri ApiUri { get; set; }
        public string USER_SESSION { get; private set; } = "USER_SESSION";
        public bool CreateSession([FromBody] UserResponseBO userAuthResponse, ISession session)
        {
            session.SetString(USER_SESSION, JsonConvert.SerializeObject(userAuthResponse.UserAuth));
            return true;
        }
        public TblUserAuth GetSession(ISession session)
        {
            string _currentUserSession = session.GetString(USER_SESSION);

            if (_currentUserSession != null)
            {
                TblUserAuth userAuth = JsonConvert.DeserializeObject<TblUserAuth>(_currentUserSession);
                return userAuth;
            }
            else
            {
                throw new System.ArgumentException("User session invalid or expired.");
            }
        }
        public Uri ApiUriInit(string Environment)
        {
            if (Environment == "Development")
            {
                ApiUri = ApiUri_Local;
            }
            else if (Environment == "Staging")
            {
                ApiUri = ApiUri_Staging;
            }
            else
            {
                ApiUri = ApiUri_Live;
            }
            //string Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            return ApiUri;

        }
    }
}
