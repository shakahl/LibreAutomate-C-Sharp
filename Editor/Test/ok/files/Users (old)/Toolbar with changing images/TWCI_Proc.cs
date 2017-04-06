 /ToolbarWithChangingImages
function# hWnd message wParam lParam

ARRAY(int)+ g_twci_a
int+ g_twci_i

sel message
	case WM_CREATE
	CreateControl(0 "Static" 0 SS_BITMAP 0 0 0 0 hWnd 3)
	g_twci_a.create(2)
	g_twci_i=0
	str s
	g_twci_a[0]=LoadPictureFile("$my qm$\Macro150.bmp" 0)
	 g_twci_a[1]=LoadPictureFile("$my pictures$\avatar_waterdrops.jpg" 0)
	g_twci_a[1]=LoadPictureFile("$my pictures$\avatar_waterdrops.jpg" 0)
	 g_twci_a[0]=LoadImageAnyFormat("$my qm$\Macro150.bmp")
	 g_twci_a[1]=LoadImageAnyFormat("$my pictures$\avatar_waterdrops.jpg")
	SetTimer hWnd 1 500 0
	
	case WM_DESTROY
	int i; for(i 0 g_twci_a.len) DeleteObject(g_twci_a[i])
	g_twci_a.redim
	
	case WM_TIMER
	sel wParam
		case 1
		SendMessage(id(3 hWnd) STM_SETIMAGE IMAGE_BITMAP g_twci_a[g_twci_i])
		g_twci_i^1

 to avoid memory leaks, don't use bitmaps with alpha pixels
