 At first, get filenames of all jpg files in A: for easy access later. To insert code for this, use "Enumerate files" dialog.
  declare array for filenames
ARRAY(str) ja
  enumerate jpg files in A: and write filenames to it
Dir d; str sPath
foreach(d "A:\*.jpg" FE_Dir)
	sPath=d.FileName(1)
	out sPath ;;this is just for debugging
	ja[ja.redim(-1)]=sPath

 Open template in Microsoft Publisher. Use "Run program" dialog, or "Open document" dialog.
run "path to Microsoft Publisher executable" "path to template"

 I don't understand what here is "goes to background" and "goes to foreground". Let's say that it is two windows.

 Find and activate "background" window. Use "find window" dialog and "window actions" dialog.
int hback=win("background window title")
act hback
 
 Then, somehow select the "client name" input field, and type client's name. Try to record, or use keys ("Keys" dilog), or mouse ("Mouse" dialog). If cannot be selected using keyboard or mouse, try "Accessible object actions" dialog.
 example with mouse and keys
lef 300 300 hback ;;left click it to set keyboard focus
key "Client's name" ;;and

 Find and activate "foreground" window.
int hfore=win("foreground window title")
act hfore

 Since I don't know how photo boxes in Microsoft Publisher are selected,
 and filenames entered, in following code I provide just comments.
 Try to implement it using keyboard and mouse.

int i; str filename ;;variables for accessing filenames in array ja
rep 6 ;;repeat following tab-indented code 6 times
	 select first photo box
	 ...
	filename=a[i]; i+1 ;;get next file
	 insert the file
	 ...
	 select second box, and so on, for all 6 photos
	 then, select next page
	