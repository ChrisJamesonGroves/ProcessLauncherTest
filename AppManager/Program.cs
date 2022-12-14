using System.Diagnostics;
using System.Reflection;

namespace AppManager
{
    /// <summary>
    /// This is the manager app which will allow a process to be started and stopped.
    /// </summary>
    internal class Program
    {
        // This is used to identify apps to be killed. So if you change this, or create a new application
        // to be managed by this AppManager, then ensure to change the application 'company' name. This
        // is found by right clicking the Project > Properties > Package > General > Company.
        const string CompanyName = "AcmeInc";
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

            Console.WriteLine("Launcher App: \n" +
                "\tProcess Ready: \n" +
                "\tPress \n" +
                "\t'a' to start process \n" +
                "\t'b' to stop process \n" +
                "\t'i' for process information \n" +
                "\t'e' to force an exception in this manager \n" +
                "\t'p' to print processes \n" +
               $"\t'k' to kill all {CompanyName} processes \n" +
                "\t'x' to exit");

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
                    else if (key == 'e')
                    {
                        Console.WriteLine("Launcher App: 'e' Pressed; Throwing Exception");
                        throw new Exception("Test Exeption thrown");
                    }
                    else if (key == 'x')
                    {
                        Console.WriteLine("Launcher App: 'x' Pressed; Closing application");
                        keepRunning = false;
                        Task.Delay(1000);
                    }
                    else if (key == 'p')
                    {
                        Console.WriteLine("Launcher App: 'p' Pressed; Printing Processes");
                        PrintProcesses();
                    }
                    else if (key == 'k')
                    {
                        Console.WriteLine($"Launcher App: 'k' Pressed; Killing {CompanyName} Processes");
                        KillCompanyProcesses();
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

        static void PrintProcesses()
        {
            var CompanyProcesses = GetCompanyProcesses();

            if (CompanyProcesses.Count == 0) return;

            foreach (Process thisProc in CompanyProcesses)
            {
                Console.WriteLine($"{CompanyName} Process Still Running: " + thisProc.ProcessName);
            }
        }

        static void KillCompanyProcesses()
        {
            var CompanyProcesses = GetCompanyProcesses();
            int killCount = 0;

            foreach (Process thisProc in CompanyProcesses)
            {
                if (thisProc.Id == Process.GetCurrentProcess().Id)
                    continue;

                Console.WriteLine("Killing Process: " + thisProc.ProcessName);
                killCount++;
                thisProc.Kill();
            }

            Console.WriteLine($"Killed {killCount} Process(es)");
        }

        static List<Process> GetCompanyProcesses()
        {
            Process[] all = Process.GetProcesses();
            List<Process> CompanyProcesses = new List<Process>();

            foreach (Process thisProc in all)
            {
                try
                {
                    string? name = thisProc?.MainModule?.FileVersionInfo.CompanyName;

                    if (thisProc != null && name == CompanyName)
                    {
                        CompanyProcesses.Add(thisProc);
                    }
                }
                catch
                {
                    // Do nothing, we arent interested in processes without CompanyName
                }
            }

            return CompanyProcesses;
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