int prog
int w1=child(mouse)
if(!w1) goto gm

sel _s.getwinexe(w1) 1
	case "WINWORD"
	prog=1
	
	case "IEXPLORE"
	if(!childtest(w1 "" "Internet Explorer_Server")) goto gm
	prog=2
	 if link, middle-click and ret
	Acc _a=acc(mouse)
	Htm hm=htm(_a)
	 out hm.Tag
	sel(hm.Tag 1)
		case "A" goto gm
		case "BODY"
		case else _s=hm.Text; if(!_s.len) goto gm ;;IMG or some other object without text
	 out hm.Tag; ret
	
	case else goto gm


dou
str s.getsel
int rare=!IsCommonEnglishWord(s.trim)

sel prog
	case 1
	if(rare) 'CA[ ;;font color
	'CAh ;;highlight
	
	 The hotkeys in Word must be explicitly assigned. The Highlight command in the Word hotkeys dialog is under All Commands.
	
	case 2
	MSHTML.IHTMLDocument2 document=htm(w1) ;;info: in IE8 worked if document was IDispatch. In IE9 then execCommand fails. Why?
	IDispatch range = document.selection.createRange(); err ret
	document.selection.empty
	range.execCommand("BackColor", 0, "yellow");
	if(rare) range.execCommand("Italic", 0, 1);

ret
 gm
mid
