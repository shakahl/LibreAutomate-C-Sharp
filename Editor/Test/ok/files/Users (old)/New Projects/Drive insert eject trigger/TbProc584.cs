 /
function# hWnd message wParam lParam

def WM_DEVICECHANGE  0x219
def DBT_DEVICEARRIVAL 0x8000
def DBT_DEVICEREMOVECOMPLETE  0x8004
def DBT_DEVTYP_VOLUME 0x2

type DEV_BROADCAST_VOLUME dbcv_size dbcv_devicetype dbcv_reserved dbcv_unitmask @dbcv_flags

sel message
	case WM_DEVICECHANGE
	sel(wParam) case [DBT_DEVICEARRIVAL,DBT_DEVICEREMOVECOMPLETE] case else ret
	DEV_BROADCAST_VOLUME* v=+lParam
	if(v.dbcv_devicetype!=DBT_DEVTYP_VOLUME) ret
	int i j=v.dbcv_unitmask
	for(i 0 26) if(j&1) break; else j>>1
	i+'A'
	sel wParam
		case DBT_DEVICEARRIVAL
		out "inserted %c" i
		 if(i='E') mac "OnInserted"
		
		case DBT_DEVICEREMOVECOMPLETE
		out "removed %c" i
		 if(i='E') mac "OnRemoved"
		