out
type LINE_HEADER $file_path line_number file_offset []LINE_HEADER*macro_calling_line LINE_HEADER*macro_line
type FASM_STATE condition output_length []error_code !*output_data []LINE_HEADER*error_line
dll "Q:\Downloads\fasmDLL\FASM.dll"
	#fasm_Assemble $lpSource !*lpMemory cbMemorySize nPassesLimit hDisplayPipe

 include 'Q:\Downloads\fasmw17122\INCLUDE\win32a.inc'
 SetEnvVar "INCLUDE" "Q:\Downloads\fasmw17122\INCLUDE" ;;does not work
SetEnvVar "FASMI" "Q:\Downloads\fasmw17122\INCLUDE" ;;does not work too
 include '%FASMI%\macro\export.inc'
 include '%FASMI%\macro\proc32.inc'
 format PE GUI 4.0 DLL on 'qmmacro.dll'
str asm=
 format PE GUI 4.0 DLL
 entry DllEntryPoint
 include 'Q:\Downloads\fasmw17122\INCLUDE\macro\import32.inc'
 section '.text' code readable executable
 DllEntryPoint:
 ;display 'test'
 mov eax, 1
 ret 12
;
 ;section '.rdata' data readable
 ;section '.data' data readable writeable
 ;section '.rsrc' data readable resource from 'my.res'
 section '.idata' import data readable writeable
 library kernel,'KERNEL32.DLL'
 import kernel, GetLastError,'GetLastError'
 section '.reloc' fixups data readable discardable
 ;these are used only for exe:
 ;stack 0x100000, 0x1000
 ;heap 0x100000, 0x1000

 proc DllEntryPoint hinstDLL,fdwReason,lpvReserved
	 mov	eax,TRUE
	 ret
 endp

#compile "__HeapMemory"
int+ g_heapExec; if(!g_heapExec) g_heapExec=HeapCreate(HEAP_CREATE_ENABLE_EXECUTE 0 0)
HeapMemory hmem; int nmem=20000; byte* mem=hmem.Alloc(nmem g_heapExec)
FASM_STATE& fs=mem

 __Handle hpr hpw; if(!CreatePipe(&hpr &hpw 0 0)) ret

PF
 int hr=fasm_Assemble(asm mem nmem 100 hpw)
int hr=fasm_Assemble(asm mem nmem 100 0)
PN;PO

 if(ReadFile(hpr _s.all(1000 2) 1000 &_i 0)) out _i; out _s.fix(_i)

if hr
	out "ERROR %i %i" hr fs.error_code
	LINE_HEADER& line=fs.error_line
	 _s.getl(asm+line.file_offset 0)
	 out "line %i: %s" line.line_number _s
	ret

out "len=%i outputOffset=%i" fs.output_length fs.output_data-mem

 out call(fs.output_data)

 ret
_s.fromn(fs.output_data fs.output_length)
str sDll="Q:\Downloads\objconv\test_fasm.dll"
_s.setfile(sDll)
int hm=LoadLibrary(sDll)
if(hm) FreeLibrary(hm); else out _s.dllerror
