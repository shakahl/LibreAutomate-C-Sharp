 /
function# $macro str&code

 Gets macro text.
 If finds #ret line, erases all lines before.
 Else erases first line if it is like /macro_options.
 Returns QM item id.

 macro - macro name or +id. Error if empty.

#opt nowarnings 1
opt noerrorshere 1

if(!macro or (macro>=0x10000 and !macro[0])) end ERR_BADARG
int iid=qmitem(macro 1); if(!iid) end ERR_MACRO
code.getmacro(iid)

int n i=findrx(code "^#ret\b.*[]" 0 8 _i)
if i>=0 ;;erase lines before #ret
	i+_i-2
	code[i]=0; n=numlines(code)-1; code[i]=13
	code.replace(_s.all(n 2 10) 0 i) ;;faster than replacerx
else if 0=findrx(code "^[ ;]*[/\\](?![/\*]).*[]" 0 0 _i)
	code.remove(0 _i-2) ;;erase first line

ret iid
