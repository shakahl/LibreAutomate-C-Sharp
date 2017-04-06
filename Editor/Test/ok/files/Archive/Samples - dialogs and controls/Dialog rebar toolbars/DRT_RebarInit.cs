 /DRT_Main
function hDlg

 Creates and initializes rebar control.
 Adds all toolbar controls.

int rst=RBS_BANDBORDERS|RBS_VARHEIGHT|CCS_TOP|CCS_NODIVIDER
int hrb=CreateControl(0 "ReBarWindow32" 0 rst 0 0 0 0 hDlg 3)
ARRAY(int) atb; child "" "ToolbarWindow32" hDlg 16 0 0 atb

REBARBANDINFO rbBand.cbSize=sizeof(REBARBANDINFO)
rbBand.fMask = RBBIM_STYLE|RBBIM_CHILD|RBBIM_CHILDSIZE|RBBIM_SIZE
rbBand.fStyle = RBBS_CHILDEDGE|RBBS_GRIPPERALWAYS|RBBS_VARIABLEHEIGHT

int i
for i 0 atb.len
	int htb=atb[i]
	rbBand.hwndChild = htb
	int dwBtnSize = SendMessage(htb, TB_GETBUTTONSIZE, 0,0);
	rbBand.cyChild = dwBtnSize&0xffff+2
	rbBand.cyMinChild = rbBand.cyChild
	rbBand.cxMinChild = dwBtnSize>>16
	SendMessage(hrb, RB_INSERTBAND, -1, &rbBand);
	rbBand.fStyle|RBBS_BREAK

 todo:
 restore saved band positions using rget or IXml.
