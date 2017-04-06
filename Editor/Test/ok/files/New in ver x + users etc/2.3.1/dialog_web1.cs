\Dialog_Editor
 typelib SrchUILib {ECA4E801-17AE-4863-9F5C-AF4047AABEE0} 1.0
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str ax3SHD =
 <html>
 <body>
 <p>
 <a href="#C7">See also Chapter 7.</a>
 </p>
 <h2>Chapter 1</h2>
 <p>This chapter explains ba bla bla</p>
 <h2>Chapter 2</h2>
 <p>This chapter explains ba bla bla</p>
 <h2>Chapter 3</h2>
 <p>This chapter explains ba bla bla</p>
 <h2><a name="C4">Chapter 4</a></h2>
 <p>This chapter explains ba bla bla</p>
 <h2>Chapter 5</h2>
 <p>This chapter explains ba bla bla</p>
 <h2>Chapter 6</h2>
 <p>This chapter explains ba bla bla</p>
 <h2><a name="C7">Chapter 7</a></h2>
 <p>This chapter explains ba bla bla</p>
 <h2>Chapter 8</h2>
 <p>This chapter explains ba bla bla</p>
 <h2>Chapter 9</h2>
 <p>This chapter explains ba bla bla</p>
 <h2>Chapter 10</h2>
 <p>This chapter explains ba bla bla</p>
 </body>
 </html>

if(!ShowDialog("dialog_web1" &dialog_web1 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 600 407 "Form"
 3 ActiveX 0x54000000 0x0 0 0 600 208 "SHDocVw.WebBrowser"
 4 Button 0x54032000 0x0 0 393 48 14 "Location"
 END DIALOG
 DIALOG EDITOR: "" 0x2030106 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SHDocVw.WebBrowser we3
	we3._getcontrol(id(3 hDlg))
	we3.Refresh
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4
	we3._getcontrol(id(3 hDlg))
	out we3.LocationURL
	
	case IDOK
	case IDCANCEL
ret 1