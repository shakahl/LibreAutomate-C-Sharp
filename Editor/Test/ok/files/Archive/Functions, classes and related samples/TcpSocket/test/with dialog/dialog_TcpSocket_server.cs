\Dialog_Editor

 To test:
   Run dialog_TcpSocket_server and click Start.
   Then run dialog_TcpSocket_client and click Send.

if(getopt(nthreads)>1) ret ;;allow single instance
#compile "__TcpSocket"

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 231 64 "TCP server"
 4 Static 0x54000000 0x0 170 8 22 12 "Port"
 5 Edit 0x54032000 0x200 196 6 32 14 "port"
 6 Button 0x54032000 0x0 6 6 48 14 "Start"
 7 Button 0x5C032000 0x0 58 6 48 14 "Stop"
 14 Static 0x54000000 0x0 6 30 36 12 "Response"
 15 Edit 0x54231044 0x200 46 28 182 30 "res"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "*" "" ""

str controls = "5 15"
str e5por e15res
e5por=5033
e15res="response"
if(!ShowDialog(dd &sub.DlgProc &controls win("" "QM_Editor") 0 0 0 0 0 100)) ret


#sub DlgProc
function# hDlg message wParam lParam
TcpSocket- t_server
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	t_server.Close
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 6 ;;Start
	t_server.ServerStart(GetDlgItemInt(hDlg 5 0 0) &sub.OnClientConnected hDlg 1)
	EnableWindow lParam 0
	EnableWindow id(7 hDlg) 1
	
	case 7 ;;Stop
	t_server.Close
	EnableWindow lParam 0
	EnableWindow id(6 hDlg) 1
ret 1

err+ out _error.description


#sub OnClientConnected
function TcpSocket&client $clientIp hDlg !*reserved

 This function is called in server side, when a client connects.
 This function runs in separate thread for each client connection.

out F"SERVER: client connected: {clientIp}"

str s
client.Receive(s 1000)
out F"SERVER: client request: {s}"

s.getwintext(id(15 hDlg))
client.Send(s)

err+ out _error.description
