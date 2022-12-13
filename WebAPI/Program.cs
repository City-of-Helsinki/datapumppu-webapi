using WebAPI.LiveMeetings;

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

            builder.Services.AddSignalR(options => options.EnableDetailedErrors = true);
            
            
            // Removed for now, until we know what to do about it
            //builder.Services.AddHostedService<LiveMeetingObserver>();

            var app = builder.Build();

            app.UseCors();

            app.MapControllers();

            app.UseRouting();

            // Configure the HTTP request pipeline.
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