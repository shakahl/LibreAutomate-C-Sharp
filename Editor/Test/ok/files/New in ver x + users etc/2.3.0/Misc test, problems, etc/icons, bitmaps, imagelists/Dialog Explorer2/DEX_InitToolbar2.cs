 /DEX_Main2
function hDlg htb

DEX_DATA2& d=+DT_GetParam(hDlg)

SetWinStyle htb TBSTYLE_FLAT|TBSTYLE_TOOLTIPS 1
SendMessage htb TB_SETIMAGELIST 0 d.il.GetImageList

ARRAY(TBBUTTON) a.create(3)
ARRAY(str) as="One[]Two[]Three"
int i
for i 0 a.len
	TBBUTTON& t=a[i]
	t.idCommand=1001+i
	t.iBitmap=i
	t.iString=SendMessage(htb TB_ADDSTRINGA 0 as[i]) ;;note: the string must be terminated with two 0. str variables normally have two 0, but if you will use unicode...
	t.fsState=TBSTATE_ENABLED

SendMessage(htb TB_BUTTONSTRUCTSIZE sizeof(TBBUTTON) 0)
SendMessage(htb TB_ADDBUTTONS a.len &a[0])
