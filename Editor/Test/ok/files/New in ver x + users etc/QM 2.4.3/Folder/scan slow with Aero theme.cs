PerfFirst
rep 100
	PixelSearch(0 0 600 400 RandomInt(0 0xffffff))
PerfNext;PerfOut


 Single loop execution time:
   Windows 10: 18 ms
   Windows 7: 20 ms
   Windows 7 without Aero theme: 1.6 ms
   Windows 8.1: 20 ms
   Windows XP: 1.4 ms

 Without an Aero theme >10 times faster. But I don't know how to disable it on Windows 8.1 and 10. It seems impossible.
 Cannot make it faster. The speed depends on Windows, ie how slowly it gives screen pixels. Then the searching part is much faster.
