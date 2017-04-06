 /
function# lineindex ~text [$toolbarname]

 Replaces toolbar button text.
 The replacement is permanent, ie it changes toolbar text.
 Can be called while the toolbar is running or not.
 Returns 1 if successful, 0 if not.

 lineindex - 0-based line index in toolbar text.
 text - new text of the button.
 toolbarname - toolbar name. Can be omitted or "" if called from the toolbar itself.

 EXAMPLE toolbar
 0 :out 0
 1 :ReplaceToolbarButtonText 0 "1"
 2 :ReplaceToolbarButtonText 0 "2"


int iid
if(len(toolbarname)) iid=qmitem(toolbarname)
else iid=getopt(itemid 3)
if(!iid) ret

str s.getmacro(iid)
int i=findl(s lineindex); if(i<0) ret
int j=find(s " :" i); if(i<0) ret
s.replace(text i j-i)
s.setmacro(iid); err ret
ret 1
