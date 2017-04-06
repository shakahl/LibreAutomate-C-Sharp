 Exception etc. FASM creates invalid dlls.

dll "Q:\Downloads\objconv\pcre.dll"
	!*pcre_compile $pattern #options $*errptr #*erroffset !*tableptr
	#pcre_exec !*code !*extra $subject #length #startoffset #options #*ovector #ovecsize

str s.getfile("Q:\app\HTMLHelp\Functions\IDP_ACC.html")
str rx="<.+?class=.+?#34;PUSHBUTTON"

PF
if(findrx("" rx 0 128 _s)<0) end "err"
PN
if(findrx(s _s)<0) end "err"
PN;PO

 outx GetModuleHandle("Q:\Downloads\objconv\pcre.dll")
 outx &pcre_compile
PF
lpstr errs; int errc
byte* p=pcre_compile(rx 0 &errs &errc 0)
if(!p) out errs; ret
PN
type POINT3 POINT'a[3]
POINT3 r
int e=pcre_exec(p 0 s s.len 0 0 +&r 6)
if(e<0) out e; ret
PN;PO
out "%i %i" r.a[0].x (r.a[0].y-r.a[0].x)

 or:
 str sDll="Q:\Downloads\fasmw17122\EXAMPLES\DLL\ERRORMSG.dll" ;;this is invalid too
 _s.setfile(sDll)
 int hm=LoadLibrary(sDll)
 if(hm) FreeLibrary(hm); else out _s.dllerror
