using MatrixRemote_RemoteAPI.Models;
using MatrixRemote_RemoteAPI.Models.Auth.SignUp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MatrixRemote_RemoteAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        public AuthController(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;

        }
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser, string roles)
        {
            //Check if user exists
            var userExist = await _userManager.FindByEmailAsync(registerUser.Email);

            if (userExist != null)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new Response { Status = "Error" , Message = "Email is already registered. "});
            }

            //Add user to db
            IdentityUser user = new()
            {
                Email = registerUser.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerUser.UserName

            };
            var result = await _userManager.CreateAsync(user, registerUser.Password);
            return result.Succeeded
                ? StatusCode(StatusCodes.Status201Created,
                    new Response { Status = "Success", Message = "Email successfully registed. " })
                : (IActionResult)StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "Email unable to be registered. " });
            // Assign a role
        }
    }
}
