 outw GetShellWindow; ret

 out
ArrangeWindows 0; 2; ArrangeWindows 0
 ArrangeWindows 1; 2; ArrangeWindows 2
 outw win
 BringWindowToTop win
 if(!min(win)) Zorder win
 ArrangeWindows 3
 ArrangeWindows 4
 ArrangeWindows 5
 ArrangeWindows 8
 ArrangeWindows 6 "Quick Macros[]Document Explorer" "test"
 5
 ArrangeWindows 7 "" "test"

