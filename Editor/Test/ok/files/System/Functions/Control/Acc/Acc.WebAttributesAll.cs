function ARRAY(str)&array [flags] ;;flags: 0 interpolated (IE), 2 as in HTML (IE), 4 no empty attributes

 Gets all HTML attributes.

 array - variable for attributes.
   Will create 2-dim array.
   In first dimension will be 2 elements: for names and for values.
   If fails, array will be empty.

 REMARKS
 Works with Internet Explorer, Firefox, possibly Chrome. Read more in <help>Acc.WebPageProp</help>.

 Added in: QM 2.3.3.


if(!a) end ERR_INIT

Htm e; FFNode f
sel __HtmlObj(e f)
	case 1
	e.AttributesAll(array flags)
	
	case 2
	f.AttributesAll(array flags&4)
	
	case else end ERR_FAILED

err+
