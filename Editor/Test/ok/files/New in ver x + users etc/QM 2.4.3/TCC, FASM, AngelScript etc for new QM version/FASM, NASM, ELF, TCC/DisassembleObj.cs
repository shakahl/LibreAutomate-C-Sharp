 /Convert obj to elf
function! $objFile $asmFile

 Disassembles objFile (.obj file) to NASM/YASM dialect. Creates asmFile (.s file).


int ec=RunConsole2(F"$qm$\objconv.exe -fnasm -xp ''{objFile}'' ''{asmFile}''")
if(ec) ret

str s.getfile(asmFile)
 out s
sub.Sections s
s.replacerx("(?m)^(global \S+:) +(\w+)" "$1$2") ;;global Func: function -> global Func:function
sub.Unmangle s
sub.FixRedefinedLabels s
 sub.ChangeApiCalls s
sub.SEH s

s.setmacro("asm")
s.setfile(asmFile)

ret 1

 note: ndisasm cannot be used. It supports only executable files, not object files. Also, it disassembles data too, unless you specify code offset.


#sub Sections
function str&s
 Fixes section attributes, removes unused sections.

s.replacerx("(?ms)^SECTION .drectve.+?(?=^SECTION)" "" 4) ;;remove the ".drective" section
s.replacerx("(?m)^(SECTION \.text\b)(?:\S+)? +align=1 execute" "$1") ;;use default
s.replacerx("(?m)^(SECTION \.rdata) +align=\d+ noexecute" "$1 progbits alloc noexec nowrite align=4")
s.replacerx("(?m)^(SECTION \.\S+) +align=\d+ noexecute" "$1") ;;use default (.data, .bss, exception tables)
s.replacerx("(?m)^(SECTION \.)(xdata\$x|sxdata)" "$1rdata") ;;use default (.data, .bss, exception tables)
 out s

#sub FixRedefinedLabels
function str&s
 Makes label names unique.
 Because different functions have labels with same name. Assembler sees them as global, and gives 'label redefined' error.

int iSect i j
REPLACERX rr
 rr.frepl=&sub.Callback_str_replacerx

rep ;;for each SECTION .text
	rr.ifrom=find(s "[10]SECTION .text " rr.ito)+1; if(!rr.ifrom) break
	rr.ito=find(s "[10]SECTION " rr.ifrom+10); if(rr.ito<0) rr.ito=s.len
	iSect+1
	 out "%i %i" rr.ifrom rr.ito
	rr.repl=F"$0_{iSect}_"
	lpstr rx="[\[\s]\$L[NL]\d+\b"
	if(!s.replacerx(rx rr)) continue
	 also rename labels in SEH data section of this function
	FINDRX fr.ifrom=rr.ifrom; fr.ito=rr.ito
	i=findrx(s "\b_sehtable\$(\w+)" fr 0 _s); if(i<0) continue
	i=findrx(s F"(?ms)^\Q{_s}\E:.+?[][]" i 0 j); if(i<0) continue
	rr.ifrom=i; rr.ito=i+j
	s.replacerx(rx rr)
 out iSect


#sub Unmangle
function str&s
 Replaces all _Func and __imp__Func@N to Func.

s.replacerx("(?<=[\[\s])_(\w+)" "$1")
s.replacerx("(?<=[\[\s])_imp__(\w+)(@\d+)?" "$1")
s.replacerx("(?<=[\[\s])\?(\w+)(@[^\s:]+)?" "$1")
 out s

#sub ChangeApiCalls ;;obsolete, don't use
function str&s
 Replaces all 'call near [ApiFunc]' to call ApiFunc.

s.replacerx("(?m)^( *call +)near *\[(\w+)\]" "$1$2")


#sub SEH
function str&s
 Adds _SEH_prolog and _SEH_epilog.

s.replacerx("(?m)^extern (_SEH_(pro|epi)log)\b")
ret

 out s.replacerx("(?m)^extern (_SEH_(pro|epi)log)\b" "global $1")

_s=
 SECTION .text   align=1
 _SEH_prolog:
  push        _except_handler3
  mov         eax, [fs:0] 
  push        eax  
  mov         eax, [esp+10h] 
  mov         dword [esp+10h],ebp 
  lea         ebp,[esp+10h] 
  sub         esp,eax 
  push        ebx  
  push        esi  
  push        edi  
  mov         eax, [ebp-8] 
  mov         dword [ebp-18h],esp 
  push        eax  
  mov         eax, [ebp-4] 
  mov         dword [ebp-4],0FFFFFFFFh 
  mov         [ebp-8],eax 
  lea         eax,[ebp-10h] 
  mov         [fs:0],eax 
  ret              
 _SEH_epilog:
  mov         ecx, [ebp-10h] 
  mov         [fs:0],ecx 
  pop         ecx  
  pop         edi  
  pop         esi  
  pop         ebx  
  leave            
  push        ecx  
  ret              

s.addline(_s)


 #sub Callback_str_replacerx
 function# REPLACERXCB&x
 out x.match
