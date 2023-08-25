using WebAPI.LiveMeetings;
using WebAPI.StorageClient;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebAPI.Data;
using WebAPI.Data.Statistics;

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
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyHeader();
                    policy.AllowCredentials();
                    policy.SetIsOriginAllowed((host) => builder?.Configuration["ALLOWED_HOSTS"].Split(",").Contains(host) ?? false);
                });
            });

            builder.Services.AddSingleton<IMeetingDataProvider, MeetingDataProvider>();
            builder.Services.AddSingleton<IVotingDataProvider, VotingDataProvider>();
            builder.Services.AddSingleton<IStatementsDataProvider, StatementsDataProvider>();
            builder.Services.AddSingleton<ISeatsDataProvider, SeatsDataProvider>();
            builder.Services.AddSingleton<IAgendaSubItemsProvider, AgendaSubItemsProvider>();
            builder.Services.AddSingleton<IReservationsDataProvider, ReservationsDataProvider>();
            builder.Services.AddSingleton<IPersonStatementsProvider, PersonStatementsProvider>();
            builder.Services.AddSingleton<IStatementStatisticsDataProvider, StatementStatisticsDataProvider>();
            builder.Services.AddSingleton<IVotingStatisticsDataProvider, VotingStatisticsDataProvider>();
            builder.Services.AddSingleton<IPersonStatementStatisticsDataProvider, PersonStatementStatisticsDataProvider>();
            builder.Services.AddSingleton<IParticipantStatisticsDataProvider, ParticipantStatisticsDataProvider>();

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

            builder.Services.AddHostedService<KafkaLiveMeetingObserver>();

            builder.Services.AddLogging(options =>
            {
                options.AddSimpleConsole(c =>
                {
                    c.IncludeScopes = true;
                    c.SingleLine = false;
                    c.TimestampFormat = "dd.MM.yyyy HH:mm:ss ";
                });
            });


            var app = builder.Build();
            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

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