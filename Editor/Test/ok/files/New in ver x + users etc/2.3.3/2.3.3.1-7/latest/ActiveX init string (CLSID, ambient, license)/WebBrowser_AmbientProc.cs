function# dispid `&v param

 out dispid
sel dispid
	case -5512 ;;DISPID_AMBIENT_DLCONTROL
	v=param
	ret 1
