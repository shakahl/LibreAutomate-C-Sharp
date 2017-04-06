function [str&url] [str&title] [str&html] [str&text]

 Gets web page properties.

 url, title, html, text - variables that receive page properties. Can be 0.

 REMARKS
 Works with:
   Internet Explorer and IE-based web browsers/controls.
   Firefox and other Gecko-based windows, eg Thunderbird. If using portable Firefox, <link "http://www.quickmacros.com/forum/viewtopic.php?f=1&t=5551">look here</link>.
   Chrome. Only if Firefox is installed. Error if used html or text.
 If the object is in a frame/iframe, gets properties of the frame/iframe, not of the main page.

 Added in: QM 2.3.3.


if(!a) end ERR_INIT

Htm e; FFNode f
sel __HtmlObj(e f)
	case 1
	e.DocProp(0 url title html text)
	
	case 2
	f.DocProp(0 url title html text)
	
	case else
	if(__FirefoxNotInstalled(_s)) end F"{ERR_OBJECTGET}. {_s}"
	end ERR_FAILED

err+ end ERR_FAILED
