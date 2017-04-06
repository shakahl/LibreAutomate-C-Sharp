typelib Excel {00020813-0000-0000-C000-000000000046} 1.2 0 1
Excel.Application a._getactive; err act; act; a._getactive
IDispatch ws=a.ActiveSheet

int v=ws.CheckBox1.Value
sel v
	case -1 mes "True"
	case 0 mes "False"
ws.CheckBox1.Value=0 ;;can be 0 or 1 or 2
