#script

string dd=@"
 BEGIN DIALOG
 0 '' 0x90C80AC8 0x0 0 0 224 136 'Dialog'
 3 ListBox 0x54230101 0x200 11 7 88 63 ''
 1 Button 0x54030001 0x4 116 116 48 14 'OK'
 2 Button 0x54030000 0x4 168 116 48 14 'Cancel'
 END DIALOG
 DIALOG EDITOR: '' 0x2040301 '*' '' '' ''
"
var d=new Dialog1();
if(!d.Show(dd DlgProc d)) return;


#functions

class Dialog1 :Dialog {
string lb3; int c4;

public Dialog1() {
//initialize variables if need
d.lb3="test 1[]test 2[]test 3"; //parse [] at run time. Or: d.lb3=$"test 1{_}test 2{_}test 3"; Or: d.lb3=new string[] {"test 1", "test 2", "test 3"};
}


}

Ptr DlgProc(Ptr hDlg, WM message, Ptr wParam, Ptr lParam)
{
