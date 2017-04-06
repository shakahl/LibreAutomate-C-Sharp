 add more choices in message box
sel mes("<>Question.[][]Other choices:[]<a id=''1000''>Maybe</a>" "" "YN?")
	case 'Y' out "Yes"
	case 'N' out "No"
	case 1000 out "Maybe"

 add popup menu in input box
str s
if(!inp(s F"<><a href=''{&inp_callback_select3}''>select</a>")) ret
out s
