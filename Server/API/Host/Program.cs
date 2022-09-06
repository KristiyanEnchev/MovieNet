namespace Host
{
    using Microsoft.AspNetCore.Builder;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                var app = builder.Build();

                app.Run();
            }
            catch (Exception ex) when (!ex.GetType().Name.Equals("HostAbortedException", StringComparison.Ordinal))
            {
            }
            finally
            {
            }
        }
    }
}