 \
function hwnd idObject idChild

 Opens and closes QM toolbars when you open a folder window or navigate to another folder in a folder window.
 Runs when the address bar name changes. The name is the folder path; it is not displayed as raw path.
 Tested on Windows 7 and 10. Will not work on XP.
 Need to edit this function: change/add/remove the code that opens (mac) and closes (clo) toolbars.
 For non-English Windows edition users:
   If does not work, need to replace the word "Address" in this function and in its trigger.
   To discover it, capture the address bar with the "Find accessible object" dialog and you'll see it in the Name field.
str addressBarName="Address"

#region don't edit this
wait 10 WV hwnd; err ret ;;detects the name change when closing the window
str s.getwintext(hwnd); s.remove(0 addressBarName.len+2)
int w=GetAncestor(hwnd 2) ;;hwnd is the address bar control, and now we need the folder window
#endregion

 Edit the following code.
 This code works only if you need 1 such toolbar. Can be modified to support multiple toolbars, but it is not so simple.

out s ;;remove this when this macro works

lpstr macro="WINDOWS FOLDER TOOLBAR"
int tb=win(macro "QM_toolbar")
if matchw(s "C:\Windows*" 1)
	if(tb=0) mac macro w
else
	if(tb!=0) clo tb
