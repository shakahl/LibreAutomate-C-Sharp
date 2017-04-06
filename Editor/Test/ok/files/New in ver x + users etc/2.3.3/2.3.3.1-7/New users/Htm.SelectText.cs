function [iStart] [iLength]

 Selects text of this element.
 Error if fails.

 iStart - selection start offset.
 iLength - selection length. If omitted or 0, selects all to the end. If negative, selection will be empty.


MSHTML.IHTMLTxtRange r
MSHTML.IHTMLBodyElement b
MSHTML.IHTMLTextAreaElement ta
MSHTML.IHTMLInputElement in

_s=el.tagName
sel _s 1
	case "BODY" b=+el; r=b.createTextRange
	case "TEXTAREA" ta=+el; r=ta.createTextRange
	case "INPUT" in=+el; r=in.createTextRange; err ;;only for type=text. Fails if moveToElementText.

if !r
	b=el.document.body
	r=b.createTextRange
	r.moveToElementText(el)

if(iStart) r.moveStart("character" iStart)
if(iLength) r.collapse(TRUE); if(iLength>0) r.moveEnd("character" iLength)

r.select

err+ end _error
