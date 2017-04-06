function `table [ARRAY(str)&a] [ARRAY(MSHTML.IHTMLElement)&a2] [flags] [str&tableText] ;;flags: 1 get HTML, 2 get all TD elements, including of inner tables

 Gets cells of a HTML table to a 1-dim array.
 Error if something fails, eg the specified table does not exist.

 table - table name or 0-based index. See <help>HtmlDoc.GetHtmlElement</help>. QM 2.4.4: also can be table's MSHTML.IHTMLElement or derived COM interface.
 a - array variable for results. The function creates 1-dimension array where each element is cell text. Can be 0 if not needed.
 a2 - array variable for results. The function creates 1-dimension array where each element is cell object that can be used to get html elements within the cell. Can be 0 if not needed.
 tableText - str variable that receives whole text or html of the table. Can be 0 if not needed.

 EXAMPLE
 HtmlDoc d.InitFromWeb("http://www.weather.com/weather/tenday/48183")
 ARRAY(str) a; d.GetTable(12 a)
  display text in first cell of each row
 int i ncolumns=2
 for i 0 a.len ncolumns
	 out a[i]
	  out a[i+1] ;;second cell, and so on
	 out "---------"

  EXAMPLE2
 HtmlDoc d.InitFromWeb("http://www.weather.com/weather/tenday/48183")
 ARRAY(str) a; d.GetTable(12 a 0 2) ;;uses flag 2 to get all TD elements, including of inner tables
  display text in some cells of each row, including cells of inner tables
 int i nTDinRow=8
 for i 0 a.len nTDinRow
	 out a[i+1]
	 out "---"
	 out a[i+2]
	 out "---"
	 out a[i+5]
	 out "----------------"

  EXAMPLE3
 HtmlDoc d.InitFromWeb("http://www.weather.com/weather/tenday/48183")
 ARRAY(MSHTML.IHTMLElement) a; d.GetTable(12 0 a)
  display tag and text of all inner html elements of first cell in each row
 int i ncolumns=2
 for i 0 a.len ncolumns
	 MSHTML.IHTMLElement el
	 foreach el a[i].all
		 str tag=el.tagName
		 str txt=el.innerText
		 out "<%s> %s[]" tag txt
	 out "----------------"


MSHTML.IHTMLElement t=GetHtmlElement("TABLE" table)
if(!t) end F"{ERR_OBJECT} (table)"

if(&tableText) if(flags&1) tableText=t.innerHTML; else tableText=t.innerText

MSHTML.IHTMLElementCollection cells
if(flags&2)
	MSHTML.IHTMLElement2 t2=+t
	cells=t2.getElementsByTagName("TD")
else
	MSHTML.IHTMLTable2 t3=+t
	cells=t3.cells

int i nc=cells.length
if(&a) a.create(nc)
if(&a2) a2.create(nc)
 
for i 0 nc
	MSHTML.IHTMLElement el=cells.item(i)
	if(&a) if(flags&1) a[i]=el.innerHTML; else a[i]=el.innerText
	if(&a2) a2[i]=el

err+ end ERR_FAILED
