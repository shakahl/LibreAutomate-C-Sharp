\Dialog_Editor

 To test:
   Run dialog_TcpSocket_server and click Start.
   Then run dialog_TcpSocket_client and click Send.


#compile "__TcpSocket"

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 231 74 "TCP client"
 4 Static 0x54000000 0x0 170 8 22 12 "Port"
 5 Edit 0x54032000 0x200 196 6 32 14 "port"
 9 Static 0x54000000 0x0 4 6 26 13 "Server"
 10 Edit 0x54030080 0x200 32 4 124 14 "server"
 11 Static 0x54000000 0x0 4 26 26 13 "Data"
 12 Edit 0x54231044 0x200 32 22 196 30 "data"
 13 Button 0x54032000 0x0 4 56 48 14 "Send"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "*" "" ""

str controls = "5 10 12"
str e5por e10ser e12dat
e5por=5033
e10ser="localhost"
e12dat="request"
if(!ShowDialog(dd &sub.DlgProc &controls win("" "QM_Editor") 0 0 0 0 0 300)) ret


#sub DlgProc
function# hDlg message wParam lParam
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 13 ;;Send
	str s
	s.getwintext(id(10 hDlg))
	TcpSocket x.ClientConnect(s GetDlgItemInt(hDlg 5 0 0))
	s.getwintext(id(12 hDlg))
	x.Send(s)
	if(x.Receive(s 1000 1000)=2 and !s.len) s="<timeout>"
	out F"CLIENT: server response: {s}"
ret 1

err+ out _error.description
