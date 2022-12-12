using System.Diagnostics;
using System.Reflection;

namespace AppManager
{
    /// <summary>
    /// This is the manager app which will allow a process to be started and stopped.
    /// </summary>
    internal class Program
    {
        static Launcher? launcher;
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            Console.WriteLine("Launcher App: Started");

            launcher = new Launcher(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\..\\..\\..\\..\\TestAppToLaunch\\bin\\Debug\\net6.0\\TestAppToLaunch.exe");

            // Close the app if the process was not initialised. No point continuing
            if (launcher.InitialiseProcess() == false)
            {
                Console.WriteLine("Launcher App: Failed to initialise process. Closing...");
                Task.Delay(1000).Wait();
                return;
            }

            Console.WriteLine("Launcher App: \n\tProcess Ready: \n\tPress \n\t'a' to start process \n\t'b' to stop process \n\t'i' for process information \n\t'x' to exit");

            // Keep running, taking approprirate action based on key presses
            bool keepRunning = true; 
            while (keepRunning)
            {
                Task.Delay(100).Wait();

                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).KeyChar;
                    if (key == 'a')
                    {
                        Console.WriteLine("Launcher App: 'a' Pressed; Starting Process");
                        launcher.StartProcess();
                    }
                    else if (key == 'b')
                    {
                        Console.WriteLine("Launcher App: 'b' Pressed; Stopping Process");
                        launcher.StopProcess();
                    }
                    else if (key == 'i')
                    {
                        Console.WriteLine("Launcher App: 'i' Pressed; Displaying process Info");
                        launcher.PrintProcessDetails();
                    }
                    else if (key == 'x')
                    {
                        Console.WriteLine("Launcher App: 'x' Pressed; Closing application");
                        keepRunning = false;
                        Task.Delay(1000);
                    }
                    else
                    {
                        Console.WriteLine("Launcher App: Invalid Key Pressed: " + key);
                    }
                }
            }
        }

        static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {
            Console.WriteLine("Launcher App: Event Handler: Process Exit: Stopping Launched Process");
            if (launcher != null)
            {
                launcher.StopProcess();
            }
        }
    }

    /// <summary>
    /// Launcher class does all the work of checking is the process is sucessfully initialised, starts and stops the process
    /// as well as handling exceptions.
    /// </summary>
    public class Launcher
    {
        Process? processReference;
        public string ProcessToLaunch { get; }
        private bool FileExists = false;
        

        public Launcher(string processToLaunch)
        {
            ProcessToLaunch = processToLaunch;
        }

        public bool InitialiseProcess()
        {
            ConstructProcess();
            return FileExists;
        }

        public void StartProcess()
        {
            if (!FileExists)
            {
                Console.WriteLine("Launcher App: Files does not exist, Cannot continue");
                return;
            }

            try
            {
                if (IsProcessRunning())
                {
                    Console.WriteLine("Launcher App: Process already running");
                    return;
                }

                processReference.Start();
                Console.WriteLine("Launcher App: Process Created: " + processReference.Id);

            }
            catch(Exception ex)
            {
                Console.WriteLine("Launcher App: Failed to create process: " + ex.Message);
            }
        }

        public void StopProcess()
        {
            if (!IsProcessRunning())
            {
                // This will happen if the process was already stopped after previosuly being started
                Console.WriteLine("Launcher App: Process Not Running");
                return;
            }

            Console.WriteLine("Launcher App: Process stopping " + processReference.HasExited);
            try
            {
                processReference.Kill();
                Console.WriteLine("Launcher App: Process killed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Launcher App: Process failed to kill: " + e.Message);
            }
        }

        public void PrintProcessDetails()
        {
            try
            {
                _ = processReference.HasExited; // Initial check to see if a process was ever started.

                string runningState = processReference.HasExited ? "not running" : "running";
                string timeTag = processReference.HasExited ? "Exited" : "Started";
                string time = processReference.HasExited ? processReference.ExitTime.ToString() : processReference.StartTime.ToString();

                Console.WriteLine($"Launcher App: Process Path    : {processReference.StartInfo.FileName}");
                Console.WriteLine($"Launcher App: Process ID      : {processReference.Id}");
                Console.WriteLine($"Launcher App: Process State   : {runningState}");
                Console.WriteLine($"Launcher App: Process {timeTag} : {time}");
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Launcher App: Process never started. No details available");
            }
        }

        private void ConstructProcess()
        {
            // Check that file exists
            if (!File.Exists(ProcessToLaunch))
            {
                Console.WriteLine("Launcher App: File does not exist: " + ProcessToLaunch);
                FileExists = false;
                return;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = ProcessToLaunch;
            Console.WriteLine(ProcessToLaunch);
            processReference = new Process();
            processReference.StartInfo = startInfo;
            FileExists = true;
        }

        private bool IsProcessRunning()
        {
            try
            {
                if (processReference == null || processReference.HasExited)
                {
                    return false;
                }
            }
            catch (InvalidOperationException)
            {
                return false;
            }

            return true;
        }
    }
} 