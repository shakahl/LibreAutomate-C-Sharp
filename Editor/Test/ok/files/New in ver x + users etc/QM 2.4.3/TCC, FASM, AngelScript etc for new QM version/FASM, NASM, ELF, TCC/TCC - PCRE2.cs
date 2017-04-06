out

int* f
__Tcc2 x
#if 1
PF
f=x.Compile(":Q:\app\qmcore\pcre\pcre.c" "pcre_version[]pcre_compile[]pcre_exec")
#else
DisassembleObj "Q:\app\qmcore\pcre\Release\pcre.obj" "Q:\Downloads\objconv\pcre.s"
if(!AssembleElf("Q:\Downloads\objconv\pcre.s" "Q:\Downloads\objconv\pcre.o")) ret
 if(!AssembleElfF("Q:\Downloads\objconv\pcre.s" "Q:\Downloads\objconv\pcre.o")) ret
PF
f=x.Compile(":Q:\Downloads\objconv\pcre.o" "pcre_version[]pcre_compile[]pcre_exec[]pcre_malloc[]pcre_free" 0 "user32")
 f=x.Compile(":Q:\Downloads\objconv\pcre.o" "pcre_version[]pcre_compile[]pcre_exec" 0 "user32")
 f=x.Compile(":Q:\Downloads\objconv\pcre.o" "pcre_compile" 0 "user32")
 f=x.Compile(":Q:\Downloads\objconv\pcre.o" "zo" 0 "user32")
 f=x.Compile(":Q:\Downloads\objconv\pcre.o[]Q:\Downloads\objconv\maketables.o" "pcre_version[]pcre_compile[]pcre_exec[]pcre_maketables" 0 "user32" 0)
int* mf=+f[3]; *mf=&q_malloc; mf=+f[4]; *mf=&q_free
#endif
PN;PO

 lpstr version=+call(f[0]); out version; ret

str s.getfile("Q:\app\HTMLHelp\Functions\IDP_ACC.html")
str rx="<.+?class=.+?#34;PUSHBUTTON"

PF
if(findrx("" rx 0 128 _s)<0) end "err"
PN
if(findrx(s _s)<0) end "err"
PN;PO

_hresult=100
PF
lpstr errs; int errc
 byte* p=pcre_compile(rx 0 &errs &errc 0)
byte* p=+call(f[1] rx 0 &errs &errc 0)
 out p
if(!p) out errs; ret
PN
type POINT3 POINT'a[3]
POINT3 r
int e=call(f[2] p 0 s s.len 0 0 &r 6)
if(e<0) out e; ret
PN;PO
out "%i %i" r.a[0].x (r.a[0].y-r.a[0].x)
