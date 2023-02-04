using WebAPI.LiveMeetings;
using WebAPI.StorageClient;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebAPI.Data;

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

            builder.Services.AddSingleton<IMeetingDataProvider, MeetingDataProvider>();
            builder.Services.AddSingleton<IVotingDataProvider, VotingDataProvider>();
            builder.Services.AddSingleton<IStatementsDataProvider, StatementsDataProvider>();
            builder.Services.AddSingleton<ICache, Cache>();

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
                    ValidIssuer = builder.Configuration["JWT_ISSUER"],
                    ValidAudience = builder.Configuration["JWT_AUDIENCE"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT_KEY"]))
                };
            });
            // Removed for now, until we know what to do about it
            //builder.Services.AddHostedService<LiveMeetingObserver>();
            builder.Services.AddHostedService<KafkaLiveMeetingObserver>();

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