// Simple Test app which is used to prove the launcher class works as expected.

namespace MyApp
{
    internal class Program
    {
        private const int StopOnCount = 20;

        static CancellationTokenSource cancelToken = new CancellationTokenSource();

        static void Main(string[] args)
        {
            Console.WriteLine("Demo App: Started");
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            int counter = 0; 
            while (counter < StopOnCount)
            {
                Console.WriteLine("Demo App: Looping " + counter++);
                Task.Delay(1000).Wait();
            }

            Console.WriteLine($"Demo App: Closing itself after {counter} itterations");
        }

        private static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {
            cancelToken.Cancel();
            Console.WriteLine("Demo App: Token Cancelled");
        }
    }
}
