 /dlg_drag_test
function button TVDRAG49&td

TVHITTESTINFO ht
xm ht.pt; ScreenToClient(td.htv &ht.pt)
int hidrop=SendMessage(td.htv TVM_HITTEST 0 &ht)
int candrop=hidrop or ht.flags&TVHT_NOWHERE

if(button)
	SendMessage td.htv TVM_SELECTITEM TVGN_DROPHILITE 0
	if(!candrop) ret
	ret TvMoveItem(td.htv td.hidrag hidrop GetMod=2)
else
	SendMessage td.htv TVM_SELECTITEM TVGN_DROPHILITE hidrop
	if(!candrop) ret 3
	ret iif(GetMod=2 2 1)

 instead of TVM_SELECTITEM could use TVM_SETINSERTMARK, but then need more calculations
