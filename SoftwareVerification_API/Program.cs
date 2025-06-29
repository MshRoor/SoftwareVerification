using BusinessLogic.Repository;
using BusinessLogic.Repository.IRepository;
using DataAccess.Data;
using DataAccess.DbAccess;
using SoftwareVerification_API.Service.IService;
using SoftwareVerification_API.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using SoftwareVerification_API.Helper;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Microsoft.OpenApi.Models;
using Serilog;
using Hangfire;
using AppModels.Common;
using AppModels.Account;
using Microsoft.AspNetCore.Authorization;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ENGI9839CourseProject_API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please Bearer and then token in the field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                   {
                     new OpenApiSecurityScheme
                     {
                       Reference = new OpenApiReference
                       {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                       }
                      },
                      new string[] { }
                    }
                });
});
// Configure Serilog from appsettings.json
builder.Host.UseSerilog((context, services, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration) // Read settings from appsettings.json
                 .Enrich.FromLogContext() // Enrich log context
                 .Enrich.WithMachineName() // Add machine name
                 .Enrich.WithThreadId()   // Add thread id
                 .Enrich.WithEnvironmentUserName() // Add the current user
);

//my services
//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddDbContext<XcelAppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("XcelDbConnection")));
builder.Services.AddTransient<ISqlDataAccess, SqlDataAccess>();
// custom identity
//builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders();
builder.Services.AddIdentity<XcelAppUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
})
    .AddEntityFrameworkStores<XcelAppDbContext>()
    .AddDefaultTokenProviders();


var appSettingsSection = builder.Configuration.GetSection("APISettings");
builder.Services.Configure<APISettings>(appSettingsSection);

var apiSettings = appSettingsSection.Get<APISettings>();
var key = Encoding.ASCII.GetBytes(apiSettings.SecretKey);
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidAudience = apiSettings.ValidAudience.FirstOrDefault()!.ToString(),
                        ValidIssuer = apiSettings.ValidIssuer,
                        ClockSkew = TimeSpan.Zero
                    };
                });


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddTransient<IEmailService, EmailServiceMailKit>();
builder.Services.AddTransient<ISMSService, SMSService>();

//builder.Services.AddScoped<IUnitOfWorkEF, UnitOfWorkEF>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddAuthorization(options =>
{
    //options.AddPolicy(Permissions.AddUser, policy => policy.Requirements.Add(new PermissionRequirement(Permissions.AddUser)));
    foreach (var permission in Permissions.All)
    {
        options.AddPolicy(permission, policy => policy.Requirements.Add(new PermissionRequirement(permission)));
    }
});


builder.Services.AddHangfire(x =>
    x.UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangFire"))
);
builder.Services.AddHangfireServer(x =>
{
    x.SchedulePollingInterval = TimeSpan.FromSeconds(60);
    x.Queues = new[] { SD.HangFireQueueName };
});

builder.Services.AddCors(o => o.AddPolicy("SoftwareVerification", builder =>
{
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddRouting(option => option.LowercaseUrls = true);
builder.Services.AddControllers().AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNamingPolicy = null)
                .AddNewtonsoftJson(opt =>
                {
                    opt.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

//my services

var app = builder.Build();
//app.Services.CreateScope().ServiceProvider.GetService<IDbInitializer>().Initialize();

//app.UseMiddleware<SwaggerBasicAuthMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();   //// REMOVE IN PRODUCTION
app.UseSwaggerUI();  /////  REMOVE IN PRODUCTION

app.UseHttpsRedirection();

app.UseCors("SoftwareVerification"); //
app.UseAuthentication(); //

app.UseAuthorization();

// Use Serilog request logging to capture HTTP details
app.UseSerilogRequestLogging(); // This will automatically log HTTP request details

//app.UseMiddleware<BasicAuthMiddleware>(); //authentication for hangfire dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = [new HangfireBasicAuthFilter(builder.Configuration.GetSection("HangfireLogin"))],
    DarkModeEnabled = true
});

app.MapControllers();

app.Run();

// Expose Program for integration testing   --  The public partial class Program is required so the test project can bootstrap the API.
public partial class Program { }

