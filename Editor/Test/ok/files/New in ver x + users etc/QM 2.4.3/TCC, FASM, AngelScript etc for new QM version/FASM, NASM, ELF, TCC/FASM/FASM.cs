out
type LINE_HEADER $file_path line_number file_offset []LINE_HEADER*macro_calling_line LINE_HEADER*macro_line
type FASM_STATE condition output_length []error_code !*output_data []LINE_HEADER*error_line
dll "Q:\Downloads\fasmDLL\FASM.dll"
	#fasm_Assemble $lpSource !*lpMemory cbMemorySize nPassesLimit hDisplayPipe

#opt nowarnings 1
str sCode; lpstr asm=""
if(!Scripting_GetCode(asm sCode 0 1)) ret

#compile "__HeapMemory"
int+ g_heapExec; if(!g_heapExec) g_heapExec=HeapCreate(HEAP_CREATE_ENABLE_EXECUTE 0 0)
HeapMemory hmem; int nmem=20000; byte* mem=hmem.Alloc(nmem g_heapExec)
FASM_STATE& fs=mem

PF
int hr=fasm_Assemble(asm mem nmem 100 0)
 PN;PO
if(hr) ret FasmError(hr fs asm)

 out "len=%i outputOffset=%i" fs.output_length fs.output_data-mem
 outb fs.output_data fs.output_length

PF
_i=call(fs.output_data 0)
PN;PO
out _i

#ret
use32
func1:
push ebp
mov ebp,esp
push esi

;for(i=10000;
mov         esi,100000 
		;i>0;
jmp         g1
		;i--)
g3:
dec         esi 
g1:
cmp         esi,0 
jle         g2
		;x+=testR3(1);
push        1    
call        func2
pop         ecx  
add         [ebp+8], eax
jmp  g3
g2:
;return x;
mov         eax, [ebp+8] 

pop esi
leave
ret

func2:
push ebp
mov ebp,esp

mov         eax, [ebp+8] 
inc         eax  

leave
ret

;s1 db 'test',0

