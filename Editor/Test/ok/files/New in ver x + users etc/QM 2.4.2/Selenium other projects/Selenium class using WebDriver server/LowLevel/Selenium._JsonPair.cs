function~ $name $value [flags] ;;flags: 1 no {}

 Formats and returns {"name":value} or "name":value.
 No errors.


lpstr f
sel flags&1
	case 0 f="{''%s'':%s}"
	case 1 f="''%s'':%s"
ret _s.format(f name value)
