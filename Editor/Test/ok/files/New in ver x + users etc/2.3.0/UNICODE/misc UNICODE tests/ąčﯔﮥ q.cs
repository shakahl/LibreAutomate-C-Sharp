 run "notepad.exe" "" "" "" 0x10000
 run "notepad.exe"
 run "$desktop$\ąčﯔﮥ q.exe" "" "" "" 0x10000
 run "$desktop$\ąčﯔﮥ q.exe"
 run "$desktop$\test - Ϡ.txt"
 run "$desktop$\ąčﯔﮥ q.exe" "$desktop$\test - Ϡ.txt" "open"
 StartProcess 1 "$desktop$\ąčﯔﮥ q.exe" "$desktop$\test - Ϡ.txt" "$desktop$"
 StartProcess 4 "$desktop$\ąčﯔﮥ q.exe" "$desktop$\test - Ϡ.txt" "$desktop$"

run "$desktop$\ąčﯔﮥ q.exe - Shortcut.lnk"
