function#* $code $func $libList $qmFuncList *qmFuncAddrArr $sysIncludePaths $includePaths $libPaths $preprocDef

ref __tcc

ARRAY(str) afn=func
int size i fa sizeofAFA=afn.len*4
int* m cafa
str s
byte* t

 init

type ___TCCAUTODELETE !*t !*m
___TCCAUTODELETE tad

int codeFormat=m_flags&3

t=tcc_new; if(t) tad.t=t; else end ERR_MEMORY
tcc_set_codepage _unicode
tcc_set_lib_path(t 0)
if(m_flags&16) tcc_disable_warnings(t)
tcc_set_error_func(t &this &sub.OnError)

foreach(s sysIncludePaths) tcc_add_sysinclude_path(t s.expandpath)
foreach(s includePaths) tcc_add_include_path(t s.expandpath)

 define more preprocessor symbols

str prDef=
 NULL ((void *)0)
 __int64 long long
 __stdcall __attribute__((stdcall))
 __cdecl __attribute__((cdecl))
 __declspec(x) __attribute__((x))
;
prDef.addline(preprocDef)
if(codeFormat=0) prDef.addline("printf q_printf")

foreach s prDef
	lpstr dname dval
	i=tok(s &dname 2 " [9]" 3)
	if(i=1) dval=0
	if(i) tcc_define_symbol(t dname dval)

 compile

tcc_set_output_type(t codeFormat)
foreach(s libPaths) tcc_add_library_path(t s.expandpath) ;;must be after tcc-set-output-type, or the default path will be at the end

if(code[0]=':' and !m_iid) ;;files
	foreach s code+1
		if(tcc_add_file(t s.expandpath)=-1) end F"failed to add file {s}"
else
	if(tcc_compile_string(t code)=-1) end "failed to compile"

 link

foreach s libList
	s.expandpath
	if(findcs(s ".\/")>=0) if(tcc_add_file(t s)=-1) end F"failed to add file {s}"
	else if(tcc_add_library(t s)=-1) end F"failed to add library {s}"

if codeFormat
	if(tcc_output_file(t s.expandpath(func))) end F"failed to create file {s}"
	ret

tcc_add_symbol(t "q_printf" &q_printf)
foreach(s qmFuncList) tcc_add_symbol(t s +*qmFuncAddrArr); qmFuncAddrArr+4

size=tcc_relocate(t 0); if(size=-1) end "failed to link"
m=tcc_alloc_code_memory(size+sizeofAFA); tad.m=m
if(tcc_relocate(t m+sizeofAFA)=-1) end "failed to link"

 get func names, etc

if(m_flags&8) cafa=&this+sizeof(this)
for i 0 afn.len
	fa=tcc_get_symbol(t afn[i])
	if(!fa) end F"function not found: {afn[i]}"
	m[i]=fa
	if(cafa) cafa[i]=fa

m_code=m; tad.m=0
this.f=*m
ret m

err+
	 original TCC code sometimes would call exit(). I replaced it with RaiseException(0x5555).
	if(_error.source=4 and _error.description.beg("Exception 0x5555.")) end ERR_FAILED
	end _error


#sub OnError
function[c] __Tcc&tcc $errstr
tcc.__OnError(errstr)
