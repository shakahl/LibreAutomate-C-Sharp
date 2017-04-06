str ip
 ip="98.17.88.210"
 ip.getclip
Acc a.FromMouse; ip=a.Name

 PF
 IntGetFile "http://www.quickmacros.com/test/test.php" _s
IntGetFile F"http://www.quickmacros.com/test/test.php?IP={ip}" _s
 IntGetFile "http://www.quickmacros.com/support.html" _s
 PN; PO
out _s
OnScreenDisplay _s 1

 out
 str ip c sm.getmacro("Macro2095")
 foreach ip sm
	 IntGetFile F"http://www.quickmacros.com/test/test.php?IP={ip}" c
	 out F"{ip}: {c.trim}"

#ret
  1    <1 ms    <1 ms    <1 ms  ip-122-1.zirzile.lt [84.32.122.1] 
  2     1 ms     1 ms     1 ms  84.15.1.205 
  3     *        3 ms     *     84.15.10.179 
  4    20 ms     3 ms     3 ms  84.15.6.213 
  5    16 ms    16 ms    15 ms  xe-8-1-0.bar1.Stockholm1.Level3.net [213.242.110.209] 
  6   153 ms   155 ms   153 ms  ae-111-3501.bar1.Stockholm1.Level3.net [4.69.158.242] 
  7   154 ms   154 ms   154 ms  ae-46-46.ebr3.Frankfurt1.Level3.net [4.69.143.170] 
  8   153 ms   153 ms   154 ms  ae-73-73.csw2.Frankfurt1.Level3.net [4.69.163.6] 
  9   156 ms   156 ms   156 ms  ae-71-71.ebr1.Frankfurt1.Level3.net [4.69.140.5] 
 10   157 ms   156 ms   157 ms  ae-47-47.ebr2.Paris1.Level3.net [4.69.143.142] 
 11   155 ms   164 ms   155 ms  ae-42-42.ebr2.Washington1.Level3.net [4.69.137.54] 
 12   156 ms   159 ms   155 ms  ae-62-62.csw1.Washington1.Level3.net [4.69.134.146] 
 13   153 ms   152 ms   153 ms  ae-61-61.ebr1.Washington1.Level3.net [4.69.134.129] 
 14   153 ms   153 ms   153 ms  ae-2-2.ebr3.Atlanta2.Level3.net [4.69.132.85] 
 15   155 ms   153 ms   156 ms  ae-63-63.ebr1.Atlanta2.Level3.net [4.69.148.242] 
 16   154 ms   155 ms   183 ms  ae-1-8.bar1.Orlando1.Level3.net [4.69.137.149] 
 17   154 ms   154 ms   153 ms  ten-7-4.edge1.level3.mco01.hostdime.com [67.30.140.198] 
 18   155 ms   155 ms   155 ms  ofelia.dizinc.com [72.29.65.132] 

 From QM forum users database, time zone of 5 from 215 users does not match country (detected from IP).
