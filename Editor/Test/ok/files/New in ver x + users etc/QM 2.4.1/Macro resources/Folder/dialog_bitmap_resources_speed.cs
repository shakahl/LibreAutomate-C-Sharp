\Dialog_Editor
/exe
function# hDlg message wParam lParam
if(hDlg) goto messages

out
str controls = "3 7 13 14 15 16"
str sb3 sb7 sb13 sb14 sb15 sb16
 sb3=":1 Q:\ico and bmp\bmp\AavmGuih_132.bmp"
sb13="resource:<>test.bmp"
sb7="resource:<test images in annotations>image:h1CCB37B1"
 sb14=":5 $my qm$\bitmap30x30.png"
 sb15="resource:<Macro2240>bitmap30x30.png"
if(!ShowDialog("" &dialog_bitmap_resources_speed &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 263 180 "Dialog"
 3 Static 0x5400000E 0x0 0 38 16 16 ""
 7 Static 0x5400000E 0x0 54 70 16 16 ""
 13 Static 0x5400000E 0x0 144 70 16 16 ""
 14 Static 0x5400000E 0x0 238 70 16 16 ""
 15 Static 0x5400000E 0x0 54 108 16 16 ""
 16 Static 0x5400000E 0x0 54 156 16 16 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040100 "*" "" "" ""
 4 QM_DlgInfo 0x54000000 0x20000 116 0 96 48 "<><image ''resource:<test images in annotations>image:hE0801C30''>test</image>"

ret
 messages
sel message
	case WM_INITDIALOG
	__GdiHandle a1 a2
	a1=LoadPictureFile("resource:<>test.bmp" 0|128)
	 a2=LoadPictureFile("resource:<test images in annotations>image:h1CCB37B1" 2|128)
	int hm; lpstr name
	 hm=GetModuleHandle(0); name=+168
	 int+ g_hm; if(!g_hm) g_hm=LoadLibraryEx("Q:\My QM\qmm\1273bb0c-Macro2197.qmm" 0 LOAD_LIBRARY_AS_DATAFILE)
	 int+ g_hm; if(!g_hm) g_hm=LoadLibraryEx("Q:\My QM\qmm\test.exe" 0 LOAD_LIBRARY_AS_DATAFILE)
	int+ g_hm; if(!g_hm) g_hm=LoadLibraryEx("Q:\My QM\qmm\test.exe" 0 LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE)
	hm=g_hm; name="test.bmp"
	
#if 0
	hm~3
	str sd
	sd.flags=1; sd.all(10000000)
	 _i=GetFileOrFolderSize("Q:\My QM\qmm\1273bb0c-Macro2197.qmm"); byte* t=+hm; _s.fromn(t _i); hm=_s+1
	sd.getfile("$my qm$\test.bmp" 14)
	_i=GetFileOrFolderSize("Q:\My QM\qmm\1273bb0c-Macro2197.qmm")
	byte* t=+hm
	_s.fromn(t 0x374 sd sd.len)
	hm=_s+1
	
	 byte* t=+hm
	 lpstr null
	 _s.fromn(t 0x200   null 0x1000-0x200   t+0x200 0x174   sd sd.len)
	
	 str y.all(0x1000-0x200 2 32)
	 _s.insert(y 0x200)
	 hm=_s
	
	 out _s.len
	
	 lpstr k=+hm
	 memcpy &_i k+0x374 4; outx _i
	 int i; int* p=+k
	 for i 0 0x2000 4
		 out "0x%08X 0x%08X" i *p
		 if(*p=0x1174) out "found"; break
		 p+4
	int* p=_s+0x300
	 p[0]=sd-_s+0x1000-0x200
	 int* p=_s+0x1100
	 p[0]=sd-_s
	p[1]=sd.len
	outx p[0]
#endif
	
	PF
	 a2=LoadImage(hm name 0 0 0 LR_CREATEDIBSECTION)
	a2=LoadImage(hm name 0 0 0 0)
	if(!a2) out _s.dllerror
	PN;PO
	
	BITMAP b
	if(GetObject(a1 sizeof(BITMAP) &b)) out "%i %i" b.bmHeight b.bmBits
	if(GetObject(a2 sizeof(BITMAP) &b))out "%i %i" b.bmHeight b.bmBits
	
	if(!a2) ret
	__MemBmp- t_bm.Attach(a2); a2.handle=0
	
	case WM_PAINT
	PAINTSTRUCT ps; BeginPaint hDlg &ps
	BitBlt ps.hDC 0 0 100 50 t_bm.dc 0 0 SRCCOPY
	EndPaint hDlg &ps
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
