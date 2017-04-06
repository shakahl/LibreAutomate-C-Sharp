 /
function hwndFF $name [flags] ;;flags: 1 name is URL, 2 wildcard, 4 beginning, 8 end, 16 anywhere, 32 rx

 Finds and selects Firefox tab.
 Error if not found.

 hwndFF - Firefox window handle. If 0, finds.
 name - tab name. If flag 1 - URL.
 flags:
   1 - name is URL.
   2-32 - how to compare name. Default: exact match. Always case insensitive.


ARRAY(str) names urls; ARRAY(Acc) a
int selectedTab=FirefoxGetTabs(hwndFF names iif(flags&1 &urls 0) 0 a)
int i
ARRAY(str)& as=iif(flags&1 urls names)
for i 0 names.len
	if SelStr(flags|1 as[i] name)
		if i!=selectedTab
			a[i].DoDefaultAction
		ret 1

end "tab not found"
err+ end _error
