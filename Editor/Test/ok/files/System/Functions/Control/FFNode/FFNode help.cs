 Finds a HTML node COM object and gets properties.

 Works with Firefox, Thunderbird and other windows that use Gecko engine.
 In Chrome works if Firefox is installed, but not everything.
 If using portable Firefox, <link "http://www.quickmacros.com/forum/viewtopic.php?f=1&t=5551">look here</link>.

 Node objects are similar to accessible objects. They provide more info: attributes, HTML, full DOM tree.
 Most Firefox/Chrome accessible objects can be converted to node objects (Acc to FFNode). Many node objects can be converted to accessible objects (FFNode to Acc).

 HOW TO USE
 Declare variable and call FindFF or one of FromX functions. Then call other functions.
 To create code for FindFF, use the 'Find accessible object' dialog. Check 'as Firefox node'. It inserts code for Acc. If you don't need Acc, replace 'Acc' with 'FFNode'.

 EXAMPLE
int w=win("Google - Mozilla Firefox" "Mozilla*WindowClass" "" 0x804)
FFNode f.FindFF(w "A" "Gmail" "" 0x1001)
out f.Attribute("href")

 also you can use ISimpleDOMNode functions
f=f.node.parentNode
out f.Tag
