 Functions of this class manipulate <help #IDP_ACCESSIBLE>accessible objects</help>.
 There are two dialogs in the floating toolbar that you can use to insert these functions.
 Most functions throw error if failed.

 EXAMPLE
 Click link in web page
int w=wait(3 WV win(" Internet Explorer" "IEFrame"))
Acc a.Find(w "LINK" "Advanced search" "" 0x3001 3)
a.DoDefaultAction
