using CheckoutCart.BBL;
using CheckoutCart.BBL.Interface;
using CheckoutCart.BLL;
using CheckoutCart.BLL.Interface;
using CheckoutCart.DAL;
using CheckoutCart.DAL.Interface;
using CheckoutCart.Domain;
using CheckoutCart.Helpers.AutoMapper;
using CheckoutCart.Helpers.Security;
using CheckoutCart.Helpers.Security.Contants;
using CheckoutCart.Helpers.Security.Interfaces;
using CheckoutCart.Helpers.Swagger;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

builder.Services.AddScoped<IUserDao, UserDao>();
builder.Services.AddScoped<IRoleDao, RoleDao>();
builder.Services.AddScoped<IStatusDao, StatusDao>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IJwtAuthManager, JwtAuthManager>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerConfiguration();

var jwtSettings = new JwtSettings();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(nameof(JwtSettings)));

builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GateNewsApi V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
