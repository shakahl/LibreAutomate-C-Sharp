/Dialog_Editor
str s=
 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 175 "Dialog"
 3 Static 0x54000000 0x0 6 6 212 18 "Click OK and make the computer sleep (not hibernate). Note that the macro does not do it for you."
 4 Static 0x54000000 0x0 6 28 106 13 "The macro will wake it up after"
 5 Edit 0x54032000 0x200 114 26 22 15 ""
 6 Static 0x54000000 0x0 138 28 82 13 "seconds,"
 7 Static 0x54000000 0x0 6 44 106 13 "and perform a sample task for"
 8 Edit 0x54032000 0x200 114 42 22 15 ""
 9 Static 0x54000000 0x0 138 44 48 13 "seconds."
 11 Button 0x54032009 0x0 6 62 136 12 "Wake up normally"
 12 Button 0x54002009 0x0 6 74 136 12 "Wake up temporarily, with display"
 13 Button 0x54002009 0x0 6 86 136 12 "Wake up temporarily, without display"
 10 Static 0x54000000 0x0 6 104 214 48 "On temporary wakeup, the computer automatically goes to sleep when the macro ends, but not earlier than after 2 minutes. However it doesn't if you or the/a macro use the mouse or keyboard or activate windows. You can read more about the functions in the MSDN Library."
 1 Button 0x54030001 0x4 118 156 48 14 "OK"
 2 Button 0x54030000 0x4 170 156 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2020105 "" "" ""

str controls = "5 8 11 12 13"
str e5 e8 o11Wak o12Wak o13Wak
e5=60
e8=60
o11Wak=1
if(!ShowDialog("" 0 &controls)) ret

int fl; if(o12Wak=1) fl=1; else if(o13Wak=1) fl=2
WakeUpAfter val(e5) fl

 sample task

str dd=
 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 155 39 "sample task"
 2 Button 0x54030000 0x4 52 22 48 14 "Cancel"
 3 msctls_progress32 0x54030000 0x0 2 4 152 14 ""
dd+"[]END DIALOG"

opt waitmsg 1
int hDlg=ShowDialog(dd 0 0 0 3)
int i n=val(e8)*10
for i 0 n
	SendMessage id(3 hDlg) PBM_SETPOS 100.0/n*i 0; err ret
	0.1
clo hDlg; err
