using System.Runtime.InteropServices;
namespace Ro32
{
    internal static class Program
    {
        [STAThread]
        static async Task Main(string[] args)
        {
            ApplicationConfiguration.Initialize();

            Application.Run(new Ro32(args));
        }

    }
}