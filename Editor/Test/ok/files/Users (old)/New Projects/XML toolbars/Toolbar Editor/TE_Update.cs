 /ToolbarEditor

int- htb hed
int il=TE_TbInit

str s ss
ss.getwintext(hed)
if(!ss.len) ret

ARRAY(TBBUTTON) a
foreach s ss
	TBBUTTON& t=a[]
	if(s="-")
		t.fsStyle=TBSTYLE_SEP
		t.iBitmap=-2
	else
		int hi=GetFileIcon(s)
		if(hi)
			t.iBitmap=ImageList_ReplaceIcon(il -1 hi)
			DestroyIcon hi
		else
			t.iBitmap=-2
			 out "failed to extract icon: %s" 1 s
		t.idCommand=a.len+100
		t.fsState=TBSTATE_ENABLED

if(!a.len) ret
SendMessage(htb TB_ADDBUTTONS a.len &a[0])
