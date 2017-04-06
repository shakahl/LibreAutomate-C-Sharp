 /CtoQM
function# str&name str&r

 Simplifies constant. Returns 0 to include it in the file, 1 to exclude (eg if empty value).
 Expands C macros in it (simple and function-style), calculates expressions, joins strings, removes typecasts, etc.
 Adds most excluded constants to m_mo, which later will be saved to filename_other.txt.
 Also calls IsDefAlias, which corrects names of functions etc, and excludes those constants.
 Called for each item in m_mc (#define, enum and const). Not called for m_mg (GUID).
 Called from Save (after extracting types, dlls, etc).

int db x y isx ex
str ss comm

 g0
if(!r.len)
	ss.from("def " name)
	AddToMap(m_mo name ss "" 1)
	ret 1
if(r[0]='(')
	UnPar(r) ;;(...)
if(isdigit(r[0]) and findrx(r "^\w+$")=0)
	if(r.endi("L") or r.endi("U")) r.fix(r.len-1)
	ret
if(findc(r 34)>=0)
	rep
		if(!r.replacerx("(L?''.*?)'' *L?''(.*?'')" "$1$2")) break ;;join "string1" "string2"
		 out r
	if(r[0]=34 or (r[0]='L' and r[1]=34))
		 out r
		ret
if(!ex)
	str s0=r
	m_aems[0]=&name
	ex=ExpandMacros(r 1)
	if(ex)
		 out "%s[]%s[]%s[]" name s0 r
		goto g0

 if(findc(r 34)>=0) out r
foreach(ss "const[]unsigned[]extern ''C''[]__stdcall[]__declspec(selectany)[]__declspec(dllimport)[]struct[]volatile")
	if(r.findreplace(ss "" 2)) r.trim
if(!r.len) goto g0

if(IsDefAlias(name r))
	 out "%s %s" name r
	ret 1

 some of following would also modify strings, but there are only few strings, and none of them actually is modified
r.findreplace("' '" "32") ;;escape from replacing to ''
r.replacerx(m_rx.sp1); r.replacerx(m_rx.sp2) ;;remove spaces

if(find(r "->")>=0 and find(name "SIZE")>=0) ;;get through QM
	 out "z('' %s #'', %s);" name name
	ret

if(findc(r '(')>=0) ;;remove (CASTTYPE)
	if(UnCast(r)) comm.format(" ;;%s" r); UnCast(r 1);; db=1

r.replacerx(m_rx.numberL "$1") ;;numberL to number
rep() if(!r.replacerx(m_rx.numberue1 "$1")) break ;;unenclose numbers
rep() if(!r.replacerx(m_rx.numberue2 "$1")) break ;;unenclose numbers
rep() if(!r.replacerx("\((\([^\(\)]+\))\)" "$1")) break ;;unenclose ((...))
rep() if(!r.replacerx("(?<![\w\)])\((\w+)\)(?=[<\|+]|$)" "$1")) break;; else db=1 ;;unenclose (word) where it is safe

if(r.beg("{") and !findrx(r "^\{([^,]+?,){10}0x.+?\}$"))
	ss.format("GUID %s=%s;" name r)
	 out ss
	if(_other(ss)=0) ret 1
	 db=1

if(findrx(r "[^\w\.\[\]\{\}\,\; ]")>=0) ;;calculate expression
	isx=1
	x=m_expr2.EvalC(r); err isx=0;; db=1
	if(isx) r.format("0x%08X" x)

if(findc(r '(')>=0)
	 db=1
	if(r.replacerx("\b(?:UINT|wchar_t)\((-?\w+)\)" "$1U")) db=0
	if(r.replacerx("\b__(uuidof\(\w+\))" "$1")) db=0
	if(findrx(r "sizeof\(|__declspec\(")>=0) db=0
	 db=1

 if(findcs(r " {};:#?")>=0) db=1
 if(findrx(r "[^\w\(\)\.\[\]\{\}\&\:,; \\]")>=0) db=1 ;;expression
 if(r.len>40) db=1
 if(findrx(r "(?<=\w)\(\w+\)")>=0) db=1 ;;all function-style macros must be expanded
 if(findrx(r "(?<!\w)\(\w+\)")>=0) db=1

if(comm.len) r.formata("[]%s" comm)

 db=1
if(db)
	out "%s %s" name r
	 out comm

 NOTES
 Removes all type casts. In some cases it can give wrong value. Eg, (BYTE)-1 actually is 255. Where similar problems are possible, adds source as comments.
