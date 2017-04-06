out
ARRAY(str) names urls
int selectedTab=FirefoxGetTabs(0 names urls)
out selectedTab
int i
for i 0 names.len
	out "--------[]%s[]%s" names[i] urls[i]
