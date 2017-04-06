function str&s [flags] ;;flags: 1 join lines, 2 remove commas, 4 remove semicolons

s.replacerx("(?<!\r)\n" "[]") ;;\n to \r\n
s.replacerx("(?s)/\*.*?\*/" "" 8) ;;remove /*comments*/
s.replacerx("^(.*)//.*$" "$1[]" 8) ;;remove //comments
s.replacerx("\\[]" " " 8) ;;remove \ line breaks
s.replacerx("(?m)[ \t]+$" "")
s.replacerx("(?m)^[ \t]+$" ""); ;;remove empty lines

if(flags&4) s.findreplace(";" " ")
sel(flags&3)
	case 0 s.replacerx("[\t ]{2,}" " ")
	case 1 s.replacerx("[\s]+" " ")
	case 2 s.replacerx("[, \t]+" " ")
	case 3 s.replacerx("[\s,]+" " ")
	
s.replacerx("\( +" "(")
s.replacerx(" +\)" ")")
