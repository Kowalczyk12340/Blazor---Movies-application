using System.Timers;
using Timer = System.Timers.Timer;

namespace BlazorMovies.Client.Auth
{
    public class TokenRenewer : IDisposable
    {
        private readonly ILoginService _loginService;
        Timer timer;

        public TokenRenewer(ILoginService loginService)
        {
            _loginService = loginService;
        }

        public void Initiate()
        {
            timer = new Timer();
            timer.Interval = 1000 * 60 * 4;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("timer elapsed");
            _loginService.TryRenewToken();
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
