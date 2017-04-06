function$ $decl [~&v1] [~&v2] [~&v3] [~&v4] [~&v5] [~&v6] [~&v7] [~&v8] ;;decl: "[-irsdD ]type names[append]".  d can reuse existing var when default used, D declare always, i ignore v content, r return v1 if no decl, s use self for v1 if it is omitted or null

 Gets unique names for new local variables, and creates declaration string if need.
 Sets this to the declaration string for variables that need to declare, or empty if don't need to declare.
 Returns this.

 decl - type, default names and options.
   Examples: "str s", "int x y cx cy", "int* p", "ARRAY(str) a".
   Can begin with hyphen and one or more options:
     i - ignore initial v content; use default name. If d not used, always declares.
     r - if don't need to declare (would return empty string), return v (and set this to v). Use when you need either "Type var" or "var".
     s - use self if v1 omitted or null.
     d - declare unless v or default name is an existing variable of the type. Does not declare if v empty.
       By default, declares unless v is an existing variable of the type. Declares if v empty.
     D - declare always.
   A default name can be 0 or other number. Then, if v empty, just sets v = the number.
   A default name can be a predefined variable, eg "str _s".
   Can append a string to the declaration string. Ex: "int x[]" (append newline).
 v - (in out) variables containing names, eg dialog variables.
   The function corrects v if need (replaces invalid characters, makes unique).
   If v empty or used option i, sets to the default name.
   v1 can be omitted or null. Then behaves like with -ir (returns "type defname" or "defname"; see also -s).


int i n action ignoreV returnV useDef useSelf needDecl
str _sv1 sAppend

if decl[0]='-'
	rep
		decl+1
		sel decl[0]
			case 32 decl+1; break
			case 'i' ignoreV=1
			case 'r' returnV=1
			case 's' useSelf=1
			case 'd' action=1
			case 'D' action=2
			case else end ERR_BADARG

n=getopt(nargs); if(n=1 or !&v1) n=2; &v1=_sv1; returnV=1; if(useSelf) v1=s; else ignoreV=1

ARRAY(lpstr) as; str _sd=decl
if(tok(_sd as n " " 3)!n) end ERR_BADARG
lpstr _sa=as[n-1]; _sa+__LenId(_sa 1); if(_sa[0]) sAppend=_sa; _sa[0]=0 ;;get string after last var

s.all
__strt** av=&decl
for i 1 n
	lpstr& d=as[i]
	__strt& v=av[i]
	
	useDef=!v.len or ignoreV
	if(useDef) v=d; else v.VN
	
	if(isdigit(v[0])) continue
	if(action=2 or (action=0 and useDef)) needDecl=1; v.VarMakeUnique
	else needDecl=v.VarMakeUnique(as[0])>=0
	
	if needDecl
		if(!s.len) s=as[0]
		s.formata(" %s" v)

if(s.len) s+sAppend
else if(returnV) s=v1
ret s
