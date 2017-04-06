function$ [flags] ;;flags: 1 escape ' instead of ", 2 escape Unicode characters to \uXXXX

 Replaces JavaScript string special characters to escape sequences.
 Returns self.

 EXAMPLE
 str s="a '' ' \ [] [8][9][11][12] ąﻟ"
 s.EscapeJavascript(2)
 out s


if(!this.len) ret
findreplace("\" "\\")
if(flags&1) findreplace("'" "\'"); else findreplace("''" "\''")
findreplace("[]" "\r\n")
findreplace("[8]" "\b")
findreplace("[9]" "\t")
findreplace("[10]" "\n")
findreplace("[11]" "\u000B")
findreplace("[12]" "\f")
findreplace("[13]" "\r")

if flags&2 and findrx(this "[\x80-\xff]")>=0
	BSTR b=this; int i j
	str s.all(b.len*6 2)
	for i 0 b.len
		if(b[i]<128) s[j]=b[i]; j+1
		else s.set(F"\u{b[i]%%04X}" j); j+6
	s.fix(j); this.swap(s)

ret this
