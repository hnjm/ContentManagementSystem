﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCMS.Entities;
using WebCMS.Entities.Models.Authentication;

namespace WebCMS.Services.Authentication
{
    public interface ITokenService
    {
        string Add(string emailAddress, UserTokenType userTokenType);
        string Redeem(string token, string emailAddress, string password);
    }

    public class TokenService : ITokenService
    {
        #region Dependencies

        private readonly WebEntityModel _context;
        private readonly IUserService _userService;
        private readonly IRegistrationService _registartionService;

        public TokenService(WebEntityModel context, IUserService userService, IRegistrationService registrationService)
        {
            _context = context;
            _userService = userService;
            _registartionService = registrationService;
        }

        #endregion Dependencies

        public string Add(string emailAddress, UserTokenType userTokenType)
        {
            var user = _userService.Get(emailAddress);

            if (user == null) return string.Empty;

            var userToken = new UserToken
            {
                UserId = user.UserId,
                Token = Guid.NewGuid().ToString(),
                UserTokenType = userTokenType,
                DateAdded = DateTime.Now
            };

            _context.UserTokens.Add(userToken);
            _context.SaveChanges();
            return userToken.Token;
        }

        public string Redeem(string token, string emailAddress, string password)
        {
            var userToken = _context.UserTokens.FirstOrDefault(x => x.Token.Equals(token, StringComparison.OrdinalIgnoreCase));

            if (userToken == null)
                return "Invalid Token. Please Request Reset Password Token Again!";

            if (!userToken.User.EmailAddress.Equals(emailAddress, StringComparison.OrdinalIgnoreCase))
                return "Invalid Token. This Token does not match the Email Address you entered!";

            if (userToken.DateRedeemed.HasValue)
                return "Invalid Token. This Token has already been used.";

            _registartionService.ChangePassword(userToken.User.UserId, password);

            userToken.DateRedeemed = DateTime.Now;
            _context.SaveChanges();

            return string.Empty;
        }
    }
}
