 find the Enter US$... element
MSHTML.IHTMLElement el=htm("B" "or more" "" "eBay item" 0 49 0x21 0 -1)
 get its text
str text=el.innerText
 extract the number
str amount
findrx(text "[\d\.,]+" 0 0 amount)

 example of manipulating the number
 double d=amount
 d+1
 amount=d

 find the adjacent input field
el=htm("B" "" "<B>or more</B>" "eBay item" 0 49 0x24 0 -4)
 set its text
el.innerText=amount

 also can click the button
el=htm("INPUT" "Place Bid >" "" "eBay item" 0 100 0x421)
 el.click
