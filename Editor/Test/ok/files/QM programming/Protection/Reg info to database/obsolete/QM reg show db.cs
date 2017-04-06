web "http://www.quickmacros.com:2082/3rdparty/phpMyAdmin/index.php" 1
int h=win("phpMyAdmin"); act h
MSHTML.IHTMLElement el=htm("SELECT" "lightm_db" "" h "1" 0 0x221 5)
MSHTML.IHTMLElement2 el2=+el; el2.focus
key DD
HtmlClick("IMG" "Browse" "" h "3" 0 0x321 5 -1)
HtmlSetText("100" "INPUT" "session_max_rows" "" h "3" 8 0x221 5)
HtmlSetText("150" "INPUT" "pos" "" h "3" 9 0x221)
HtmlClick("INPUT" "Show :" "" h "3" 7 0x421)
