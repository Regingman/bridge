using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MyDataCoinBridge.Interfaces;
using MyDataCoinBridge.Services;
using System.IO;
using System;
using MyDataCoinBridge.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Text;
using MyDataCoinBridge.DataAccess;
using static System.Environment;
using DotNetEnv;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
{
    var services = builder.Services;
    var env = builder.Environment;
    services.Configure<AppSettings>(options =>
    {
        options.FCM_CONFIGURATION = DotNetEnv.Env.GetString("FCM_CONFIGURATION", "Variable not found");
        options.G_API_KEY = DotNetEnv.Env.GetString("G_API_KEY", "Variable not found");
        options.G_API_PASSWORD = DotNetEnv.Env.GetString("G_API_PASSWORD", "Variable not found");
        options.G_IMAGE_TOKEN = DotNetEnv.Env.GetString("G_IMAGE_TOKEN", "Variable not found");
        options.BRIDGE_URI = DotNetEnv.Env.GetString("BRIDGE_URI", "Variable not found");
        options.JWT_KEY = DotNetEnv.Env.GetString("JWT_KEY", "Variable not found");
        options.DB_CONNECTION = DotNetEnv.Env.GetString("DB_CONNECTION", "Variable not found");
    });
    //services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

    services.AddDbContext<WebApiDbContext>();

    services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(o =>
    {
        o.SaveToken = true;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false, // on production make it true
            ValidateAudience = false, // on production make it true
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "bridge.mydatacoin.io",
            ValidAudience = "bridge.mydatacoin.io",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(DotNetEnv.Env.GetString("JWT_KEY", "Variable not found"))),
            ClockSkew = TimeSpan.Zero
        };
        o.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Add("IS-TOKEN-EXPIRED", "true");
                }
                return Task.CompletedTask;
            }
        };
    });


    services.AddControllers().AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    );

    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyDataCoinBridge.io", Version = "v1" });
        c.EnableAnnotations();
        c.OperationFilter<AuthorizationOperationFilter>();
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Scheme = "Bearer",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
              Enter 'Bearer' [space] and then your token in the text input below.
              \r\n\r\nExample: 'Bearer 12345abcdef'"
        });

        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    });

    services.AddScoped<IProviders, ProvidersServices>();
    services.AddScoped<IUser, UserService>();
    services.AddScoped<IWebHooks, WebHooksService>();
    services.AddScoped<IJWTManager, JWTManagerService>();
    services.AddHttpClient<ProvidersServices>();
    services.AddHealthChecks();
}

var app = builder.Build();
{
    app.UseSwagger(options => { options.SerializeAsV2 = true; });
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyDataCoinBridge.io v1"); });

    Env.Load();
    app.UseDeveloperExceptionPage();

    //app.UseStaticFiles(new StaticFileOptions()
    //{
    //    FileProvider = new PhysicalFileProvider(Path.Combine("/var/www/Uploads", @"Resources")),
    //    RequestPath = new PathString("/Resources")
    //});

    app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

    app.UseHttpsRedirection();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}

app.Run();
