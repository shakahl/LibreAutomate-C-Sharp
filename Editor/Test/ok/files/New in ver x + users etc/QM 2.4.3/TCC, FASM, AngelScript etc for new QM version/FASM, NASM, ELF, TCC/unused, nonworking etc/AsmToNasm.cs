 /test AsmToNasm
function! $asmFile $elfFile

 Assembles asmFile (MSVC-created .asm file) with NASM. Creates elfFile (.o file).
 Not finished. Difficult, unreliable, I don't know how to convert everything correctly.

out
lpstr sFunc
sFunc="testR2"

str s.getfile(asmFile)
s.replacerx("(?ms).+?[](?:INCLUDELIB\s\w+[])+(.+[])END[]" "$1" 4) ;;remove head and tail
s.findreplace("[][]" "[]" 8) ;;remove empty lines
 s.replacerx("[ \t]{2,}" " ") ;;replace multiple spaces and tabs to single space
s.replacerx("[\t ]+;[^\r\n]+") ;;remove comments in code lines
s.replacerx("(?m)^; \d+ *: " " ;") ;;remove line numbers etc from commented source code
s.replacerx("(?m)^;.*[]") ;;remove comments such as "Function compile flags"
s.replacerx("(?m)^\S+[ \t]+END[SP]$") ;;remove Sec ENDS and Func ENDP
s.findreplace("[][][]" "[][]" 8) ;;replace 2 empty lines to 1
s.replacerx("(?m)^CONST	SEGMENT" "section .rdata")
s.replacerx("(?m)^_TEXT	SEGMENT" "section .text")
s.replacerx("(?m)^_DATA	SEGMENT" "section .data")
s.replacerx("(?m)^_BSS	SEGMENT" "section .bss")
s.replacerx("(?m)^PUBLIC\b" "global")
s.replacerx("(?m)^EXTRN\b" "extern")
 s.replacerx("(?m)^(extern\b.+:)PROC$\b" "$1function") ;;error
s.replacerx("(?m)^(extern\b.+):PROC$\b" "$1") ;;remove :PROC
s.replacerx("(?m)^(\S+)[ \t]+PROC" "$1:") ;;Func PROC ;comments -> Func:
 s.replacerx("(?m)^(\S+) = " "$1 equ ")
 s.replacerx("(?m)^(\S+) = ([^\r\n]+)" "$1 equ $2[]$1 equ -$2") ;;error, cannot redefine
s.replacerx("(?m)^(\S+) = ([^\r\n]+)" "%define $1 $2")
 s.replacerx("(?m)^(\S+) = ([^\r\n]+)" "%define $1 $2[]%define $1 -$2") ;;no error to redefine
s.replacerx("(?m)^(\t.+?)[ \t]+PTR\b" "$1") ;;remove PTR
s.replacerx("(?m)^(\t.+?)[ \t]+OFFSET\b" "$1") ;;remove OFFSET
s.replacerx("(?m)^(\tlea\s..., )DWORD\b" "$1") ;;remove DWORD from lea
s.replacerx("(?m)^(\t.+?)(\S+)\[(.+?)\]" "$1[$3+$2]") ;;var[reg] -> [reg+var]
 s.replacerx("\$(L[NL]\d+)@(\w+)" "_$1_$2") ;;replace $ to _ in label names. Would need this for FASM only.
 s.replacerx("\?(\w+)[\w@]+" "$1") ;;unmangle function names
s.replacerx("(?m)^\tnpad\t\d" "align 4") ;;alignment
s.replacerx("(?m)^\tORG \$\+\d" "align 4") ;;alignment
s.replacerx("\bFLAT:") ;;DD FLAT:
s.rtrim

 out s;; ret
s.setmacro("asm")
 s.getmacro("asm")

str sFile.expandpath("$temp$\conv.s")
s.setfile(sFile)

str cl
cl=" -O1" ;;faster assembling, slightly slower execution

int ec=RunConsole2(F"$qm$\nasm.exe ''{sFile}'' -o ''{elfFile}'' -f elf -w-number-overflow{cl}" _s)
 if(_s.len) _s.replacerx("(?m)^.+? warning: (Unknown section attribute|incompatible section attributes ignored on redeclaration of section).+\n?")
out _s
if(ec) ret
ret 1

 Optimizations:
 With default optimization (-Ox) slowly assembles (3.6 s). Pcre.o file size 47KB. Execution speed same as original (VC-compiled).
 With -O0 also slowly assembles, and other parameters are not good.
 With -O1 faster assembles (0.65 s). Pcre.o file size 50KB. Execution speed slightly slower than original, maybe 1-2%.

 Warnings:
 Even -w-all does not remove warnings "incompatible section attributes ignored on redeclaration of section".
