 /
function# [hwndFF] [ARRAY(str)&names] [ARRAY(str)&urls] [ARRAY(Acc)&docObjects] [ARRAY(Acc)&buttonObjects]

 Gets names of Firefox tabs. Optionally gets their URL and accessible objects.
 Returns 0-based index of the selected tab.
 Error if fails.

 hwndFF - Firefox window handle. If 0, finds. Also can be SeaMonkey window handle.
 names - variable for tab names. Can be 0 if don't need.
 urls - variable for addresses. Can be 0 if don't need.
 docObjects - variable for DOCUMENT accessible objects that represent web pages. Can be 0 if don't need.
 buttonObjects - variable for PAGETAB accessible objects that represent tab buttons. Can be 0 if don't need.

 REMARKS
 Requires QM 2.3.3 or later.
 Elements of urls and docObjects of non-loaded tabs will be empty. Firefox does not load pages in invisible tabs at startup.

 EXAMPLE
 ARRAY(str) names urls
 int selectedTab=FirefoxGetTabs(0 names urls)
 out selectedTab
 int i
 for i 0 names.len
	 out "--------[]%s[]%s" names[i] urls[i]


ARRAY(str) _names; if(!&names) &names=_names
names=0
if(&urls) urls=0
if(&docObjects) docObjects=0
if(&buttonObjects) buttonObjects=0

if(!hwndFF) hwndFF=win("Mozilla Firefox" "Mozilla*WindowClass" "" 0x804); if(!hwndFF) end ERR_WINDOW

int i r=-1
FFNode fn
ARRAY(str) ap

 enum tabs
Acc a.Find(hwndFF "PAGETAB" "" "class=MozillaWindowClass" 0x1004 1) ;;note: added 1 s waiting because fails when called first time after launching FF
rep
	if(a.Role!ROLE_SYSTEM_PAGETAB) goto next ;;[ + ] button
	
	names[]=a.Name
	if(&buttonObjects) buttonObjects[]=a
	
	 get selected tab
	if(r<0 and a.State&STATE_SYSTEM_SELECTED) r=i
	
	if &urls
		 get id of associated pane and store in ap. Because the order of tabs and panes may be different.
		fn.FromAcc(a)
		ap[]=fn.Attribute("linkedpanel")
	
	 next
	a.Navigate("n"); err break
	i+1

if(r<0) end ERR_FAILED
if(!&urls and !&docObjects) ret r

 enum panes
if(&urls) urls.create(names.len)
if(&docObjects) docObjects.create(names.len)
#if QMVER<0x2030400
a.Find(hwndFF "DOCUMENT" "" "" 0x3000 0 0 "pa3fi")
err a.Find(hwndFF "" "" "" 0x3010 1 0 "pa4fi")
#else
a.Find(hwndFF "" "" "" 0x3000 2 0 "pa3fi")
#endif
rep
	 get id of this pane, and find in ap
	fn.FromAcc(a); _s=fn.Attribute("id")
	for(i 0 ap.len) if(ap[i]=_s) break
	if(i=names.len) goto next2 ;;should never happen
	 get DOCUMENT object and url
	 Acc aa; a.Navigate("fi2" aa); err goto next2 ;;fails if there is an alert bar, tested in SeaMonkey
	Acc aa.Find(a.a "DOCUMENT" "" "" 0x1010); err goto next2
	if(&urls) urls[i]=aa.Value
	if(&docObjects) docObjects[i]=aa
	
	 next2
	a.Navigate("n"); err break

ret r

err+ end _error
