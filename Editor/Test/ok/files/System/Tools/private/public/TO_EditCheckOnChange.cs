 /
function &hDlg idCtrl1 idCheck1 [idCtrl2] [idCheck2] [idCtrl3] [idCheck3] [idCtrl4] [idCheck4] [idCtrl5] [idCheck5] [idCtrl6] [idCheck6] [idCtrl7] [idCheck7] [idCtrl8] [idCheck8]

 When user changes text in an edit control, checks a checkbox control.
 Call on WM_COMMAND.

int message wParam; memcpy &message &hDlg+4 8

if(message!WM_COMMAND or wParam>>16!EN_CHANGE) ret
int* p=&idCtrl1
int i idc=wParam&0xffff
for i 0 getopt(nargs)-1 2
	if p[i]=idc
		but+ id(p[i+1] hDlg 1); err
		ret
