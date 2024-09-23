using MatrixRemote_RemoteAPI.Data;
using MatrixRemote_RemoteAPI.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog; //maybe take out? whether or not to use serilog

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
//    .WriteTo.File("log/remoteLogs.txt", rollingInterval: RollingInterval.Year).CreateLogger();

//builder.Host.UseSerilog();



builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseNpgsql(builder.Configuration.GetConnectionString("WebApiDatabase"));
});

builder.Services.AddControllers(option =>
{
    option.ReturnHttpNotAcceptable = true;
    option.RespectBrowserAcceptHeader = true; // This ensures that the server respects the Accept header
}).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();

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
