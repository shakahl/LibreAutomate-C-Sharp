 /
function MSHTML.HTMLDocument'doc str&s

MSHTML.HTMLInputElement te=+doc.getElementById("t")
str s1=te.getAttribute("value" 0)

MSHTML.HTMLInputElement ch=+doc.getElementById("c")
str s2=ch.getAttribute("checked" 0)

s.format("text: %s[]checkbox: %s" s1 s2)
