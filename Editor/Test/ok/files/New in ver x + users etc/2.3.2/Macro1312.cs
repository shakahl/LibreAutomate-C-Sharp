 outw win("test" "")
 outw GetWindow(win("test" "") GW_OWNER)
 outw GetLastActivePopup(win("test" ""))

 act win("book2" "")

 spe
Q &q
act win("Book3" "")
 act win("Book3" "") 1
Q &qq
outq
