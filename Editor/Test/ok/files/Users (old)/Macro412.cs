out
str s
 IntPost("http://www.quickmacros.com/form2.php" "" s "My-header: aaa")
IntPost("http://www.quickmacros.com/form2.php" "" s "My-header: aaa[]My-header2: bbb")
 IntPost("http://user:password@www.quickmacros.com/form2.php" "" s "My-header: aaa[]My-header2: bbb")
out s
