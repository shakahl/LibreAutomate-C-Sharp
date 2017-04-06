 /CtoQM
function# str&args str&q $fn str&comm $container

 Converts function arguments.

if(!args.len) ret 1
args.findreplace("&" "*")
args.replacerx(",(?![\w\.])" ",int"); sel(args[0]) case [',','('] args-"int" ;;int not specified
if(findc(args '(')>=0) if(!args.replacerx("\w+\**\([^\)]*?(\w+)?\)\([^\)]*\)" "int fa_$1")) ret

ARRAY(str) a aa amn
int i nt unn('a') ql(q.len) db ctr; str at cb
nt=tok(args a -1 ",")
for i 0 nt
	str& s=a[i]
	s.trim
	if(nt=1 and s="void") break
	 if(s="...") q.fix(ql); break
	if(s="...") q+" ..."; break ;;in QM 2.2.1, dll functions support ... . COM functions cannot have it because __stdcall.
	if(findrx(s "^([A-Z_]\w*)(\**) ?([A-Z_]\w*)?(|\[.*?\]|=.+)$" 0 1 aa)) ret
	
	if(!aa[3].len) ;;no name
		 db=1
		if(fn) GetArgNames(fn amn); fn=0; if(amn.len!=nt and !(amn.len=nt-1 and a[nt-1]="...")) amn.redim
		if(amn.len) aa[3]=amn[i]
		else aa[3].left(+&unn 1); if(unn<'z') unn+1
	else if(aa[4].beg("[")) aa[2]-="*"
	
	ConvType(aa[1] aa[2] at 0 container cb)
	 if(at="!**") at="!*";;maybe inteface. Don't do it
	q.formata(" %s%s" at aa[3])
	if(cb.len and &comm) comm.formata(" ;;%s: %s" aa[3] cb); cb.fix(0)

if(db) out q
ret 1
