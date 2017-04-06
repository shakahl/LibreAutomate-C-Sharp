OnScreenDisplay "Run Notepad" 1 0 0 0 0 0 2
run "notepad.exe"
1
int w1=win(" - Notepad" "Notepad") ;;get window handle
OnScreenDisplay "Activate next window" 1 0 0 0 0 0 2
act
1
OnScreenDisplay "Activate Notepad" 1 0 0 0 0 0 2
act w1
1
OnScreenDisplay "Close" 1 0 0 0 0 0 2
clo w1

 Tip: Press the A button to show/hide annotations.
