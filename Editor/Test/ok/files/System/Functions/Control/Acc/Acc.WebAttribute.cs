function~ $name [flags] ;;flags: 0 interpolated (IE), 1 case sensitive (IE), 2 as in HTML (IE)

 Returns value of a HTML attribute.
 Returns empty string if fails or if the attribute does not exist.

 name - attribute name (eg "href").

 REMARKS
 Works with Internet Explorer, Firefox, possibly Chrome. Read more in <help>Acc.WebPageProp</help>.

 Added in: QM 2.3.3.


if(!a) end ERR_INIT

Htm e; FFNode f
sel __HtmlObj(e f)
	case 1
	ret e.Attribute(name flags)
	
	case 2
	ret f.Attribute(name)
	
	case else end ERR_FAILED

err+
