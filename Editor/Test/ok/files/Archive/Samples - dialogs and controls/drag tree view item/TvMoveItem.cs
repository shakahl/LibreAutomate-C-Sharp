 /
function# htv hi hito copy

 Moves or copies treeview control item hi to the place of hito.
 If successful, returns new item handle (on move too, because the item must be deleted and new item created).
 Does not copy Vista-specific properties. If you use them, set them again.
 If hito is 0, moves/copies to the end.


if(hi=hito) ret hi

 get all properties of hi, except Vista properties
TVINSERTSTRUCTW is
TVITEMW& t=is.item
t.hItem=hi; t.mask=TVIF_IMAGE|TVIF_SELECTEDIMAGE|TVIF_PARAM|TVIF_STATE|TVIF_TEXT
t.stateMask=TVIS_BOLD|TVIS_CUT|TVIS_DROPHILITED|TVIS_EXPANDED|TVIS_EXPANDEDONCE|TVIS_EXPANDPARTIAL|TVIS_SELECTED|TVIS_OVERLAYMASK|TVIS_STATEIMAGEMASK|TVIS_USERMASK
BSTR b; t.pszText=b.alloc(300); t.cchTextMax=300
if(!SendMessage(htv TVM_GETITEMW 0 &t)) ret
int selected=t.state&TVIS_SELECTED

 insert new item with these properties
if(copy) t.mask&=~TVIF_STATE
else
	int hparent(GetParent(htv)) wp(SubclassWindow(hparent &DefWindowProcW)) ;;subclass parent window to prevent receiving select and delete messages

if(hito)
	is.hParent=SendMessage(htv TVM_GETNEXTITEM TVGN_PARENT hito)
	is.hInsertAfter=SendMessage(htv TVM_GETNEXTITEM TVGN_PREVIOUS hito)
	if(!is.hInsertAfter) is.hInsertAfter=TVI_FIRST
else is.hInsertAfter=TVI_LAST

int hinew=SendMessage(htv TVM_INSERTITEMW 0 &is)

 delete hi
if(hinew and !copy)
	if(selected) SendMessage htv TVM_SELECTITEM TVGN_CARET hinew
	SendMessage(htv TVM_DELETEITEM 0 hi)

 unsubclass
if(wp) SubclassWindow(hparent wp)

ret hinew
