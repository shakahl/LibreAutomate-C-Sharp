 formats "string" as {'s', 't', ...};

str s.getclip ss="{[]"
int i
for i 0 s.len
	int c=s[i]
	sel c
		case 13 ss+"13"
		case 10 ss+"10,[]"; continue
		case 9 ss+"'\t'"
		case 34 ss+"'\'''"
		case ''' ss+"'\''"
		case else ss.formata("'%c'" c)
	ss+", "

ss+"0[]};";
out ss
ss.setclip
