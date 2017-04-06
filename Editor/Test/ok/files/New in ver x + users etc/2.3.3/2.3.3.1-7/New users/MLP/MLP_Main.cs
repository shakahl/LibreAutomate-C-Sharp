 /
function what ;;what: 0 MSB, 1 LSB, 2 PRG

 Main function.
 TODO:
 Change timeouts.
 Maybe edit code to change behavior.
 Maybe edit functions in "private folder".
 When all works, edit MLP_Enter.

 Keys:
 F6, F7, F8 - shows corresponding OSD: MSB, LSB or PRG. If already visible, just activates. It does not activate OSD window, just makes other keys work with that OSD.
 0-9 and Backspace - changes text of the active OSD. Not numpad, although may be easily added.
 ~ - clears text of the active OSD.
 Enter - calls MLP_Enter and sets to hide the active OSD soon.
 Esc - hides all OSD's and ends all this.
 Other keys - don't use while an OSD is shown. Press Esc at first.

 __________________________________

 Change these values.

double timeWaitForKey(5) timeAfterEnter(1.5)
 __________________________________

type MLP_DATA whatActive str'text[3]
MLP_DATA+ g_mlp ;;global data used by this project. If need, add more variables to MLP_DATA.

g_mlp.whatActive=what ;;which OSD is active, ie accepts numbers and Enter. F6/F7/F8 activates corresponding OSD.

MLP_Osd what "___" -1 0!win(F"MLP_{what}" "QM_OSD_Class") ;;show OSD. If this OSD already exists, don't change text, just reset timeout.

if(getopt(itemid 1)=getopt(itemid)) ret ;;called from this function. We don't need more than one loop that waits for a key.

rep
	int k=wait(timeWaitForKey KF) ;;wait for any key, eat
	err ret ;;timeout
	 g1
	what=g_mlp.whatActive
	
	sel k
		 these three keys must match triggers. Need to use them here, because key triggers don't work when waiting for a key.
		case VK_F6 MLP_Main 0
		case VK_F7 MLP_Main 1
		case VK_F8 MLP_Main 2
		
		case [48,49,50,51,52,53,54,55,56,57,192,8] ;;0-9, Backspace, ~
		MLP_Text what k ;;change number or clear
		
		case VK_RETURN ;;Enter
		str s=g_mlp.text[what]; s.ltrim(" _0")
		if(s.len) MLP_Enter what val(s)
		MLP_Osd what "" timeAfterEnter 1 ;;change timeout
		ret
		
		case else ;;any other key
		if(k!VK_ESCAPE)
			 key (k) ;;retype. Not recommended. Problems with modifiers etc. Instead show warning.
			OnScreenDisplay "To hide OSD, use Esc." 0 0 0 "" 0 0 5 "MLP_info"
		ret ;;hides all OSD
