out
str s s1 s2
 s="one+two=three[][9]m[9][]k[10]k[2][13]k []k "
 s="one+two=three[]hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh hhhhhhhhhhhhhhhhhhhhhhhhhhhh"
s="ąčę zxc " ;;in Unicode mode
 s="��� zxc " ;;in ANSI mode
 s="šǂʏϻп"
 BSTR b=s; out b
 outb s s.len 1

str charset
 charset="windows-1257" ;;baltic
 charset="windows-1252" ;;western
 charset="utf-8"

Q &q
s1=s
s1.QuotedPrintableEncoding(0)
 s1.QuotedPrintableEncodingCDO(0 charset)
Q &qq
s2=s1
s2.QuotedPrintableEncoding(1)
 s2.QuotedPrintableEncodingCDO(1 charset)
Q &qqq; outq

out s
out "-----------"
out s1
 outb s1 s1.len 1
out "-----------"
out s2
 outb s2 s2.len 1
