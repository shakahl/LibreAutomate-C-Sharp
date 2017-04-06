/exe
function# [hWnd] [message] [wParam] [lParam]
if(hWnd) goto messages

#compile "__opengl"
int-- hglrc
int-- hdc
long-- ifff
RECT-- myrect
float-- theta
PIXELFORMATDESCRIPTOR-- PixFrm

int+ __gl_atom
if(!__gl_atom) __gl_atom=RegWinClass("qm_opengl" &wnd_opengl 0 0 0 CS_OWNDC)

hWnd=CreateWindowEx(0 "qm_opengl" 0 WS_POPUPWINDOW|WS_CAPTION|WS_VISIBLE 0 0 1024 768 0 0 _hinst 0)

hdc=GetDC(hWnd)
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
 PostMessage hWnd WM_SETCURSOR 0 0
SetTimer hWnd 1 10 0

MessageLoop

 MSG msg
 rep
	 if(PeekMessage(&msg 0 0 0 PM_REMOVE))
		 if(msg.message=WM_QUIT) break
		 TranslateMessage &msg; DispatchMessage &msg
	 else
		 glClearColor(0,0,0,0)
		 glClear(GL_COLOR_BUFFER_BIT)
		 glPushMatrix
		 glRotatef(theta, 1.0, 1.0, 1.0); ;;changed
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
		 0.001

 opt waitmsg 1
 rep
	 0.001
	 glClearColor(0,0,0,0)
	 glClear(GL_COLOR_BUFFER_BIT)
	 glPushMatrix
	 glRotatef(theta, 1.0, 1.0, 1.0); ;;changed
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
 messages
sel message
	case WM_CREATE
	case WM_DESTROY
		wglMakeCurrent(0,0)
		wglDeleteContext(hglrc)
		ReleaseDC( hWnd, hdc )
		PostQuitMessage 0
	
	 case WM_SETCURSOR
	case WM_TIMER
	sel wParam
		case 1
		 Q &q
		glClearColor(0,0,0,0)
		glClear(GL_COLOR_BUFFER_BIT)
		glPushMatrix
		glRotatef(theta, 1.0, 1.0, 1.0); ;;changed
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
		 Q &qq
		 outq
		 
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

ret DefWindowProc(hWnd message wParam lParam)

 BEGIN PROJECT
 main_function  wnd_opengl
 exe_file  $my qm$\wnd_opengl.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {9526248B-E121-44B1-99D7-54FE81907C71}
 END PROJECT
