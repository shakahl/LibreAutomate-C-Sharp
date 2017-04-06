 Assume the macro runs Notepad, pastes some text and opend the Find dialog.
run "notepad.exe"
wait 10 "Notepad"
paste "text to find"
key Cf

 Then you want the macro to wait until the 'Find Next' button is enabled.
 One of possible ways - capture button image in enabled state, and let the macro wait for the image.
 To create code, use the 'Find image' dialog. It's in the floating toolbar, 'Windows, controls' menu.
 At first open Notepad and its Find dialog, let the 'Find Next' button be enabled.
 Open the 'Find image' dialog and click Capture.
 Draw a small rectangle inside the 'Find Next' button. Note that the F is sometimes underlined sometimes not, therefore it's better to not include that part.
 Click OK in the popup menu.
 Click Test. It should show a rectangle around the captured image in the Find dialog.
 In the first combo box select 'Wait for'. Initially it is 'Find'.
 Optionally set timeout, eg 60.
 Uncheck 'Move mouse' if don't need it.
 OK. It creates code like this:
int w=win("Find" "#32770")
wait 60 S "Macro2001.bmp" id(1 w) 0 16|0x400 ;;push button 'Find Next'

 Then the macro continues. If the image not found in 60 s, the macro ends with error.
mes "The button now is enabled."
