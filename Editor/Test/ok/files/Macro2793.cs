 /exe 1
MainWindow "Hello World" "QM_HW_Class" &sub.WndProc 200 200 400 200

 Read more in MainWindow help.


#sub WndProc
function# hWnd message wParam lParam
sel message
	case WM_SYSCOMMAND
	out F"WM_SYSCOMMAND {InSendMessage}"
	case WM_CLOSE
	out F"WM_CLOSE {InSendMessage}"
	
	case WM_DESTROY: PostQuitMessage 0
ret DefWindowProc(hWnd message wParam lParam)

 BEGIN PROJECT
 main_function  Macro2793
 exe_file  $my qm$\Macro2793.qmm
 flags  6
 guid  {553F9CF8-E310-49C4-B3E1-E38F166356D1}
 END PROJECT
