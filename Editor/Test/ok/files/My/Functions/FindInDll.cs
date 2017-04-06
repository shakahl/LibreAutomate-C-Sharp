 /
function! $_file $s [flags] ;;flags: 1 out ansi

str s1.getfile(_file)
str s2.all(s1.len/2 2)
if(!WideCharToMultiByte(CP_ACP 0 +s1 s1.len/2 s2 s2.len "[1]" 0)) ret
s2.findreplace("" " " 32)
s2.replacerx("[\x01-\x1f\x7f-\xff]")
s2.replacerx(" +" " ")

if(flags&1) out s2.wrap(200)

ret find(s2 s 0 1)>=0
