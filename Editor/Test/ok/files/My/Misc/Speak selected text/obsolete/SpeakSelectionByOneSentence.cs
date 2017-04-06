if(getopt(nthreads)>1) shutdown -6 0 "SpeakSelection"; 0.3

AddTrayIcon "sound.ico"
int+ __speak_sel_info
str si=
 This function speaks selected text, one sentence at a time.
 Use these keys to control it:
 
 Left - repeat this sentence.
 Right - next sentence (in the selected text).
 Up - previous sentence.
 Down or other key - stop.

 Down or other key - stop. (edit: only Down key can be used to stop)

if(!__speak_sel_info) __speak_sel_info=1; mes si "" "i"

str s.getsel
if(!s.len) ret
ARRAY(str) a
tok s a -1 "[].;?!" ;;end-of-sentence characters
int i
for(i a.len-1 -1 -1) a[i].trim(" [9]()"); if(!a[i].len) a.remove(i)

for i 0 a.len
	Speak a[i] 0 "" -1
	sel wait(0 KF)
		case VK_RIGHT
		case VK_LEFT i-1
		case VK_UP i-iif(i 2 1)
		case else SpeakStop; ret

 for i 0 a.len
	 Speak a[i] 0 "" -1
	  g1
	 int vk=wait(0 KF)
	 sel vk
		 case VK_RIGHT
		 case VK_LEFT i-1
		 case VK_UP i-iif(i 2 1)
		  case else SpeakStop; ret
		  edit:
		 case VK_DOWN SpeakStop; ret
		 case else key+ (vk); goto g1
