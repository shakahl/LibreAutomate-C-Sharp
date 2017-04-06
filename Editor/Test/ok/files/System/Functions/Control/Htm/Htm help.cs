 Functions of this class manipulate <help "htm">html elements</help>.
 There are two dialogs in the floating toolbar that you can use to insert these functions.
 Most functions throw error if failed.

 EXAMPLE
 Click link in web page
int w=wait(3 WV win(" Internet Explorer" "IEFrame"))
Htm e=htm("A" "Advanced search" "" w 0 3 0x21 3)
e.Click
