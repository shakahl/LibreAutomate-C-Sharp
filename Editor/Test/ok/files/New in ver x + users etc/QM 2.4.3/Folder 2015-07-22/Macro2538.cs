 /exe

__RegisterHotKey hk1.Register(0 1 MOD_CONTROL|MOD_SHIFT VK_F5)

MSG m
rep
	if(GetMessage(&m 0 0 0)<1) break
	sel m.message
		case WM_HOTKEY
		sel m.wParam
			case 1 ;;Ctrl+Shift+F5 pressed
			mac "sub.FreezeMemory"
			
	DispatchMessage &m


#sub FreezeMemory

lock _ "" 0; err ret ;;allow single instance

int hWnd,r,w,Value,NewValue
int BufferSize = sizeof(Value)
byte* BaseAddress
BaseAddress+0x100579C

double X=0.1 ;;every 0.1 s

hWnd=win("Notepad")
if(!hWnd) ret
 if(hWnd!=win) ret ;;if window not active
__ProcessMemory o.Alloc(hWnd 0) ;;just get process handle; error if fails
rep
	 o.ReadOther(&Value BaseAddress BufferSize)
	 out Value
	NewValue=0
	o.WriteOther(BaseAddress &NewValue BufferSize)
	if(WaitForSingleObject(o.hprocess X*1000)!=WAIT_TIMEOUT) break ;;if process ended
	 if(hWnd!=win) break ;;if window deactivated

 BEGIN PROJECT
 main_function  Macro2538
 exe_file  $my qm$\Macro2538.qmm
 flags  6
 guid  {827CBA55-6639-4252-99C8-9AD90F1D12BE}
 END PROJECT
