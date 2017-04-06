 /TCC - PCRE2
 /Convert obj to elf
function! $asmFile $elfFile

 Assembles asmFile (NASM .s files) with FASM. Creates elfFile (.o file).


int ec
str s stf

stf.expandpath("$temp$\fasm.s")
s.getfile(asmFile)

s.replacerx("(?m)^(global +\w+):function\b" "$1")
s.replacerx("(?m)^global\b" "public")
s.replacerx("(?m)^extern\b" "extrn")

 s.replacerx("(?m)^SECTION +(\S+)(?:( +align)=(\d+))?.+[]" "section ''$1'' executable writeable $2 $3[]") ;;executable writeable
s.replacerx("(?m)^SECTION .text\b" "section ''.text'' executable")
s.replacerx("(?m)^SECTION .data\b" "section ''.data'' writeable")
s.replacerx("(?m)^SECTION .bss\b" "section ''.data'' writeable") ;;TCC fails to link if bss
s.replacerx("(?m)^SECTION .rdata progbits alloc noexec nowrite align=" "section ''.rdata'' align ")
 FASM creates invalid ELF files. TCC either fails to link, or exception at run time, probably bad relocations.

s.replacerx("\bmatch\b" "match_1_") ;;error 'invalid macro argument'
 s.replacerx("(?m)\bnear\b") ;;error 'invalid name'
s.replacerx("(?m)\$([\w])" "__s_$1") ;;error 'invalid name'
s.replacerx("\bresd\b" "dd")
s.replacerx("(?m)^ALIGN\b.+[]")
s.replacerx("(\bbyte\b.+, )0FFFFFF(\w\wH)" "$1 0$2")


s-"use32[]format ELF[]"

type LINE_HEADER $file_path line_number file_offset []LINE_HEADER*macro_calling_line LINE_HEADER*macro_line
type FASM_STATE condition output_length []error_code !*output_data []LINE_HEADER*error_line
dll "Q:\Downloads\fasmDLL\FASM.dll"
	#fasm_Assemble $lpSource !*lpMemory cbMemorySize nPassesLimit hDisplayPipe

PF
int nmem=2000000
str mem.all(nmem 2)
FASM_STATE& fs=mem
PN
int hr=fasm_Assemble(s mem nmem 100 0)
PN;PO
if(hr) ret FasmError(hr fs s)
s.fromn(fs.output_data fs.output_length)
s.setfile(elfFile)
ret 1
