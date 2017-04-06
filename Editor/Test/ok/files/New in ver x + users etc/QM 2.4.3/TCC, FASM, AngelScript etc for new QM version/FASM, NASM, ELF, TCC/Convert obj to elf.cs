out

 To use (link) MSVC .obj files with TCC:
 1. Call DisassembleObj to disassemble with objconv.exe to NASM .s file.
 2. Call AssembleElf to assemble with NASM to .o file (ELF).

 Successfully disassembled/assembled/linked pcre.obj.
 Need to be careful:
   Don't use global variables with ctors, dtors or non-static initializers, because they will not be called.
   I don't know how to use function pointers in global variables.
   Problems with exception handling. Better don't use it in the C/C++ code.

 Possible alternative: Let MSCV create .asm file. Process it (convert from MASM to NASM dialect) and assemble with NASM. Tried unsuccessfully. See AsmToNasm.
 Also tried to use FASM instead of NASM. It assembles much faster. However FASM creates invalid file: either TCC fails to compile, or the compiled code crashes.

#if 1
DisassembleObj "Q:\app\Release\TestObj.obj" "Q:\Downloads\objconv\TestObj.s"
AssembleElf("Q:\Downloads\objconv\TestObj.s" "Q:\Downloads\objconv\TestObj.o")
 AssembleElfF("Q:\Downloads\objconv\TestObj.s" "Q:\Downloads\objconv\TestObj.o")

#else
 DisassembleObj "Q:\app\qmcore\pcre\Release\pcre.obj" "Q:\Downloads\objconv\pcre.s"
 PF
 AssembleElf "Q:\Downloads\objconv\pcre.s" "Q:\Downloads\objconv\pcre.o"
 PN;PO

DisassembleObj "Q:\app\qmcore\pcre\Release\pcre.obj" "Q:\Downloads\objconv\pcre.s"
 AssembleElf "Q:\Downloads\objconv\pcre.s" "Q:\Downloads\objconv\pcre.o"
AssembleElfF "Q:\Downloads\objconv\pcre.s" "Q:\Downloads\objconv\pcre.o"
 DisassembleObj "Q:\app\qmcore\pcre\Release\maketables.obj" "Q:\Downloads\objconv\maketables.s"
 AssembleElf "Q:\Downloads\objconv\maketables.s" "Q:\Downloads\objconv\maketables.o"


 DisassembleObj "C:\Program Files\Microsoft Visual Studio 9.0\VC\lib\msvcrt_win2000.obj" "Q:\Downloads\objconv\msvcrt.s"
 AssembleElf "Q:\Downloads\objconv\msvcrt.s" "Q:\Downloads\objconv\msvcrt.o"
