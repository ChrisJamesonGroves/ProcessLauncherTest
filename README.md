# ProcessLauncherTest
A little C# test solution that manages the starting and stopping of a process.

This contains 2 projects; 

**AppManager** is the main application for Starting, Stopping and getting information about the secondary app. 
This prevents multiple of the same application from being launched and will not crash if trying to stop the app 
when its not running.

This can currently:
* Start the process - This will only allow one instance of the process to be started at a time
* Stop the process
* Get process information - This will print basic information including process name and current state (running/ not running)
* Force an exception - This is to prove that if the AppManager crashes the launched app will become orphaned.
* Print all apps running which has the corresponding Company name (currently set to 'AcmeInc'). Details to change this are in AppManager > Process.cs file
* Kill all Processes - This will kill all the apps with the corresponding Company name (currently set to 'AcmeInc'). Details to change this are in AppManager > Process.cs file

To manage an application simply add the application file path to the AppManager > Main method. This is currently set to 
launch the TestAppToLaunch example app.

**TestAppToLaunch** which represents a simple demo application that is to be managed by the AppManager. This simply loops around, 
printing the count number every second. After 10 cycles it closes its self. This app is used to prove the AppManager
works as expected, including if the launched app closes itself.
