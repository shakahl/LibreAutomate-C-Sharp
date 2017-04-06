 \
function $_file

 Executes macro code stored in file.
 Warning: Don't call this function several times in sequence.

 EXAMPLE
 RunFileAsMacro "$desktop$\test.txt"


opt noerrorshere 1

str s.getfile(_file)
RunTextAsMacro s
