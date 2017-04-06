out

AssembleElf "Q:\Downloads\objconv\TestTccLink.s" "Q:\Downloads\objconv\TestTccLink.o"


 AssembleElf "Q:\Downloads\objconv\pcre.s" "Q:\Downloads\objconv\pcre.o"
 AssembleElf "Q:\Downloads\objconv\maketables.s" "Q:\Downloads\objconv\maketables.o"



 FASM
 str prog="Q:\Downloads\fasmw17122\FASM.exe"
 RunConsole2 F"{prog} {fileS} {fileO}"
