 find edit field
MSHTML.IHTMLElement el=htm("INPUT" "as_q" "" "Google Advanced Search" 0 0 0x221)
 store text into a variable
str a=el.getAttribute("value" 0)
 and so on
el=htm("INPUT" "as_epq" "" "Google Advanced Search" 0 3 0x221)
str b=el.getAttribute("value" 0)
el=htm("INPUT" "as_oq" "" "Google Advanced Search" 0 4 0x221)
str c=el.getAttribute("value" 0)
el=htm("INPUT" "as_eq" "" "Google Advanced Search" 0 5 0x221)
str d=el.getAttribute("value" 0)

 activate Excel and enter the variables
act "Excel"
key (a) T (b) T (c) T (d) HD ;;type a, press Tab, ..., press Home and Down