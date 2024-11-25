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
using System.Runtime.InteropServices;
using System.Text;
using User.Management.Service.Models;
using User.Management.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;

// Database context using Npgsql (PostgreSQL)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("WebApiDatabase")));

// Identity service configuration
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Configure Identity options
builder.Services.Configure<IdentityOptions>(options => options.SignIn.RequireConfirmedEmail = true);

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = true; // Ensure HTTPS is required for JWT
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
    };
});

// Email service configuration
builder.Services.Configure<EmailConfiguration>(configuration.GetSection("EmailConfiguration"));
builder.Services.AddScoped<IEmailService, EmailService>();

// CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policyBuilder =>
        {
            policyBuilder.WithOrigins("http://localhost:5173")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

// Controller configuration
builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = false;
    options.RespectBrowserAcceptHeader = true;
})
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
    options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
})
.AddXmlDataContractSerializerFormatters();

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo
    {
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

// Add logging service
builder.Services.AddSingleton<ILogging, LoggingV2>();

// Configure Kestrel to listen on specific HTTP/HTTPS ports
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5110); // HTTP port

    // Check if running on Windows or Linux
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
        // Path for Windows environment
        serverOptions.ListenAnyIP(7033, listenOptions =>
        {
            listenOptions.UseHttps(@"C:\Users\Matt\Downloads\certificate.pfx", "WesleyStone");
        });
    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    {
        // Path for Raspberry Pi (Linux environment)
        serverOptions.ListenAnyIP(7033, listenOptions =>
        {
            listenOptions.UseHttps("/home/matt/.dotnet/corefx/cryptography/x509stores/my/337F18918694D6ABCF281D663E64EF3E4588CCB7.pfx");
        });
    }
});

var app = builder.Build();

//generic logging for troubleshooting
app.Use(async (context, next) =>
{
    try
    {
        await next.Invoke();
    }
    catch (Exception ex)
    {
        Log.Error("An unhandled exception occurred.", ex);
        throw;
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigins");

app.UseHttpsRedirection();  // Automatically redirects HTTP to HTTPS

app.UseAuthentication();  // Add authentication middleware before authorization
app.UseAuthorization();

app.MapControllers();

app.Run();
