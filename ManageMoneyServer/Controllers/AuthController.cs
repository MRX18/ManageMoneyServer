using ManageMoneyServer.Filters;
using ManageMoneyServer.Models;
using ManageMoneyServer.Models.ViewModels;
using ManageMoneyServer.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ManageMoneyServer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ValidationActionFilter]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private UserManager<User> UserManager { get; set; }
        private SignInManager<User> SignInManager { get; set; }
        private IConfiguration Configuration { get; set; }
        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            Configuration = configuration;
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            User user = new User
            {
                UserName = model.FullName,
                Email = model.Email
            };

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if(!result.Succeeded)
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                
                foreach(IdentityError error in result.Errors)
                {
                    if(error.Code.Contains("password", StringComparison.InvariantCultureIgnoreCase))
                    {
                        ModelState.AddModelError("password", error.Description);
                    } else
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                }

                return new EmptyResult();
            }

            return await GenerateJwt(model);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            return await GenerateJwt(model);
        }
        private async Task<IActionResult> GenerateJwt(LoginViewModel model)
        {
            AuthorizedViewModel result = null;
            User user = await UserManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                ModelState.AddModelError("email", Properties.Messages.UserNotFound);
            } else
            {
                Microsoft.AspNetCore.Identity.SignInResult signinResult = await SignInManager.CheckPasswordSignInAsync(user, model.Password, false);

                if (!signinResult.Succeeded)
                {
                    return Unauthorized(Properties.Messages.UnableAuthorize);
                } else
                {
                    List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email)
                    };

                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                    DateTime now = DateTime.UtcNow;

                    JwtSecurityToken jwt = new JwtSecurityToken(
                        issuer: Configuration["AuthOptions:ISSUER"],
                        audience: Configuration["AuthOptions:AUDIENCE"],
                        notBefore: now,
                        claims: claimsIdentity.Claims,
                        expires: now.Add(TimeSpan.FromMinutes(double.Parse(Configuration["AuthOptions:LIFETIME"]))),
                        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["AuthOptions:KEY"])), SecurityAlgorithms.HmacSha256)
                    );

                    string token = new JwtSecurityTokenHandler().WriteToken(jwt);

                    result = new AuthorizedViewModel
                    {
                        FullName = user.UserName,
                        Token = token
                    };
                }
            }

            return new JsonResponse(result);
        }
    }
}
