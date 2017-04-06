 Assume the macro runs Notepad, pastes some text and opend the Find dialog.
run "notepad.exe"
wait 10 "Notepad"
paste "text to find"
key Cf

 Then you want the macro to wait until the 'Find Next' button is enabled.
 One of possible ways - let the macro wait until the button control state is enabled.
 A control is a child window in a window, eg button, edit box, list view. However not all UI objects are controls.
 At first open Notepad and its Find dialog, let the 'Find Next' button be enabled.
 Open the 'Wait' dialog and select 'Wait for window enabled'.
 Select 'Control'. Drag the Drag tool to the 'Find Next' button.
 If the button is a control, you can see a rectangle around it. Else you cannot use this.
 Click Test. It should show a rectangle around the button in the Find dialog.
 Optionally set timeout, eg 60.
 Uncheck 'Get handle' if don't need it.
 OK. It creates code like this:
int w=win("Find" "#32770")
wait 60 WE id(1 w) ;;push button 'Find Next'

 Then the macro continues. If the button is not enabled in 60 s, the macro ends with error.
mes "The button now is enabled."

 This is the best way, however does not work in web pages and with many UI objects in applications where controls (as child windows) are not used.
