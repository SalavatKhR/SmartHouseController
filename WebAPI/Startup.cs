using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MQTTnet;
using WebAPI.Data;
using WebAPI.Hubs;
using WebAPI.Models;

namespace WebAPI;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        _configuration = configuration;
        _env = env;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
            options.UseOpenIddict();
        });
        services.AddIdentity();
        services.AddSingleton<MqttFactory>();
        services.AddSingleton<IConnections, Connections>();
        services
            .AddAuthenticationAndJwt(_configuration)
            .AddAuthorization()
            .AddOpenIddictServer(_env);
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSignalR();
        services.AddCors(options =>
        {
            options.AddPolicy(name: "policyName",
                builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });
        services.AddSwaggerGen(option =>
        {
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
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
            option.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SmartHouseController API",
                Version = "v1"
            });
        });
    }
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = "swagger";
            });
        }
        
        app
            .UseStaticFiles() //for wwwroot
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseCors("policyName")
            .UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ControllersHub>("/hub");
                endpoints.MapDefaultControllerRoute();
            });
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host
            .CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, builder) => builder
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true)
                .AddEnvironmentVariables())
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
    public static void RunApp(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }
}