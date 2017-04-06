WaitPixelColor(0 0xFF0000 388 60)
 wait 0 C 0xFF0000 388 60

 0 CU IDC_NO
 0 CU IDC_HAND
 0 CU IDC_UPARROW
 0.1
 5
 wait 0 win("Wait" "#32770")
 wait 4 WN ""
 wait 0 P
 int+ g_wait=0
 wait 0 CU IDC_ARROW
 int g=wait(0 V g_wait)
 int m=wait(0 M)
