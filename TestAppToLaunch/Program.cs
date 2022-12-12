// Simple Test app which is used to prove the launcher class works as expected.

namespace MyApp
{
    internal class Program
    {
        private const int StopOnCount = 10; 

        static void Main(string[] args)
        {
            Console.WriteLine("Demo App: Started");

            int counter = 0; 
            while (counter < StopOnCount)
            {
                Console.WriteLine("Demo App: Looping " + counter++);
                Task.Delay(1000).Wait();
            }

            Console.WriteLine($"Demo App: Closing itself after {counter} itterations");
        }
    }
}
