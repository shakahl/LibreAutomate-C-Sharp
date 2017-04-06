 /
function# iid FILTER&f

 Allows the macro to run if Shift is pressed or CapsLock is toggled, but not both.
 That is, if an alpha key would type in upper case.

int shift capslock
ifk(S) shift=1
ifk(K 1) capslock=1
if(shift^capslock) ret iid
