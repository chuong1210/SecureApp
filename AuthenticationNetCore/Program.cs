using System.Security.Cryptography.Xml;
using System.Text;
using AuthenticationNetCore.Data;
using AuthenticationNetCore.Models;
using AuthenticationNetCore.Repository.imp;
using AuthenticationNetCore.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);
// Configure JWT Settings (crucial!)
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var JWTSetting = builder.Configuration.GetSection("JWTSetting");
builder.Services.AddControllers();
builder.Services.AddScoped<IInvalidatedTokenRepository, InvalidatedTokenRepository>();

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
        ClockSkew = TimeSpan.Zero,
        //ValidIssuer = builder.Configuration["JWTSetting:ValidIssuer"],
        ValidIssuer = JWTSetting["ValidIssuer"],
        ValidAudience= JWTSetting["ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTSetting.GetSection("securityKey").Value!))
    }; 
    options.SaveToken = true;



    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            // Important:  Log the specific error for debugging
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token is valid.");
            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            // Important: Handle potential message errors
            // Check if the message contains a JWT
            if (context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                // Validate if the token is correctly formatted
                string authHeaderValue = authHeader.ToString();
                if (authHeaderValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    var token = authHeaderValue.Substring("Bearer ".Length);
                    // Try to parse the token, catch potential exceptions
                    try
                    {
                        context.Token = token;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing token: {ex.Message}");

                    }
                }
            }
            return Task.CompletedTask;
        }

    };
    options.RequireHttpsMetadata = false;

});

//builder.Services.AddCors(options =>
//{

//    options.AddPolicy("AllowAll",
//        builder =>
//        {
//            builder
//                .WithOrigins("http://localhost:4200", "http://localhost:4200/*", "https://www.yourfrontend.com")
//                         .WithOrigins("*")//Important
//                        //.AllowAnyOrigin()
//                      .AllowAnyMethod()
//                      .AllowAnyHeader()
//                      .AllowCredentials();
//        }
//);
//});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Replace with your allowed domain
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });

    options.AddPolicy("AllowSubdomains", policy =>
    {
        policy.WithOrigins("https://*.localhost:4200") // Allow subdomains
              .SetIsOriginAllowedToAllowWildcardSubdomains()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });

    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


//builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri("https://localhost:5001") });

// Đăng ký CustomRoleClaim
//services.AddScoped<IRoleClaimStore<CustomRoleClaim>, RoleClaimStore<CustomRoleClaim>>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//app.UseHttpsRedirection();


app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


