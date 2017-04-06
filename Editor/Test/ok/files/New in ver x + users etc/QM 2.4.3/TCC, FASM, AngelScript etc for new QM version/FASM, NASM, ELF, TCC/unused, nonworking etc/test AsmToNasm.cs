 AsmToNasm("Q:\app\test\Release\test.asm" "Q:\Downloads\objconv\TestObj.o")
AsmToNasm("Q:\app\qmcore\pcre\Release\pcre.asm" "Q:\Downloads\objconv\pcre.o")

 str s.getfile("Q:\app\test\Release\test.asm")
 int i=find(s "?testR2@@YAHH@Z ENDP")
  out i
 outb s+i 100 1
