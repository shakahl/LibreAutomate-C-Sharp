 /ToolbarWithTooltips
function# hWnd message wParam lParam

#compile __CToolTip

type TWTVARS CToolTip'tt CToolTip'tt2
TWTVARS* v=+GetProp(hWnd, "v")

sel message
	case WM_CREATE
	SetProp(hWnd "v" v._new)
	int x=24
	CreateControl(0 "Button" "a" 0 x 0 30 20 hWnd 3)
	CreateControl(WS_EX_CLIENTEDGE "Edit" "" ES_AUTOHSCROLL x+34 0 30 20 hWnd 4)
	CreateControl(0 "Button" "c" 0 x+72 0 30 20 hWnd 5)
	v.tt.Create(hWnd TTS_ALWAYSTIP)
	v.tt.AddTools(hWnd "3 button[]4 edit")
	v.tt2.Create(hWnd TTS_ALWAYSTIP|TTS_BALLOON)
	v.tt2.AddTool(hWnd 5 "button2")
	
	case WM_DESTROY
	v.tt.Destroy
	v.tt2.Destroy
	v._delete; RemoveProp(hWnd "v")
	
	case WM_SETCURSOR
	v.tt.OnWmSetcursor(wParam lParam)
	v.tt2.OnWmSetcursor(wParam lParam)
