 /MMT_Main
function NMHDR*nh

MMTVAR- v

sel nh.idFrom
	case 3 ;;toolbar
	sel nh.code
		case NM_RCLICK
		NMMOUSE* mo=+nh
		int i=mo.dwItemSpec
		if(i<0 or i>=v.a.len) ;;not on button
			sel PopupMenu("Close Taskbar[]Monitor...")
				case 1
				DestroyWindow v.hwnd
				case 2
				str s; int hm hm1
				for i 1 32
					hm=MonitorFromIndex(i)
					if(i=1) hm1=hm; else if(hm=hm1) break
					s.formata("%i[]" i)
				i=PopupMenu(s); if(!i) ret
				v.monitor=i
				rset v.monitor "Monitor" "\MultiMonitorTaskbar"
				MMT_SetPos
		else
			int h=v.a[i].hwnd
			sel PopupMenu("Close")
				case 1
				clo h; err ret
				if(win=v.hwnd) act; err
				MMT_Buttons
