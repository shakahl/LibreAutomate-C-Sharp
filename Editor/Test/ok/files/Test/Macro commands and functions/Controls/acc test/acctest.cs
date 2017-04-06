 Acc a=acc("General" "LINK" "Internet Explorer" "Internet Explorer_Server" "http://www.quickmacros.com/forum/viewforum.php" 0x1001)
 out acctest(a "General" "LINK" "Internet Explorer" "Internet Explorer_Server" "http://www.quickmacros.com/forum/viewforum.php" 0x1001)

 Acc a=acc("General" "LINK" "Internet Explorer" "Internet Explorer_Server" "" 0x1101 283 309)
 out acctest(a "General" "LINK" "Internet Explorer" "Internet Explorer_Server" "" 0x1101 283 309)

 Acc a=acc(654 14 0)
 out acctest(a "Mouse" "PUSHBUTTON" "QM TOOLBAR" "ToolbarWindow32" "" 0x1101)
 out acctest(a 46 0 "QM TOOLBAR" 1)
 out acctest(a 654 14 0)
