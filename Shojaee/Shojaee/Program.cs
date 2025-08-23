using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MongoDB.Entities;
using Shojaee.SMSModule;
using Shojaee.UserModule;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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

app.MapGet("/", async context =>
{
    var page = "HomePage";

    if (string.IsNullOrWhiteSpace(page))
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("Page not found");
        return;
    }
    var masterFilePath = Path.Combine(app.Environment.WebRootPath, "UI", "Master.html");
    var pageFilePath = Path.Combine(app.Environment.WebRootPath, "UI", page + ".html");
    if (!File.Exists(masterFilePath))
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("Master template not found");
        return;
    }
    if (!File.Exists(pageFilePath))
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("Page not found");
        return;
    }
    var masterContent = await File.ReadAllTextAsync(masterFilePath);
    var pageContent = await File.ReadAllTextAsync(pageFilePath);
    // Replace [MainBody] with the page content
    var combinedContent = masterContent.Replace("[MainBody]", pageContent);
    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync(combinedContent);
});

app.MapGet("/{page}", async context =>
{
    var page = context.Request.RouteValues["page"]?.ToString();

    if (string.IsNullOrWhiteSpace(page))
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("Page not found");
        return;
    }
    var masterFilePath = Path.Combine(app.Environment.WebRootPath, "UI", "Master.html");
    var pageFilePath = Path.Combine(app.Environment.WebRootPath, "UI", page + ".html");
    if (!File.Exists(masterFilePath))
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("Master template not found");
        return;
    }
    if (!File.Exists(pageFilePath))
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("Page not found");
        return;
    }
    var masterContent = await File.ReadAllTextAsync(masterFilePath);
    var pageContent = await File.ReadAllTextAsync(pageFilePath);
    // Replace [MainBody] with the page content
    var combinedContent = masterContent.Replace("[MainBody]", pageContent);
    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync(combinedContent);
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
