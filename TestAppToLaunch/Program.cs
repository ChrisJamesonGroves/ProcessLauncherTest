// Simple Test app which is used to prove the launcher class works as expected.

namespace MyApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Secondary App Started");

            _ = new Worker();

            while (true)
            {
                Task.Delay(1000).Wait();
            }
        }
    }
}

public class Worker
{
    public Worker()
    {
        _ = DoWork();
    }

    private async Task DoWork()
    {
        int counter = 0;
        while (true)
        {
            Console.WriteLine("Looping " + counter++);
            await Task.Delay(1000);
        }
    }
}