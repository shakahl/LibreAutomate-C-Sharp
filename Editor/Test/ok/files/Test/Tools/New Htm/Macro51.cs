web "http://www.google.com/advanced_search" 0x1
MSHTML.IHTMLElement el=htm("INPUT" "as_q" "" " Internet Explorer" 0 0 0x221)
el.innerText="macro"
el=htm("INPUT" "as_epq" "" " Internet Explorer" 0 3 0x221)
el.innerText="quick macros"
el=htm("SELECT" "num" "" " Internet Explorer" 0 0 0x221)
MSHTML.IHTMLSelectElement elsel=+el; elsel.selectedIndex=3
el=htm("INPUT" "Google Search" "" " Internet Explorer" 0 2 0x421)
el.click
