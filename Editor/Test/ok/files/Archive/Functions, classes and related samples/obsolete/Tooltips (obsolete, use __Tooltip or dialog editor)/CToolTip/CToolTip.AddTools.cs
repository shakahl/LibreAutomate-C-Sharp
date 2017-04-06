function hDlg $text

 Assigns tooltip text to multiple controls.
 Can be used instead of calling AddTool for each control.

 text - multiline string where each line consists of control id, single space and tooltip text.

 EXAMPLE
 tt.AddTools(hDlg "3 button[]4 edit")

str s
int cid le
foreach s text
	cid=val(s 0 le)
	if(!cid or !le or le=s.len) continue
	AddTool(hDlg cid s+le+1)
