 run "qmcl.exe" "M ''ttt'' C(,,,) c1,,,c2,,,"
 spe
ClearOutput
 run "qmcl.exe" "M ''ttt'' C ( ) lpstr str -1 300 -1 5000000000 5.78"
 run "qmcl.exe" "M ''ttt'' A ''lpstr 1'' ''str 2'' -1 300 -1 5000000000 5.78"
 run "qmcl.exe" "M ''ttt'' A ''lpstr 1'' ''str 2'' 0x4U+5L 300 -1 5000000000 5.78"
run "qmcl.exe" "M ''ttt'' A( : ) lpstr 1 : str 1 : -1 : 300 : -1 : 5000000000 : 5.78"
 run "qmcl.exe" "M ''ttt'' A( : ) lpstr 1 : str 1 : WM_USER + 1 + iii : 300 : -1 : 5000000000 : 5.78"
 run "qmcl.exe" "M ''ttt'' ''cmd''"
 mac "ttt" "lpstr"
 mac "ttt" "" "lpstr" "str" 1 2 3 5000000000000 5.3
 mac "Toolbar" "Notepad"
 SendMessage win 12 2 "M ''ttt'' A ''lpstr 1'' ''str 2'' 0x4U+5L 300 -1 5000000000 5.78"
