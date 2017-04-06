	DW save, FF refresh :mac "DW save and run current doc" "" 0 "<refresh>" * firefox.exe
	-
	run in Firefox :mac "DW save and run current doc" * firefox.exe
	run in IE :mac "DW save and run current doc" "" 1 * iexplore.exe
	run in Chrome :mac "DW save and run current doc" "" 2 * "C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"
 	run in Opera :mac "DW save and run current doc" "" 3 * "$pf$\Opera\opera.exe"
Macros :mac "DW menu"
 	run test2 in Firefox :mac "DW save and run current doc" "" 0 "Q:\app\web\test\test2.html" * firefox.exe
