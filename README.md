# ProcessLauncherTest
A little C# test solution that manages the starting and stopping of a process.

This contains 2 projects; 

**AppManager** is the main application for Starting, Stopping and getting information about the secondary app. 
This prevents multiple of the same application from being launched and will not crash if trying to stop the app 
when its not running.

To manage an application simply add the application file path to the AppManager > Main method. This is currently set to 
launch the TestAppToLaunch example app.

**TestAppToLaunch** which represents a simple demo application that is to be managed by the AppManager. This simply loops around, 
printing the count number every second. After 10 cycles it closes its self. This app is used to prove the AppManager
works as expected, including if the launched app closes itself.
