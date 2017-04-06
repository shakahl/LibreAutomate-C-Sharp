#script

string dd=@"
 BEGIN DIALOG
 0 '' 0x90C80AC8 0x0 0 0 224 136 'Dialog'
 3 ListBox 0x54230101 0x200 11 7 88 63 ''
 4 Button 0x54012003 0x0 104 8 48 10 "Check"
 1 Button 0x54030001 0x4 116 116 48 14 'OK'
 2 Button 0x54030000 0x4 168 116 48 14 'Cancel'
 END DIALOG
 DIALOG EDITOR: '' 0x2040301 '*' '' '' ''
"
var v=new DialogVariables();
v.lb3="test 1[]test 2[]test 3"
if(!ShowDialog(dd DlgProc v)) return;

#functions

class DialogVariables { public object lb3, c4Che; }

Ptr DlgProc(Ptr hDlg, WM message, Ptr wParam, Ptr lParam)
{
switch(message) {
case WM_INITDIALOG:
	IntPtr hlb=Control(hDlg, 3); ;;list
	api.SetFocus(hlb);
	api.SetWindowSubclass(hlb, _WndProc_Subclass, 1, 0);
	break;
case WM_DESTROY: break;
case WM_COMMAND: goto messages2;
}
return 0;
messages2:
switch(wParam)
{
case IDOK: break;
case IDCANCEL: break;
case LBN_DBLCLK<<16|3:
	int i=LB.SelectedItem(lParam);
	Out($"selected test {i+1}");
	break;
}
return 1;
}

static api.SubClassProcDelegate _WndProc_Subclass=WndProc_Subclass;
IntPtr WndProc_Subclass(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, UIntPtr uIdSubclass, IntPtr dwRefData)
{
}






#region WINAPI
public delegate IntPtr SubClassProcDelegate(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, UIntPtr uIdSubclass, IntPtr dwRefData);

[DllImport("comctl32")]
public static extern bool SetWindowSubclass(IntPtr hWnd, SubClassProcDelegate pfnSubclass, UIntPtr uIdSubclass, IntPtr dwRefData);

[DllImport("comctl32")]
public static extern IntPtr DefSubclassProc(IntPtr hWnd, uint uMsg, IntPtr WPARAM, IntPtr LPARAM);
#endregion

#region alternatives
 to init lb/cb variables, support all these:
v.lb3="test 1[]test 2[]test 3"; //parse [] at run time
v.lb3=$"test 1{_}test 2{_}test 3" //multiline
v.lb3=new string[] {"test 1", "test 2", "test 3"};

For single-sel lb/cb variable ShowDialog sets int selected index. For multi-sel - bool[].

#endregion
