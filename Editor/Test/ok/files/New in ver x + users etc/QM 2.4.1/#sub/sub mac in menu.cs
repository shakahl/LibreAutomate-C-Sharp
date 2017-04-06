#region
simple :out 1
call :sub.Test(5)
mac :mac "sub.Test"
mac2 :out mac("sub.Test" "" 7)
mac sub.no :mac "sub.no" ;;if "sub.no" is a macro, runs it, else RT error
sub mac in menu :mac "sub mac in menu"
#endregion

#sub Test
function# a
out a
ret a

#ret
call :sub.Test(5)
mac :mac sub.Test
mac2 :out mac(sub.Test "" 7)
