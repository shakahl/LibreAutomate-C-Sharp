 /
function# $caption $text [hwndowner] [flags]

 Everything is the same as with <tip "#ShowText">ShowText</tip>, except:
   returns selected line index.
   flag 1 (modeless) unavailable.

 EXAMPLE
 int i=ShowTextAndReturnLineIndex("test" "one[]two[]three")
 out i


opt waitmsg 1
int i he=id(3 ShowText(caption text hwndowner flags|1))
rep
	0.01
	if(!IsWindow(he)) break
	i=SendMessage(he EM_LINEFROMCHAR -1 0)
ret i
