 Converts a function from MASM (in a MSVC .asm file) to FASM, and stores in clipboard.

out
lpstr sFile sFunc
sFile="Q:\app\test\Release\test.asm"
sFunc="testR2"


str s sFD.getfile(sFile)
if(findrx(sFD "(?ms)^_TEXT	SEGMENT[](.+?^(\S+)[ \t]+PROC\b.+?)^\2[ \t]+ENDP\b" 0 0 s 1)<0) ret
s.replacerx("(?m)^(\S+)[ \t]+PROC[^\r\n]*" "$1:" 4) ;;Func PROC ;comments -> Func:

s.findreplace("[][]" "[]" 8)
s.replacerx("  +" " ")
s.replacerx("(?m)^; \d+ *:" " ;;") ;;remove line numbers etc from commented source code
s.replacerx("\b(\w+) PTR\b" "$1")
s.replacerx("\$(L[NL]\d+)@(\w+)" "_$1_$2") ;;replace $ to _ in label names
s.replacerx("\?(\w+)[\w@]+" "$1") ;;unmangle function names
s.replacerx("[\t ]+; [^\r\n]+") ;;remove comments
 s.replacerx("(_\w+\$\d*)\[(.+?)\]" "[$2+$1]") ;;var[reg] -> [reg+var]
s.replacerx("(_\w+\$\d*)\[(.+?)\]" "[$2+$1]") ;;var[reg] -> [reg+var]

out s

s.setclip
act _hwndqm
