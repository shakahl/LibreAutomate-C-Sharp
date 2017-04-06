str s
Http h.Connect("www.quickmacros.com")
h.FileGetPartial("features.html" s 100)
ShowText("" s)
