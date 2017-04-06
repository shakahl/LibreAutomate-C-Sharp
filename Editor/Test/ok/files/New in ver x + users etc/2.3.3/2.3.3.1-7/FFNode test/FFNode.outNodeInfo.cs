 /FFNode test
function FFDOM.ISimpleDOMNode'x FFNODEINFO&n level

 sel(n.nodeType) case [FFDOM.NODETYPE_TEXT,FFDOM.NODETYPE_ELEMENT] ret

 indent
str st.all(level 2 9)

 name, value
str sName(n.bName) sValue(n.bValue)
sValue.findreplace("[10]" " ")
out "<>%s%i  %-10s  ''<c ''0x8000''><_>%s</_></c>'' %i" st n.nodeType sName sValue n.numChildren

 atributes
sel n.nodeType
	case [FFDOM.NODETYPE_TEXT,FFDOM.NODETYPE_COMMENT,FFDOM.NODETYPE_DOCUMENT]
	case else
	ARRAY(BSTR) an.create(100) av.create(100); ARRAY(word) ansid.create(100); word na
	x.get_attributes(an.len &an[0] &ansid[0] &av[0] &na); err out "<><c ''0xff''>no attributes</c>"
	if na
		str sa(F"<>{st}<c ''0xc0e0''><_>") san sav
		int i
		for(i 0 na) sa.formata("%s=''%s''  " san.from(an[i]) sav.from(av[i]))
		sa+"</_></c>"
		out sa

 inner html
if level>1
	sel n.nodeType
		case [FFDOM.NODETYPE_TEXT,FFDOM.NODETYPE_COMMENT]
		case else
		if(n._NamespaceId!9)
			str sHtml=x.innerHTML; err out "<><c ''0xff''>no HTML</c>"
			if sHtml.len
				sHtml.findreplace("[10]" " ")
				if(sHtml.len>1000) sHtml.fix(1000); sHtml+"............."
				out "<>%s<c ''0xff0000''><_>%s</_></c>" st sHtml
