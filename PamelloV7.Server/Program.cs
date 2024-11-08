using PamelloV7.Server.Config;

namespace PamelloV7.Server
{
    public class Program
    {
        public static async Task Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureAPIServices(builder.Services);

            var app = builder.Build();

            await StartupAPIServices(app);
        }

        private static void ConfigureAPIServices(IServiceCollection services) {
            services.AddControllers();
        }

        private static async Task StartupAPIServices(WebApplication app) {
            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}
