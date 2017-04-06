 /Drag_drop_Dialog
function hwnd

 find new id
int idctrl
for(idctrl 100 10000) if(!id(idctrl hwnd)) break

int style=WS_VSCROLL|ES_AUTOVSCROLL|ES_MULTILINE|ES_WANTRETURN|WS_TABSTOP|WS_GROUP|WS_CLIPSIBLINGS
int hctrl=CreateControl(WS_EX_CLIENTEDGE "Edit" 0 style 0 100 100 50 hwnd idctrl)

XML_Save hwnd
