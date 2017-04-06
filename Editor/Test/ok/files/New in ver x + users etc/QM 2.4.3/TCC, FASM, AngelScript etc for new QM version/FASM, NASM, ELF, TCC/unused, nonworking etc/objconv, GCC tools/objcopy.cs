 Objconv is much better.

out
str prog="C:\MinGW\bin\objcopy.exe"

RunConsole2 F"{prog} --output-target=elf32-i386 --strip-debug Q:\app\qmcore\pcre\Release\pcre.obj Q:\Downloads\objconv\pcre.o"
 RunConsole2 F"{prog} --output-target=elf32-i386 --strip-debug --redefine-sym __imp__OutputDebugStringA@4=OutputDebugStringA --redefine-sym __imp__wvsprintfA@12=wvsprintfA --remove-leading-char Q:\app\qmcore\pcre\Release\pcre.obj Q:\Downloads\objconv\pcre.o"
 RunConsole2 F"{prog} -felf -nu- Q:\app\qmcore\pcre\Release\maketables.obj Q:\Downloads\objconv\maketables.o"

 RunConsole2 "C:\MinGW\bin\objdump.exe -i"
 elf32-i386 elf32-little elf32-big
