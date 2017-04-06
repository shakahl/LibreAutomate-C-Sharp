#ret
def IID_ISimpleDOMNode uuidof("{1814ceeb-49e2-407f-af99-fa755a7d2607}")
interface# ISimpleDOMNode :IUnknown
	get_nodeInfo(BSTR*nodeName @*nameSpaceID BSTR*nodeValue *numChildren *uniqueID @*nodeType)
	get_attributes(@maxAttribs BSTR*attribNames @*nameSpaceID BSTR*attribValues @*numAttribs)
	get_attributesForNames(@numAttribs BSTR*attribNames @*nameSpaceID BSTR*attribValues)
	get_computedStyle(@maxStyleProperties !useAlternateView BSTR*styleProperties BSTR*styleValues @*numStyleProperties)
	get_computedStyleForProperties(@numStyleProperties !useAlternateView BSTR*styleProperties BSTR*styleValues)
	scrollTo(!placeTopLeft)
	[g]ISimpleDOMNode'parentNode()
	[g]ISimpleDOMNode'firstChild()
	[g]ISimpleDOMNode'lastChild()
	[g]ISimpleDOMNode'previousSibling()
	[g]ISimpleDOMNode'nextSibling()
	[g]ISimpleDOMNode'childAt(childIndex)
	[g]BSTR'innerHTML()
	[g]!*__()
	[g]BSTR'language()
	{1814ceeb-49e2-407f-af99-fa755a7d2607}
def NODETYPE_ATTRIBUTE 2
def NODETYPE_CDATA_SECTION 4
def NODETYPE_COMMENT 8
def NODETYPE_DOCUMENT 9
def NODETYPE_DOCUMENT_FRAGMENT 11
def NODETYPE_DOCUMENT_TYPE 10
def NODETYPE_ELEMENT 1
def NODETYPE_ENTITY 6
def NODETYPE_ENTITY_REFERENCE 5
def NODETYPE_NOTATION 12
def NODETYPE_PROCESSING_INSTRUCTION 7
def NODETYPE_TEXT 3

def IID_ISimpleDOMText uuidof("{4e747be5-2052-4265-8af0-8ecad7aad1c0}")
interface# ISimpleDOMText :IUnknown
	[g]BSTR'domText()
	get_clippedSubstringBounds(startIndex endIndex *x *y *width *height)
	get_unclippedSubstringBounds(startIndex endIndex *x *y *width *height)
	scrollToSubstring(startIndex endIndex)
	[g]BSTR'fontFamily()
	{4e747be5-2052-4265-8af0-8ecad7aad1c0}

def IID_ISimpleDOMDocument uuidof("{0D68D6D0-D93D-4d08-A30D-F00DD1F45B24}")
interface# ISimpleDOMDocument :IUnknown
	[g]BSTR'URL()
	[g]BSTR'title()
	[g]BSTR'mimeType()
	[g]BSTR'docType()
	[g]BSTR'nameSpaceURIForID(@nameSpaceID)
	[p]alternateViewMediaTypes(BSTR*commaSeparatedMediaTypes)
	{0D68D6D0-D93D-4d08-A30D-F00DD1F45B24}
