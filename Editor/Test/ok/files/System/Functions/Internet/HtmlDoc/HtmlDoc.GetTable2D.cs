function `table [ARRAY(str)&a] [ARRAY(MSHTML.IHTMLElement)&a2] [flags] ;;flags: 1 get HTML

 Gets cells of a HTML table to a 2-dim array.
 Error if something fails, eg the specified table does not exist.

 table - table name or 0-based index or COM interface (eg MSHTML.IHTMLElement or IHTMLTable). See <help>HtmlDoc.GetHtmlElement</help>.
 a - array variable for results. The function creates 2-dimension array where each element is cell text. Can be 0 if not needed.
 a2 - array variable for results. The function creates 2-dimension array where each element is cell object that can be used to get html elements within the cell. Can be 0 if not needed.

 REMARKS
 If some cells have colspan or/and rowspan attribute, adds empty array elements, so that all array element positions match the visual positions of cells.

 Added in: QM 2.4.4.


MSHTML.IHTMLTable t=+GetHtmlElement("TABLE" table)
if(!t) end F"{ERR_OBJECT} (table)"

MSHTML.IHTMLTableRow tr
MSHTML.IHTMLTableCell td
MSHTML.IHTMLElement el

int i nc nr=t.rows.length
 calc n columns; t.cols gives 0
foreach tr t.rows
	i=0
	foreach(td tr.cells) i+td.colSpan
	if(i>nc) nc=i

if(&a) a.create(nc nr)
if(&a2) a2.create(nc nr)

ARRAY(int) ars.create(nc) ;;all rowSpan of prevous row

int r c rowSpan colSpan
foreach tr t.rows
	c=0
	foreach td tr.cells
		 colSpan/rowSpan
		if(r) rep() if(ars[c]) ars[c]-1; c+1; else break
		
		el=+td
		if(&a) if(flags&1) a[c r]=el.innerHTML; else a[c r]=el.innerText
		if(&a2) a2[c r]=el
		
		 colSpan/rowSpan
		rowSpan=td.rowSpan; colSpan=td.colSpan
		for(i 0 colSpan) ars[c+i]=rowSpan-1
		c+colSpan
	r+1

err+ end ERR_FAILED
