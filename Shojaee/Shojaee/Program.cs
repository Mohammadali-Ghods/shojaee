using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MongoDB.Entities;
using Shojaee.SMSModule;
using Shojaee.UserModule;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<JWTHandlingService>();
builder.Services.AddScoped<SMSService>();

var key = Encoding.ASCII.GetBytes("@789312sdahdkjs@!#123jkkash@#12/**4654dsa978321@!|}{dsahkjdsajgjghf");
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = false;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true, // Validate the expiration of the token
        ClockSkew = TimeSpan.Zero // Optional: set it to zero so tokens expire exactly at token expiration time
    };
});

var app = builder.Build();

await DB.InitAsync("Shojaee",
    MongoClientSettings.FromConnectionString(
        "mongodb://20.52.99.105:8001"));

app.UseCors(c =>
{
    c.AllowAnyHeader();
    c.AllowAnyMethod();
    c.AllowAnyOrigin();
});

app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
