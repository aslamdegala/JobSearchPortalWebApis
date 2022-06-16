using AuthenticationPlugin;
using JobSearchPortal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JobSearchPortal.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private JobSearchPortalContext dbcontext;
        private IConfiguration _configuration;
        private readonly AuthService _auth;
        public UserController(JobSearchPortalContext jobSearchPortalContext, IConfiguration configuration)
        {
            dbcontext = jobSearchPortalContext;
            _configuration = configuration;
            _auth = new AuthService(_configuration);
        }
        [HttpGet]
        //   [Authorize(Roles = "user")] 
        public IEnumerable<User> Get()
        {
            return dbcontext.Users;
        }
        /// <summary>
        /// Registering a User
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Register([FromBody] User user)

        {
            var userName = dbcontext.Users.Where(u => u.UserName == user.UserName).SingleOrDefault();
            if (userName != null)
            {
                return BadRequest("UserName Exist in DB");
            }
            var UserObj = new User

            {
                Name = user.Name,
                UserName = user.UserName,
                Email = user.Email,
                Password = SecurePasswordHasherHelper.Hash(user.Password),
                Role = user.Role,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address
            };

            dbcontext.Users.Add(UserObj);
            dbcontext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);

        }
        /// <summary>
        /// Updating the user details
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="user"></param>
        /// <returns></returns>

        [HttpPut]
        public IActionResult UpdateUserDetails(int UserId, [FromBody] User user)
        {

            var data = dbcontext.Users.Find(UserId);
            if (data == null)
            {

                return StatusCode(StatusCodes.Status400BadRequest);

            }
            else
            {
                data.Name = user.Name;
                data.Password = SecurePasswordHasherHelper.Hash(user.Password);
                data.PhoneNumber = user.PhoneNumber;
                data.Address = user.Address;
                dbcontext.SaveChanges();

                return StatusCode(StatusCodes.Status200OK);
            }
        }

        /// <summary>
        /// Login the user with user id and password
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Login([FromBody] User user)

        {

            
            var data = dbcontext.Users.FirstOrDefault(u => u.UserName == user.UserName);
            if (data == null)
            {
                return NotFound();
            }
            if (!SecurePasswordHasherHelper.Verify(user.Password, data.Password))
            {
                return Unauthorized();
            }
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, user.UserName),
                 new Claim(ClaimTypes.Email,user.UserName),
                 new Claim(ClaimTypes.Role,data.Role)
            };
            var token = _auth.GenerateAccessToken(claims);
            return new ObjectResult(new
            {
                access_token = token.AccessToken,
                expires_in = token.ExpiresIn,
                token_type = token.TokenType,
                creation_Time = token.ValidFrom,
                expiration_Time = token.ValidTo,

                Role = data.Role,
                Name = data.Name,
                UserName = data.UserName,
                Email = data.Email,
                userId = data.UserId,
                user = data,

            }

            );
        }



    }
}
