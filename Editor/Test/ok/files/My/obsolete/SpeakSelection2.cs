 This does not work anymore, because Speak() is implemented differently than before.


if(getopt(nthreads)>1) key U; 0.3

int+ __speak_sel_info2
str si=
 This function speaks selected text.
 Use these keys to control it:
 
 Left - repeat this sentence.
 Right - next sentence (in the selected text).
 Up - previous sentence.
 Down - pause/resume.
 Other keys (eg Ctrl) - stop.
if(!__speak_sel_info2) __speak_sel_info2=1; mes si "" "i"

str s.getsel
if(!s.len) ret
s.findreplace("..." ".")
int paused
Speak s 0 "" -1
rep
	sel wait(0 KF)
		case VK_LEFT __spvoice.Skip("Sentence" 0); continue
		case VK_RIGHT __spvoice.Skip("Sentence" 1); continue
		case VK_UP __spvoice.Skip("Sentence" -1); continue
		case VK_DOWN paused^1; if(paused) __spvoice.Pause; else __spvoice.Resume
		case else SpeakStop; ret
