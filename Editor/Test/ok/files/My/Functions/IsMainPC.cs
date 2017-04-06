 /
function#

 Returns 1 if this computer/user is set as main.
 To set/unset it, run macro "Set main PC".


str s="$documents$\Main PC.txt"
ret FileExists(s)
