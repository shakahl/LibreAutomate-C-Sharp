 /
function $html

 Pastes text in HTML format.
 Works only with windows that support HTML format. For example, Word.
 Error if the active window does not support HTML format.

 EXAMPLES
 PasteHtml "<html><body>text <a href=''http://www.quickmacros.com''>Quick Macros</a> text</body></html>"
 
 str mySigFile="$desktop$\test.htm"
 str s.getfile(mySigFile)
 PasteHtml s


str sh.format("Version:1.0[]StartHTML:00000033[]%s" html)
sh.setsel("HTML Format")
err end _error
