int w=win("Firefox" "Mozilla*WindowClass" "" 0x4)

 out FirefoxGetAddress ;;no http://

 ARRAY(str) au at; ARRAY(Acc) ao
 FirefoxGetLinks w au at ao
  out au

 FirefoxWait "http://www.yahoo.com"
 act w; key F5
 FirefoxWait ""

ARRAY(str) names urls; ARRAY(Acc) aod aob
 int selectedTab=FirefoxGetTabs(0 names urls)
int selectedTab=FirefoxGetTabs(0 names urls);; aod aob)
out selectedTab
int i
for i 0 names.len
	out "--------[]%s[]%s" names[i] urls[i]
	 out aod[i].Name
	 out aob[i].Name

 FirefoxSelectTab 0 "LHMT*" 2
