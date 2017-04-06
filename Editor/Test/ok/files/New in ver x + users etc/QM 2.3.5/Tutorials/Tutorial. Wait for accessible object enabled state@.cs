 Assume the macro runs Notepad, pastes some text and opend the Find dialog.
run "notepad.exe"
wait 10 "Notepad"
paste "text to find"
key Cf

 Then you want the macro to wait until the 'Find Next' button is enabled.
 One of possible ways - let the macro wait until the button accessible object state is enabled.
 An accessible object is an UI object in a window, eg button, edit box, list view. Not all UI objects are accessible objects.
 At first open Notepad and its Find dialog, let the 'Find Next' button be enabled.
 Open the 'Find accessible object' dialog. It's in the floating toolbar, 'Windows, controls' menu.
 Drag the Drag tool to the 'Find Next' button.
 If the button is an accessible object, you can see a rectangle around it. Else you cannot use this.
 Click Test. It should show a rectangle around the button in the Find dialog. Note: in this case it fails because finds the 'Find accessible object' dialog itself. Need to edit window definition, eg add program name.
 Set timeout, eg 60. Else the code will not wait.
 Check 'state' in the grid. Click the numbers and the small button next to it. Click the Use cell in the 'unavailable' row to make it 'Yes'. It means that the object must not be disabled. OK.
 Test again. It should find the button when it is enabled, and not find when disabled.
 OK. It creates code like this:
int w=wait(60 WV win("Find" "#32770" "notepad"))
Acc a.Find(w "PUSHBUTTON" "Find Next" "class=Button[]id=1[]state=0x100000 1" 0x1005 60)

 Then the macro continues. If the button is not enabled in 60 s, the macro ends with error.
mes "The button now is enabled."
