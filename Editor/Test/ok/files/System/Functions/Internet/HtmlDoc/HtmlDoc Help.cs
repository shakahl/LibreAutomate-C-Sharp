 Use this class to parse HTML, ie web page source.
 Can download web page from Internet, or get from web browser, or from local file, or from string or Htm variable.
 Can extract text of whole page or a html element, get table cells, get links, get html element objects, etc.
 For example, extract table data and process it or save or pass to Excel.

 At first declare a HtmlDoc variable and call one of InitX functions to load HTML.
 Then call other functions to get HTML parts.
 Some examples: <open>web table to excel</open>, <link "http://www.quickmacros.com/forum/viewtopic.php?f=2&t=3368">download more</link>.

 When loading HTML that contains scripts, executes them, unless before called SetOptions with flag 1.
 Scripts work not with all pages, because by default HtmlDoc does not use browser window. Try to call SetOptions with flag 2.

 You also can use whole MSHTML document object model (DOM): call functions of d and d3, functions of returned objects, and functions of all other interfaces that you can access through their functions or by assigning them to variables of other MSHTML interface types using =+. See how HtmlDoc functions do it.

 EXAMPLES
 this example gets text of a web page
HtmlDoc d
d.SetOptions(2)
d.InitFromWeb("http://www.quickmacros.com")
str s=d.GetText
out s

 this example shows how to access DOM
HtmlDoc dd
dd.SetOptions(2)
dd.InitFromWeb("http://www.quickmacros.com")
out dd.d3.getElementById("m_features").outerHTML
