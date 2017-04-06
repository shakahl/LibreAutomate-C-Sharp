out
 for(int second = 0;; second++)
 {
    if (second >= 60) Assert.Fail("timeout");
    try { if (findrx(selenium.GetTitle(), "^[\s\S]*uuu[\s\S]*$")<0) break; } catch(Exception) {  }
    Thread.Sleep(1000);
 }

 for(_i 0 1000000000) if(_i>=60) end "timeout"; else if(1) break; else 1

 foreach(1 3 sub.Wait) if(GetMod) break
foreach(1 3 sub.Wait) if(!act("uuuuuuu")) break; err


#sub Wait
function ^period ^timeout

double waited
if(waited>=timeout) end "timeout"
opt waitmsg -1
wait period
waited+period
ret 1
