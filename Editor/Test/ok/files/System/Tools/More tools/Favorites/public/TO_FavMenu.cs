function [action] ;;action: 0 show, 1 manage (show menu with items Add etc; must be called from the toolbar)

 Shows or manages toolbar TO_Favorites.


sel action
	case 0 sub.Show
	case 1 sub.Manage


#sub Show
QMITEM q
int i=qmitem("TO_Favorites" 1 &q)
if !i
	i=newitem("TO_Favorites" "" "TO_FavTempl" "" "\User" 2)
	int new=1
else
	str s.getmacro(i)
	if findrx(s "(?m)^.+? :TO_FavMenu 1 \*")<0
		s.findreplace("Add to favorites :TO_FavAdd * $qm$\favorites.ico[]Edit favorites :mac+ getopt(itemid) * $qm$\favorites.ico[]") ;;QM 214 -> 215
		s.replacerx("(?m)^.+? :TO_FavOrganize [^[]]+([])?") ;;QM 242 -> 243
		s.addline("Manage favorites :TO_FavMenu 1 * $qm$\favadd.ico")
		s.setmacro(i)

if(q.itype=3 or new) mac i _hwndqm
else mac i

err+ mes F"Error: {_error.description}"


#sub Manage
sel ShowMenu("Add[]Remove[]Edit favorites[]-[]Close" 0 0 2)
	case 1 sub.AddRemove 0
	case 2 sub.AddRemove 1
	case 3 mac+ "TO_Favorites"
	case 5 clo TriggerWindow
err+


#sub AddRemove
function remove

 Adds current dialog and action to favorite actions.

 To make a dialog compatible with favorites, its dialog procedure in response to WM_USER+7 message must call function TO_FavRet.
 If dialog uses a list of actions (listbox or combobox) then:
   1. Function that shows dialog must have at least 3 parameters. Third parameter receives action index.
   2. Before calling ShowDialog must be called function TO_FavSel with that index.
   3. In response to WM_INITDIALOG message must be shown controls for that action.


int i h ha im ct
str s sm st sf
__FAVRET f

h=win("" "#32770" "QM" 0x8000 &sub.Callback &f)
if(!h)
	mes "To add or remove a dialog (or an action from a dialog) to the Favorites toolbar, at first open the dialog and select the action." "" "i"
	ret

sm.getmacro(f.dlg 1)
st.getwintext(h)
if(f.ctrl)
	ha=id(f.ctrl h); ct=iif(WinTest(ha "ComboBox") CB_GETCURSEL LB_GETCURSEL)
	i=sub_sys.CBLB_SelectedItem(ha ct=LB_GETCURSEL _s)
	if(_s~st=0) st+F" - {_s}"

s=F"{st} :TO_Fav ''{sm}'' {i}"
sub_to.Trim s " 0"
if(f.icon.len) s+F" * {f.icon}"
s+"[]"
sf=F"(?m)^.+ :TO_Fav ''\Q{sm}\E'' {i}"
sub_to.Trim sf " 0"; sf+"( +\*.*)?[]"

im=qmitem("TO_Favorites" 1)
sm.getmacro(im)

i=sm.replacerx(sf)
if(remove) if(!i) ret
else sm-s

sm.setmacro(im)

err+ mes F"Error: {_error.description}"


#sub Callback
function# hwnd __FAVRET&f

SendMessage(hwnd WM_USER+7 0 &f)
if(f.dlg) ret
ret 1
