 /opengl
 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

int-- hglrc
int-- hdc
long-- ifff
RECT-- myrect
float-- theta
PIXELFORMATDESCRIPTOR-- PixFrm

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 485 268 "Dialog"
 END DIALOG
 DIALOG EDITOR: "" 0x203000A "" "" ""


ret
int-- useControl=1
 messages
sel message
	case WM_INITDIALOG
		int-- hwndGL
		if useControl
			int+ __gl_atom
			if(!__gl_atom) __gl_atom=RegWinClass("qm_opengl" &DefWindowProc 0 0 0 CS_OWNDC)
			hwndGL=CreateControl(0 "qm_opengl" 0 0 0 0 0 0 hDlg 900)
			SendMessage hDlg WM_SIZE 0 0
		else hwndGL=hDlg
		
		hdc=GetDC(hwndGL)
		PixFrm.nSize=sizeof(PixFrm)
		PixFrm.nVersion=1
		PixFrm.dwFlags=PFD_DRAW_TO_WINDOW|PFD_SUPPORT_OPENGL|PFD_DOUBLEBUFFER
		PixFrm.iPixelType=PFD_TYPE_RGBA
		PixFrm.cColorBits=24
		PixFrm.cDepthBits=16
		PixFrm.iLayerType=PFD_MAIN_PLANE
		ifff=ChoosePixelFormat(hdc,&PixFrm)
		SetPixelFormat(hdc,ifff,&PixFrm)
		hglrc=wglCreateContext(hdc)
		wglMakeCurrent(hdc hglrc)
		 PostMessage hDlg WM_SETCURSOR 0 0
		SetTimer hDlg 1 1 0
	case WM_DESTROY
		wglMakeCurrent(0,0)
		wglDeleteContext(hglrc)
		ReleaseDC( hwndGL, hdc )
		 int w1=win("" "NVOpenGLMessageMonitor" GetCurrentProcessId)
		 zw w1
		 out GetWindowThreadProcessId(w1 &_i)
		 clo w1
		 PostThreadMessage GetWindowThreadProcessId(w1 0) WM_QUIT 0 0
		 SendMessage w1 WM_DESTROY 0 0
		 PostThreadMessage GetWindowThreadProcessId(w1 0) WM_QUIT 0 0
		 __Handle ht=OpenThread(THREAD_TERMINATE 0 GetWindowThreadProcessId(w1 0))
		 out ht
		 TerminateThread ht 0
		
	case WM_COMMAND goto messages2
	case WM_SIZE
	if(useControl) RECT rc; GetClientRect hDlg &rc; MoveWindow hwndGL 0 0 rc.right rc.bottom 1
	 case WM_SETCURSOR
	case WM_TIMER
	sel wParam
		case 1
		glClearColor(0,0,0,0)
		glClear(GL_COLOR_BUFFER_BIT)
		glPushMatrix
		glRotatef(theta, 0.0, 0.0, 1.0);
		glBegin(GL_TRIANGLES)
		glColor3f( 1.0, 0.0, 0.0)
		glVertex2f( 0.0, 1.0)
		glColor3f( 0.0, 1.0, 0.0)
		glVertex2f( 0.87, -0.5 )
		glColor3f( 0.0, 0.0, 1.0 )
		glVertex2f( -0.87, -0.5 )
		glEnd();
		glPopMatrix();
		SwapBuffers(hdc);
		theta=theta+1.0
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
