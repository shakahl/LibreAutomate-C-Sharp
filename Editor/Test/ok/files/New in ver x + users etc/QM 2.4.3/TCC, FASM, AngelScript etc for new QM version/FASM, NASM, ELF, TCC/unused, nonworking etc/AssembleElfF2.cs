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
s.replacerx("(?m)^SECTION +(\S+)(?:( +align)=(\d+))?.+[]" "section ''$1'' executable $2 $3[]") ;;executable writeable


s.replacerx("\bmatch\b" "match_1_") ;;error 'invalid macro argument'
 s.replacerx("(?m)\bnear\b") ;;error 'invalid name'
s.replacerx("(?m)\$([\w])" "__s_$1") ;;error 'invalid name'
s.replacerx("\bresd\b" "dd")
s.replacerx("(?m)^ALIGN\b.+[]")
s.replacerx("(\bbyte\b.+, )0FFFFFF(\w\wH)" "$1 0$2")


s-"use32[]format ELF[]"
s.setfile(stf)
PF
 ec=RunConsole2(F"Q:\Downloads\fasmw17122\FASM.exe {stf} {elfFile}")
ec=RunConsole2(F"Q:\Downloads\fasmw17122\FASM.exe {stf} {elfFile} -m 5000 -p 30")
 out s
PN;PO
ret ec=0
