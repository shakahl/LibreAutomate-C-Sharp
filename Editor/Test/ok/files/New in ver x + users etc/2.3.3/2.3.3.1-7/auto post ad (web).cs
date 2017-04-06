str Title="Test Header"
str Price=6
str Location="Atlanta"
str Text=
 Hello
 Line 2

str urlList=
 https://post.craigslist.org/atl/S/mob/atl
 https://post.craigslist.org/phi/S/mob/none/x
 https://post.craigslist.org/sdo/S/mob/csd
 https://post.craigslist.org/bhm/S/mob/none/x

int w1
str url
foreach url urlList
	web url 0x5 "" "" 0 w1
	 ----
	Htm el
	el=htm("INPUT" "" "" w1 0 2 0x20 3)
	el.SetText(Title)
	el=htm("INPUT" "" "" w1 0 3 0x20)
	el.SetText(Price)
	el=htm("INPUT" "" "" w1 0 4 0x20)
	el.SetText(Location)
	el=htm("TEXTAREA" "" "" w1 0 0 0x20)
	el.SetText(Text)
	