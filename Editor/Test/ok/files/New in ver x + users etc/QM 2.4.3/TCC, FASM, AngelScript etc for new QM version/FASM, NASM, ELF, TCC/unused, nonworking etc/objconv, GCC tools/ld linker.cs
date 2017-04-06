out
str o
o="--syms"
o="--reloc"
 RunConsole2 F"C:\MinGW\bin\ld.exe -o Q:\Downloads\objconv\pcre.dll Q:\app\qmcore\pcre\Release\pcre.obj c:\windows\system32\msvcrt.dll Q:\app\qmcore\pcre\pcre.def"
 RunConsole2 F"C:\MinGW\bin\ld.exe -o Q:\Downloads\objconv\pcre.dll Q:\Downloads\objconv\pcre.o c:\windows\system32\msvcrt.dll Q:\app\qmcore\pcre\pcre.def"
 RunConsole2 F"C:\MinGW\bin\ld.exe -o Q:\Downloads\objconv\pcre.dll Q:\Downloads\objconv\pcre.o c:\windows\system32\msvcrt.dll Q:\app\qmcore\pcre\pcre.def"
 RunConsole2 F"C:\MinGW\bin\ld.exe -o Q:\Downloads\objconv\pcre.dll Q:\Downloads\objconv\pcre.o c:\windows\system32\msvcrt.dll Q:\app\qmcore\pcre\pcre.def"
 RunConsole2 F"C:\MinGW\bin\ld.exe -o Q:\Downloads\objconv\pcre.dll Q:\Downloads\objconv\TestTccLink.o"

RunConsole2 F"C:\MinGW\bin\gcc.exe -o Q:\Downloads\objconv\pcre.dll Q:\Downloads\objconv\pcre.o c:\windows\system32\msvcrt.dll Q:\app\qmcore\pcre\pcre.def"

#ret
q:\app\tcc\lib\msvcrt.def
C:\MinGW\lib\libcrtdll.a
''C:\Program Files\Microsoft Visual Studio 9.0\VC\lib\msvcrt_win2000.obj''
