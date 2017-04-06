 /Convert obj to elf
function! $asmFile $elfFile

 Assembles asmFile (.s file) with NASM. Creates elfFile (.o file).


str cl
cl=" -O1"

int ec=RunConsole2(F"$qm$\nasm.exe ''{asmFile}'' -o ''{elfFile}'' -f elf -w-number-overflow{cl}" _s)
 if(_s.len) _s.replacerx("(?m)^.+? warning: (Unknown section attribute|incompatible section attributes ignored on redeclaration of section).+\n?")
out _s
ret ec=0

 Optimizations:
 With default optimization (-Ox) slowly assembles (3.6 s). Pcre.o file size 47KB. Execution speed same as original (VC-compiled).
 With -O0 also slowly assembles, and other parameters are not good.
 With -O1 faster assembles (0.65 s). Pcre.o file size 50KB. Execution speed slightly slower than original, maybe 1-2%.

 Warnings:
 Even -w-all does not remove warnings "incompatible section attributes ignored on redeclaration of section".
