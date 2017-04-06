VARIANT d = "[0x52][0x80][0x00][0xd2]"
int i
for i 0 d.bstrVal.len
	out "0x%x" d.bstrVal[i]

 output:
 0x52
 0xfffd ;;different in Unicode and ANSI mode. Because interprets it as a character, not as byte.
