 /Excel and anywho.com
function ARRAY(ANYWHORESULT)&ar $lastName $firstName $city $state $_zip

 Finds a person in anywho.com.
 Stores all results in ar.
 anywho.com must be displayed in Internet Explorer. Can be home page or results page.


out "Searching for %s %s..." firstName lastName

ar=0
int w1=win("White Pages on AnyWho - Windows Internet Explorer" "IEFrame")

 find all form elements
Htm elLN=htm("INPUT" "qn" "" w1 0 13 0x221)
Htm elFN=htm("INPUT" "qf" "" w1 0 14 0x221)
Htm elC=htm("INPUT" "qc" "" w1 0 16 0x221)
Htm elS=htm("SELECT" "qs" "" w1 0 2 0x221)
Htm elZ=htm("INPUT" "qz" "" w1 0 17 0x221)
Htm elFIND=htm("INPUT" "btnsubmit" "" win("White Pages on AnyWho - Windows Internet Explorer" "IEFrame") 0 18 0x221)

 set text of form elements
elFN.SetText(firstName)
elLN.SetText(lastName)
elC.SetText(city)
if(empty(state)) state="Select"
elS.CbSelect(state)
elZ.SetText(_zip)

 submit
elFIND.Click

 g1
 wait for results
wait 0 I ;;wait while IE is busy
Htm elDIS=htm("A" "DISCLAIMER" "" w1 0 17 0x21 60) ;;and then wait for DISCLAIMER link

 navigate from DISCLAIMER to the first result table
Acc ad=acc(elDIS)
Acc ax at
ad.Navigate("parent next" at)

 enumerate results
rep
	at.Navigate("first3" ax) ;;table -> name
	if(ax.Role!=ROLE_SYSTEM_LINK) break ;;Found ...
	
	ANYWHORESULT& r=ar[] ;;add new element to the results array
	str s=ax.Name
	 split full name into first and last name
	 Note that not always will be correct, because full name can be eg "John J Doe", and then first name will be "John J".
	int i=findcr(s ' '); if(i>=0) r.firstName.left(s i); r.lastName=s+i+1; else r.lastName=s
	 city, state, zip
	ax.Navigate("next2")
	s=ax.Name
	 out s
	ARRAY(str) as
	findrx(s "(.+), (\w\w) ?(\d+)?" 0 0 as)
	r.city=as[1]
	r.state=as[2]
	r._zip=as[3]
	
	at.Navigate("next"); err break ;;next result table

 more pages?
Htm elNext=htm("A" "Next" "<A class=L6 href=''/results.php*" w1 0 15 0x5)
if(!elNext) ret
 click Next
elNext.Click
 go to the beginning
goto g1
