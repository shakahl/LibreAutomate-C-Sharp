int t1=timeGetTime
IntPost "http://www.quickmacros.com/form2.php" "a=b&c=d" _s
out timeGetTime-t1
out _s

