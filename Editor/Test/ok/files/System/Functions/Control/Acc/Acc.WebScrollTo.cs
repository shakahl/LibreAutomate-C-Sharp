
 Scrolls web page to make this object visible.

 REMARKS
 Works with Internet Explorer and Firefox. Does not work with Chrome. Read more in <help>Acc.WebPageProp</help>.
 Offscreen objects may have "hidden" state. To find such objects, check "+ invisible".

 Added in: QM 2.3.3.


if(!a) end ERR_INIT

Htm e; FFNode f
sel __HtmlObj(e f)
	case 1
	e.Scroll
	
	case 2
	f.ScrollTo
	
	case else end ERR_FAILED

err+ end ERR_FAILED
