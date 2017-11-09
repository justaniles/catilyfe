﻿namespace CatiLyfe.Backend.Web.Core.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using CatiLyfe.Backend.Web.Models.Login;
    using CatiLyfe.Common.Security;
    using CatiLyfe.DataLayer;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// The login controller.
    /// </summary>
    [Route("[controller]")]
    public class LoginController : Controller
    {
        private ICatiAuthDataLayer authDataLayer;

        private IPasswordHelper passwordHelper;

        public LoginController(ICatiAuthDataLayer authDatalayer, IPasswordHelper passwordHelper)
        {
            this.authDataLayer = authDatalayer;
            this.passwordHelper = passwordHelper;
        }

        /// <summary>
        /// The login.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPut]
        public async Task Login([FromBody]LoginCredentials credentials)
        {
            var user = (await this.authDataLayer.GetUser(null, credentials.Email, null)).FirstOrDefault();

            var hashedPassword = this.passwordHelper.HashPassword(credentials.Password);

            this.passwordHelper.Validate(user.Password, hashedPassword);

            var claims = new[]
                             {
                                 new Claim(ClaimTypes.Name, user.Name), new Claim(ClaimTypes.Email, user.Email),
                                 new Claim(ClaimTypes.AuthenticationMethod, "catilyfe"),
                                 new Claim(ClaimTypes.AuthenticationInstant, DateTime.UtcNow.ToLongTimeString()),
                                 new Claim(ClaimTypes.Authentication, "yes"),
                             };
            var identity = new ClaimsIdentity(claims.Concat(user.Roles.Select(r => new Claim(ClaimTypes.Role, r))), "catilyfe");

            var principal = new ClaimsPrincipal(identity);

            await this.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = true });
        }
    }
}
