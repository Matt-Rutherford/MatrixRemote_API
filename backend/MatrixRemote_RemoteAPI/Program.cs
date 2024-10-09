using MatrixRemote_RemoteAPI.Data;
using MatrixRemote_RemoteAPI.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using User.Management.Service.Models;
using User.Management.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
//    .WriteTo.File("log/remoteLogs.txt", rollingInterval: RollingInterval.Year).CreateLogger();

//builder.Host.UseSerilog();


// For db, entity framework core!
var configuration = builder.Configuration;
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("WebApiDatabase")));

// For identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => 
    { 
    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider; 
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

//Add config for req email
builder.Services.Configure<IdentityOptions>(
    options => options.SignIn.RequireConfirmedEmail = true);

// Add Auth
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters() 
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
    };


});

// Add email configs
//var emailConfig = configuration
//    .GetSection("EmailConfiguration")
//    .Get<EmailConfiguration>();

builder.Services.Configure<EmailConfiguration>(configuration.GetSection("EmailConfiguration"));
builder.Services.AddScoped<IEmailService, EmailService>();

// Controllers
builder.Services.AddControllers(option =>
{
    option.ReturnHttpNotAcceptable = false; // Ensures that unacceptable formats are rejected, ie only accepts JSON rn
    option.RespectBrowserAcceptHeader = true; // Ensure the server respects the client's Accept header
})
.AddNewtonsoftJson(options =>
{
    // Optional: Configure Newtonsoft.Json settings here
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore; // Ignore null values
    options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented; // Pretty print JSON output (Optional)
})
.AddXmlDataContractSerializerFormatters(); // Optionally, keep XML serialization support

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo {
        Title = "My API",
        Version = "v1"
    });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme 
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement 
    {
        {
            new OpenApiSecurityScheme 
            {
                Reference = new OpenApiReference
                {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] {}
    }
    });
});

builder.Services.AddSingleton<ILogging, LoggingV2>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
