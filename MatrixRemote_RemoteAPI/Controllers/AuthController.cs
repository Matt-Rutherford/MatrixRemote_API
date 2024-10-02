using MatrixRemote_RemoteAPI.Logging;
using MatrixRemote_RemoteAPI.Models;
using MatrixRemote_RemoteAPI.Models.Auth.SignUp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            RoleManager<IdentityRole> roleManager, IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            //_configuration = configuration; //don't need? 
            _emailService = emailService;
            //_logger = logger;

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
    }
}

/*
 * Generated Token: CfDJ8HaJhyfXvvBCtakOY/OTTzCXkX057VM5ruwKMYDrKOc44opuPMJvfFhT2GL6v/fVUN4NO7Sc+iGCbUMAB5Bap5/7SVmQHItwpRRCC3f7C35VtOvy3BwV4p5hWPBBduWf8hWlBH1kthZnSxlIVKiTeZ4KdmOb5EH1PwyAuxN/s9WwZOjFxnLFyNvQtu46ncKJnEuIEXKcNwGxpchLSK55zxvP0q3QKWKymDlELJwlavCh1lEu9km4vuXVou8g9cSf5A==
Received Token: CfDJ8HaJhyfXvvBCtakOY%2fOTTzCXkX057VM5ruwKMYDrKOc44opuPMJvfFhT2GL6v%2ffVUN4NO7Sc%2biGCbUMAB5Bap5%2f7SVmQHItwpRRCC3f7C35VtOvy3BwV4p5hWPBBduWf8hWlBH1kthZnSxlIVKiTeZ4KdmOb5EH1PwyAuxN%2fs9WwZOjFxnLFyNvQtu46ncKJnEuIEXKcNwGxpchLSK55zxvP0q3QKWKymDlELJwlavCh1lEu9km4vuXVou8g9cSf5A%3d%3d
 * */