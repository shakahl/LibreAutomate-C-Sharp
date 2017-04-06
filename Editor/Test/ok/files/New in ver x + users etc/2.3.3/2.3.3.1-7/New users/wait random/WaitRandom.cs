 /
function^ ^_min ^_max

 Waits random time.
 Returns the time.

 _min - min time.
 _max - max time.

 EXAMPLE
 WaitRandom 0.1 5


opt waitmsg -1
_max-_min
double t=RandomNumber*_max+_min
wait t
ret t
