using POC_Services.Middlewares;
using Microsoft.AspNetCore.Authentication.Negotiate;
using POC_Services.OAuth;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

//builder.Configuration["Auth0:Domain"]
var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true).Build();
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOAuthAuthentication(config);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();
}));
builder.Services.AddSwaggerGen();
//builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//--------------------- For Authentication middleware --------------------------------------------------------

//builder.Services.AddTransient<Authentication1Middleware>();

//-------------------------- For Authorization config --------------------------------------------------------

//builder.Services.AddAuthorization(options =>
//{
//    // By default, all incoming requests will be authorized according to the default policy.
//    options.FallbackPolicy = options.DefaultPolicy;
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("corsapp");
//--------------------- For Authentication middleware --------------------------------------------------------
//app.UseMiddleware<Authentication1Middleware>();
//------------------------------------------------------------------------------------------------------------
app.UseAuthentication();
app.UseAuthorization();



app.MapControllers();

app.Run();
