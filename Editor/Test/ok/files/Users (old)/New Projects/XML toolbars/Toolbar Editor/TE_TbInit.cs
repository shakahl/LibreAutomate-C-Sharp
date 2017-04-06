 /ToolbarEditor
function# ;;returns imagelist
int- htb
int i
for(i SendMessage(htb TB_BUTTONCOUNT 0 0)-1 -1 -1) SendMessage htb TB_DELETEBUTTON i 0
ImageList_Destroy SendMessage(htb TB_SETIMAGELIST 0 0)
int il=ImageList_Create(16 16 ILC_COLOR32|ILC_MASK 0 0)
SendMessage(htb TB_SETIMAGELIST 0 il)
SendMessage(htb TB_BUTTONSTRUCTSIZE sizeof(TBBUTTON) 0)
ret il
