using MatrixRemote_RemoteAPI.Data;
using MatrixRemote_RemoteAPI.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
//    .WriteTo.File("log/remoteLogs.txt", rollingInterval: RollingInterval.Year).CreateLogger();

//builder.Host.UseSerilog();


// For db
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("WebApiDatabase")));

// For identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Add Auth
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
});

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
builder.Services.AddSwaggerGen();
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
