out
DisassembleObj "Q:\app\qmcore\pcre\Release\pcre.obj" "Q:\Downloads\objconv\pcre.s"
 if(!AssembleElf("Q:\Downloads\objconv\pcre.s" "Q:\Downloads\objconv\pcre.o")) ret
if(!AssembleElfF("Q:\Downloads\objconv\pcre.s" "Q:\Downloads\objconv\pcre.o")) ret

UnloadDll "Q:\Downloads\objconv\pcre.dll"
PF
 __Tcc2 x.Compile("" "Q:\Downloads\objconv\pcre.dll" 2 "Q:\app\qmcore\pcre\pcre.c")
__Tcc2 x.Compile("" "Q:\Downloads\objconv\pcre.dll" 2 "Q:\Downloads\objconv\pcre.o")
 __Tcc2 x.Compile("" "Q:\Downloads\objconv\pcre.dll" 2 "Q:\Downloads\objconv\pcre.o[]Q:\Downloads\objconv\maketables.o" 0)
PN;PO

dll- "Q:\Downloads\objconv\pcre.dll"
	$p_version
	!*p_compile $pattern #options $*errptr #*erroffset !*tableptr
	#p_exec !*code !*extra $subject #length #startoffset #options #*ovector #ovecsize
	p_init p_malloc p_free
p_init &q_malloc &q_free
_i=&p_compile+&p_exec ;;to load dll/func, to measure speed
 out p_version

str s.getfile("Q:\app\HTMLHelp\Functions\IDP_ACC.html")
str rx="<.+?class=.+?#34;PUSHBUTTON"
 str rx="[[:digit:]]+"

PF
if(findrx("" rx 0 128 _s)<0) end "err"
PN
if(findrx(s _s)<0) end "err"
PN;PO

_hresult=100
PF
lpstr errs; int errc
byte* p=p_compile(rx 0 &errs &errc 0)
if(!p) out errs; ret
PN
type POINT3 POINT'a[3]
POINT3 r
int e=p_exec(p 0 s s.len 0 0 +&r 6)
if(e<0) out e; ret
PN;PO
out "%i %i" r.a[0].x (r.a[0].y-r.a[0].x)


#ret
#define EXPORT __declspec(dllexport)

EXPORT char* p_version() { return (char*)pcre_version(); }
EXPORT void* p_compile(char* pattern, int options, char** errptr, int* erroffset, void* tableptr)
{ return (void*)pcre_compile(pattern, options, errptr, erroffset, tableptr); }
EXPORT int p_exec(void* code, void* extra, char* subject, int length, int startoffset, int options, int*ovector, int ovecsize)
{ return pcre_exec(code, extra, subject, length, startoffset, options, ovector, ovecsize); }

extern void*(*pcre_malloc)(unsigned long);
extern void(*pcre_free)(void*);
EXPORT int* p_init(int p_malloc, int p_free)
{
pcre_malloc=(void*(*)(unsigned long))p_malloc;
pcre_free=(void(*)(void*))p_free;
}

