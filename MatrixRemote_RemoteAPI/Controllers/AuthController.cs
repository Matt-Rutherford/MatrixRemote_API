using MatrixRemote_RemoteAPI.Logging;
using MatrixRemote_RemoteAPI.Models;
using MatrixRemote_RemoteAPI.Models.Auth.Login;
using MatrixRemote_RemoteAPI.Models.Auth.SignUp;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using User.Management.Service.Models;
using User.Management.Service.Services;

namespace MatrixRemote_RemoteAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        //private readonly ILogger _logger;
        public AuthController(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, IEmailService emailService, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _configuration = configuration;
            //_logger = logger
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser, string role)
        {
            try
            {
                // Check if user exists
                var userExist = await _userManager.FindByEmailAsync(registerUser.Email);
                if (userExist != null)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new Response { Status = "Error", Message = "Email is already registered." });
                }

                // Add user to db
                IdentityUser user = new()
                {
                    Email = registerUser.Email.ToLower(),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = registerUser.UserName
                };

                // Check if role exists
                if (await _roleManager.RoleExistsAsync(role))
                {
                    // Create user
                    var result = await _userManager.CreateAsync(user, registerUser.Password);

                    if (!result.Succeeded)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError,
                            new Response { Status = "Error", Message = "User creation failed." });
                    }

                    // Add role to user
                    var roleResult = await _userManager.AddToRoleAsync(user, role);
                    if (!roleResult.Succeeded)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError,
                            new Response { Status = "Error", Message = "Failed to assign role." });
                    }

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    Console.WriteLine($"Generated Token: {token}"); //TODO REMOVE

                    var confirmationLink = Url.Action(nameof(ConfirmEmail), "Auth", new { token = HttpUtility.UrlEncode(token), email = user.Email }, Request.Scheme);
                    //var confirmationLink = Url.Action("ConfirmEmail", "Auth", new { token, email = user.Email }, Request.Scheme);

                    var message = new Message(new string[] { user.Email! }, "Confirmation email link", confirmationLink!);
                    _emailService.SendEmail(message);

                    return Ok(new Response { Status = "Success", Message = $"User created and mail sent to {user.Email} successfully." });
                }
                else
                {
                    return NotFound(new Response { Status = "Error", Message = "This role does not exist." });
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                //_logger.LogError(ex, "Error occurred during user registration."); crashing program, not sure why
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "Internal server error occurred." });
            }
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            // Decode the token before using it
            var decodedToken = HttpUtility.UrlDecode(token);

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK,
                        new Response { Status = "Success", Message = "Email Verified Successfully." });
                }
            }

            return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "This user does not exist." });
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            //check user
            var user = await _userManager.FindByNameAsync(loginModel.Username);

            //check password
            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {

                //create claimlist
                var authClaims = new List<Claim> {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                //add roles to the claimlist
                var userRoles = await _userManager.GetRolesAsync(user);
                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                //generate token with the claims
                var jwtToken = GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    expiration = jwtToken.ValidTo
                }
                );

            }
            return Unauthorized();
            
            //return token

        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {

            var secret = _configuration["JWT:Secret"];
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(7),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

            return token;
        }
    }

    
}
