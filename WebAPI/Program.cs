using WebAPI.LiveMeetings;
using WebAPI.StorageClient;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddHealthChecks();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyHeader();
                    builder.AllowCredentials();
                    builder.SetIsOriginAllowed(_ => true);
                });
            });

            builder.Services.AddScoped<IStorageApiClient, StorageApiClient>();
            builder.Services.AddScoped<IStorageConnection, StorageConnection>();

            builder.Services.AddSignalR(options => options.EnableDetailedErrors = true);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });
            // Removed for now, until we know what to do about it
            //builder.Services.AddHostedService<LiveMeetingObserver>();

            var app = builder.Build();

            app.UseCors();

            app.MapControllers();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<LiveMeetingsHub>("/live");
                endpoints.MapHealthChecks("/healthz");
                endpoints.MapHealthChecks("/readiness");
            });

            app.Run();
        }
    }
}