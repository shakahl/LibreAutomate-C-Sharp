
 Dialog definition. To edit it, click the 'Dialog Editor' button on the toolbar.
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 Static 0x54000200 0x0 8 8 48 12 "Edit"
 4 Edit 0x54030080 0x200 64 8 72 12 ""
 5 Button 0x54012003 0x0 144 8 72 12 "Check"
 6 Static 0x54000200 0x0 8 28 48 13 "ComboBox"
 7 ComboBox 0x54230243 0x0 64 28 72 213 ""
 8 ComboBox 0x54230242 0x0 144 28 72 213 ""
 9 Static 0x54000000 0x0 8 48 48 12 "ListBox"
 10 ListBox 0x54230101 0x200 64 48 72 32 "Single-selection" "Single-selection listbox"
 11 ListBox 0x54230109 0x200 144 48 72 32 "Multi-selection" "Multi-selection listbox"
 12 Static 0x54000000 0x0 8 88 48 12 "Text"
 13 Static 0x54000003 0x0 64 88 16 16 ""
 14 Static 0x54000000 0x0 144 88 48 12 ""
 15 Button 0x54032000 0x0 8 116 48 14 "Button"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0 "*" "" "" ""

 Variables for dialog controls. The Dialog Editor updates these 2 lines of code. Don't edit directly.
str controls = "4 5 7 8 10 11 13"
str e4 c5Che cb7 cb8 lb10Sin lb11Mul si13

 You can assign values to some variables to initialize corresponding controls.
 The Dialog Editor does not use this code; just updates variable names if changed.
 Example:
cb7="&Zero[]One[]Two" ;;add 3 items to the ComboBox control. Use & to select an item.
lb10Sin=cb7
lb11Mul="Zero[]One[]&Two" ;;add 3 items to the multi-select ListBox control. Use & to select an item.
si13="$qm$\info.ico" ;;show an icon file in the static icon control. For 32x32 icon prepend &.

 Show the dialog, wait until the user closes it. Exit on Cancel. You can edit this line, for example pass more arguments.
if(!ShowDialog(dd &sub.DlgProc &controls)) ret

 Now you have control values in the variables.

#opt nowarnings 1 ;;this statement at the end of function disables warnings "possibly unused variable" when checked menu -> Run -> Compiler Options -> Show unused variables. Don't delete variables added by the Dialog Editor because they must match the controls variable.


#sub DlgProc ;;dialog procedure. You can change its name. A macro can contain multiple dialogs with dialog procedures with different names.
function# hDlg message wParam lParam

 This function should always begin with 'function# hDlg message wParam lParam' and contain 'sel message...' and 'sel wParam...' statements.
 The Dialog Editor uses its text to find/add case statements created by the Events dialog.
 You can edit everything else here.

str s; int i ;;we will use these local variables in several places. Note that local variables are created/destroyed on each message.
int-- t_initalizing t_timerCounter ;;use thread variables where need. They are destroyed when the dialog is closed and the thread ends.

sel message ;;Windows messages received by the dialog
	case WM_INITDIALOG
	t_initalizing=1
	s="edit me"; s.setwintext(id(4 hDlg)) ;;add some text to the Edit control. Or use: EditReplaceSel hDlg 4 "edit me" 1
	CheckDlgButton hDlg 5 1 ;;make the Check control checked. Or use but+ 5 hDlg, but it sends notification like when the user clicks.
	
	t_timerCounter=0
	SetTimer hDlg 10 1000 0 ;;let this dialog receive WM_TIMER messages every 1000 ms, wParam=10
	t_initalizing=0
	
	case WM_TIMER
	sel wParam
		case 10
		t_timerCounter+1
		s=t_timerCounter; s.setwintext(id(14 hDlg))
		
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
if(t_initalizing) ret 1 ;;ignore all control notifications sent while we are in WM_INITDIALOG
sel wParam ;;messages from controls of these classes: Button, Edit, ComboBox, ListBox, Static and some other
	case IDOK ;;the OK button clicked
	s.getwintext(id(4 hDlg)); out "Edit: %s" s ;;get text of the edit control
	if(but(5 hDlg)) out "checked"; else out "unchecked" ;;is checked?
	out "ComboBox1: item %i selected" CB_SelectedItem(id(7 hDlg)) ;;get the selected item of the read-only combo
	s.getwintext(id(8 hDlg)); out "ComboBox2: %s" s ;;get text of the editable combo
	out "ListBox1: item %i selected" LB_SelectedItem(id(7 hDlg)) ;;get the selected item of the first listbox
	
	 show message box
	MES m1.hwndowner=hDlg; m1.style="YN?"
	sel mes("Close dialog?" "" m1)
		case 'Y'
		case else ret ;;if returns 0 on IDOK or IDCANCEL, the dialog will not be closed
	
	 Below are examples of case statements to handle other messages sent by controls.
	 The numbers 15, 5, 4, 7 etc are controls ids, as in the dialog definition.
	 The case statements can be added with the Events dialog in the Dialog Editor.
	
	case 15
	out "Button clicked"
	
	case 5
	out "Check clicked. Now it is %s." iif(but(lParam) "checked" "unchecked")
	
	case EN_CHANGE<<16|4
	out "Edit text changed. Now it is ''%s''" s.getwintext(lParam)
	
	case CBN_SELENDOK<<16|7 ;;the user selected an item in ComboBox1
	_i=CB_SelectedItem(lParam)
	out "ComboBox1: item %i selected" _i
	
	case CBN_DROPDOWN<<16|8 ;;the user clicked ComboBox2 arrow to show the drop-down list
	SendMessage lParam CB_RESETCONTENT 0 0 ;;remove all items
	for(i 0 3) s.getl("Filled[]on[]dropdown" i); CB_Add(lParam s) ;;add new items
	
	case LBN_SELCHANGE<<16|10 ;;the user selected an item in ListBox1
	_i=LB_SelectedItem(lParam)
	out "ListBox1: item %i selected" _i
ret 1

  QUICK HELP
  1. In the dialog procedure you can add code to execute in response to certain events (messages) while the dialog is open.
  2. The dialog procedure is implicitly called whenever the dialog receives a message.
  3. It is called multiple times. Values of local variables are not preserved. If need to preserve, you can: 1. Use thread variables. 2. Use variables of #sub parent function if #sub has attribute v. 3. Use SetProp/GetProp.
  4. To execute code when the dialog is just created but still invisible, place the code under case WM_INITDIALOG. The code must be tab-indented, as well as under other case lines.
  5. To execute code when the dialog is being destroyed, place it under case WM_DESTROY.
  6. To execute code when the user clicks OK, place the code under case IDOK. When Cancel - under case IDCANCEL.
  7. To prevent closing the dialog on OK/Cancel, return 0 under case IDOK/IDCANCEL.
  8. To add code to be executed on other events (messages), use the Events button in the Dialog Editor.
  	 For example, to execute code when button 5 (5 is its id) clicked, in the Dialog Editor click the button, click Events, OK. It inserts 'case 5' line. Add your code below.
  9. When you click Save in the Dialog Editor, it creates code to show the dialog. The code declares dialog variables and calls ShowDialog.
     By default, inserts the code here. You can move it to another place here. You cannot use the code in other macros, unless the dialog procedure is not a sub-function. If need to show this dialog in multiple places in macros, usually it's better to call this function, not ShowDialog directly.
  10. The dialog variables are used only before and after ShowDialog. To get/set control data in dialog procedure, instead use window/control functions, or functions DT_GetControl, DT_SetControl, DT_GetControls, DT_SetControls.
  11. To return a value from the dialog procedure, use DT_Ret. Example: ret DT_Ret(hDlg 1000). Don't use ret 1000, it will not work.
  12. The dialog procedure used in QM is similar to the dialog box procedure that is documented in the MSDN Library on the Internet.
  	 All the messages (events) are documented there.
  	 The Dialog Editor's Events dialog contains some often used messages, but you can use any messages.
  	 Don't use EndDialog and similar functions. This is managed by QM.
