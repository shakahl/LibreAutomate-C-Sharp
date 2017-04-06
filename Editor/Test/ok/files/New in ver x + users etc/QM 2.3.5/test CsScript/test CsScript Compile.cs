/exe
 out

int flags
str macro asm
macro="cs1"; asm="cs.dll" ;;dll
 macro="cs1"; asm="cs.dll"; flags=0x300 ;;dll
 macro="cs exe win"; asm="win.exe"; flags=0x100 ;;exe win
 macro="cs exe console"; asm="console.exe"; flags=0x200 ;;exe console
 macro="cs exe win"; asm="win auto.EXE" ;;exe win

#exe addtextof "cs1"
str code.getmacro(macro)

 PF
CsScript x.Init
PF
x.SetOptions("inMemoryAsm=true")
 PN
x.Compile(code F"q:\my qm\test\{asm}" flags|0x1000)
PN
PO

 BEGIN PROJECT
 main_function  test CsScript Compile
 exe_file  $my qm$\test CsScript Compile.qmm
 flags  6
 guid  {6BA6E138-6E91-43B0-9DA0-9EE53A8B45BD}
 END PROJECT
