 /LSB_1
 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 I use " /LSB_1" at the beginning for easier testing. When you run this function, instead runs DTH_1. To open dialog editor instead, move the second line to the beginning.
 _____________________________________________________________________

 This is dialog definition, craeted by the Dialog Editor.
 In Dialog Editor we can set certain styles for dialog and edit control.
 For example, remove dialog caption, and make edit control text right-to left.

 BEGIN DIALOG
 0 "" 0x90000A48 0x88 0 0 280 210 ""
 3 Edit 0x50002082 0x0 0 0 280 210 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030208 "LSB_Show" "" ""
 _____________________________________________________________________

 THIS CODE IS DIALOG PROCEDURE
 Basic code is created by the Dialog Editor. We add more code.
 It runs while the above code is waiting in ShowDialog.
 It runs many times. It runs whenever the dialog receives a message (WM_INITDIALOG, WM_TIMER, etc).
 Messages are used to notify dialog about various events. Windows messages are documented in MSDN library.
 We add case for each message that we need. Then code below the case is executed whenever the dialog receives the message.

 messages
__Font-- t_f
sel message
	case WM_INITDIALOG ;;we receive this message when dialog is just created, but still invisible
	 get data passed through ShowDialog's param
	LSB_DATA& z=+DT_GetParam(hDlg)
	
	 set font for the edit control
	t_f.Create("LCD" z.size 1)
	t_f.SetDialogFont(hDlg "3")
	t_f.SetDialogFontColor(hDlg z.color "3")
	
	Transparent hDlg 255 0xffffff ;;make dialog transparent, using white as transparent color
	SetTimer hDlg 1 10000 0 ;;close after 10 s
	
	case WM_TIMER ;;we repeatedly receive this message if we called SetTimer; wParam is argument 2 of SetTimer; period is argument 3 of SetTimer
	sel wParam
		case 1
		DT_Cancel hDlg ;;close dialog; let ShowDialog return 0
		case 2
		DT_Ok hDlg ;;close dialog; let ShowDialog return 1; also populates dialog variables
	
	case WM_COMMAND goto messages2 ;;we receive this message when some controls want to notify parent dialog about some events
ret
 messages2
sel wParam
	case EN_CHANGE<<16|3 ;;text changed in edit control
	_s.getwintext(lParam); if(_s.len>3) _s.remove(0 _s.len-3); EditReplaceSel(lParam 0 _s 1) ;;if text length > 3, remove characters from beginning (then text will scroll)
	SetTimer hDlg 1 10000 0 ;;close after 10 s
	
	case EN_SETFOCUS<<16|3 ;;edit control focused
	SendMessage lParam EM_SETSEL -1 -1 ;;remove selection and move caret to the end of text
	
	case IDOK ;;OK button or Enter
	SetTimer hDlg 2 2500 0 ;;close after 3 s
	ret ;;prevent closing dialog now
	
	case IDCANCEL ;;Cancel button or Esc
ret 1
 _____________________________________________________________________

 MORE HELP
 1. This function allows you to execute code while the dialog is open and this way change dialog behavior.
 2. It is implicitly called whenever the dialog receives a message.
 3. It is called multiple times. Values of local variables are not preserved. Instead you can use thread variables or SetProp/GetProp.
 4. To execute code when the dialog is just created but still invisible, place the code under case WM_INITDIALOG. Add tabs at the beginning, as well as under other case's.
 5. To execute code when the dialog is being destroyed, place it under WM_DESTROY.
 6. To execute code when the user clicks OK, place the code under case IDOK. When Cancel - under IDCANCEL.
 7. To prevent closing the dialog on OK/Cancel, return 0 under case IDOK/IDCANCEL.
 8. To insert code that must be executed on other events (messages), use the Events button in the dialog editor.
 	 For example, to execute code when button 3 (3 is its id) clicked, in the dialog editor click the button, click Events, OK.
 	 It inserts 'case 3' line. Add your code below.
 9. To generate code that shows the dialog, click Apply in the dialog editor. Copy the code from the QM output.
 	 You can insert code that shows the dialog in other macro(s) or functions(s), or in this function.
 	 If you insert it in this function, insert it below 'if(hDlg) goto messages'.
 	 To run the dialog instead of edit when you click the Run button, right click the selection bar by the '\Dialog_Editor' line. To edit again, right click there again.
 10. While the dialog is open, dialog variables cannot be used. Instead use window/control functions to get/set values of controls.
 	 To set/get text, use something like _s.getwintext(id(3 hDlg)).
 	 To see if a checkbox checked, use something like if(but(id(3 hDlg))) ... .
 	 To get selected listbox item, use something like _i=LB_SelectedItem(id(3 hDlg)). Use CB_SelectedItem for combobox.
 	 In control messages, lParam usually is control handle, and can be used instead of id(n hDlg).
 11. While the dialog is open, dialog variables can be used if you use an user-defined type for them. You can set it in dialog editor options.
 	 But you must explicitly populate the variables using DT_GetControls. Example:
 	 DIALOGVARTYPE& dv=+DT_GetControls(hDlg)
 	 out dv.e3
 	 if(val(dv.c4Che)) out "checked"
 12. To return a value can be used SetWindowLong or DT_Ret. Example: ret DT_Ret(hDlg 1000).
 13. The dialog procedure used in QM is similar to the dialog box procedure that is documented in the MSDN Library on the Internet.
 	 All the messages (events) are documented there.
 	 The dialog editor's Events dialog contains some mostly used messages but you can use any messages.
 	 Don't use EndDialog and similar functions. QM dialogs are managed by QM extensions.