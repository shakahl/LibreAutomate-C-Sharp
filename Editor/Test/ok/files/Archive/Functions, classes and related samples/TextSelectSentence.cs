
 Selects sentence of text.
 How it works:
   Repeatedly presses Ctrl+Shift+Right to select next word.
   Gets selected text through clipboard.
   Stops when selected text ends with a sentence separator character (.?!;).

 Works in Word, Notepad, Firefox and other programs that support Ctrl+Shift+Right.


str s
int i lenp
spe 1
s.getsel

rep
	'CSR
	lenp=s.len
	s.getsel
	if(s.len<=lenp) break ;;break if did not select more text
	i=findcs(s ".;?!" lenp); if(i<0) continue
	sel s[i]
		case '.'
		if(i and findrx(s "\b\w\." i-1)>=0) continue ;;eg J. Smith
		if(i>=2 and findrx(s "\b\w\w\." i-2)>=0 and isupper(s[i-2])) continue ;;eg Dr. Smith
		break
		
		case ['?','!',';']
		break
