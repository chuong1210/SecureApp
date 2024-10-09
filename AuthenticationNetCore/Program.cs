using System.Security.Cryptography.Xml;
using System.Text;
using AuthenticationNetCore.Data;
using AuthenticationNetCore.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);

var JWTSetting = builder.Configuration.GetSection("JWTSetting");
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.AddSecurityDefinition("Bearer",
    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = @"JWT Authorization Example : 'Bearer wehaudhadawu2o34yy839",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"

    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
      new OpenApiSecurityScheme{
          Reference= new OpenApiReference
        {
            Type=  ReferenceType.SecurityScheme,
            Id="Bearer"

        },
          Scheme = "outh2",
          Name="Bearer",
          In=ParameterLocation.Header

      },
        new List<String>()

        }
    }
);
    }
);
builder.Services.AddDbContext<AppDBContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSqlServer")));
builder.Services.AddIdentity<AppUser, IdentityRole>(options=>
     {
    //options.Password.RequireDigit = true;
    //options.Password.RequiredLength = 6;
}
    ).AddEntityFrameworkStores<AppDBContext>().AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; 
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        //ValidIssuer = builder.Configuration["JWTSetting:ValidIssuer"],
        ValidIssuer = JWTSetting["ValidIssuer"],
        ValidAudience= JWTSetting[ "ValidAudicen"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTSetting.GetSection("securityKey").Value!))
    }; 
    options.SaveToken = true;
    options.RequireHttpsMetadata = true;



    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("Authentication failed: " + context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token is valid.");
            return Task.CompletedTask;
        }
    };

}); ;

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    
});



// Đăng ký CustomRoleClaim
//services.AddScoped<IRoleClaimStore<CustomRoleClaim>, RoleClaimStore<CustomRoleClaim>>();
var app = builder.Build();
app.UseCors("AllowAll");

//app.UseCors(options =>
//{
//    options.AllowAnyHeader();
//    options.AllowAnyMethod();
//    options.AllowAnyOrigin();
//});
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();
app.MapControllers();

app.Run();
