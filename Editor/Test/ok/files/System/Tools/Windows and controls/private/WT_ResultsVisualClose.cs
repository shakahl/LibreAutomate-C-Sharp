 /
function#

 Closes visual text capturing results window (WT_ResultsVisual).
 Returns number of closed windows.


ARRAY(int) a; int i
win("WT_ResultsDlg" "#32770" "" 0 0 0 a)
for(i 0 a.len) clo a[i]; err
if(i) 0.1
ret i
