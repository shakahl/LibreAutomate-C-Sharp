 /CtoQM
function# $shf str&s

 Called as first step of preprocessing file.
 Joins lines broken using \, removes comments, etc.

str ss; lpstr seq
int i j k nr esc

if(findrx(s "\?\?[#\\\[\]\^\{\}\|~]")>=0) out "trigraph in %s" shf
s.replacerx("(?<![13])[10]" "[]") ;;\n to \r\n
s.findreplace("\[]" " ") ;;join lines broken using \

 comments
ss.all(s.len); ss.flags=1
rep
	i=findcs(s "/'''" i); if(i<0) i=s.len; break
	 continue
	sel s[i]
		case '/'
		sel s[i+1]
			case '/' j=findc(s 13 i)
			case '*' j=find(s "*/" i)+2
			case else i+1; continue
		nr+1
		ss.geta(s k i-k)
		if(j<0) i=k; break
		i=j; k=j
		case [34,39] ;;skip string because // and /* in strings are not interpreted as comments
		seq=SkipEnclosed(s+i 0); if(!seq) end "unmatched quote in %s" 1 shf
		i=seq-s
if(nr)
	ss.geta(s k i-k)
	 out "%i %i %s" s.len ss.len shf
	RECT r; memcpy(&r &s 12); memcpy(&s &ss 12); memcpy(&ss &r 12) ;;swap
 out nr

s.findreplace("[9]" " ") ;;tabs to spaces
s.replacerx("[]\s+" "[]") ;;remove empty lines and indentation
s.replacerx(" +[]" "[]") ;;remove spaces from end of lines
s.replacerx("^# *pragma +(pack\b.+\))$" "$$$1;" 8) ;;replace #pragma pack(...) to $pack(...);
 s.replacerx("  +" " ") ;;replace multiple spaces. It makes more clear and less warnings later. However damages some strings.

 out s
