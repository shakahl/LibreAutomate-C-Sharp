function~ $name $value [flags] ;;flags: 1 no {}, 0x100 escape "</"

 Formats and returns {"name":"value-json-escaped-string"} or "name":"value-json-escaped-string".
 No errors.


str v=value
v.findreplace("\" "\\")
v.findreplace("''" "\''")
v.findreplace("[]" "\r\n")
v.findreplace("[10]" "\n")
v.findreplace("[13]" "\r")
v.findreplace("[9]" "\t")
v.findreplace("[8]" "\b")
v.findreplace("[12]" "\f")
if(flags&0x100) v.findreplace("</" "<\/")

lpstr f
sel flags&1
	case 0 f="{''%s'':''%s''}"
	case 1 f="''%s'':''%s''"
ret _s.format(f name v)
