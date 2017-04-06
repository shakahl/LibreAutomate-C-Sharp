 Run Notepad
run "notepad.exe"
 Wait for Notepad window max 5 s
wait 5 WA "Notepad"
 Wait 0.5 s
0.5
 Keys (type text, then press Enter)
key "This is simple text, typed using the keyboard." Y
 Create string variable t
str t
 Show input box (t will receive text)
inp- t "Type something:"
 Paste formatted text
paste "Pasted using the clipboard:[]	''%s''.[]" t ;;here %s is replaced with the value of t;  [] and '' can be used instead of newline and " in any string
 Show message box with buttons Yes and No
if mes("Delete text?" "" "YN?")='Y'
	 If Yes, press keys Alt+e+a and Alt+e+l
	key A{ea} A{el}
	 Ask to close
	if mes("Close Notepad?" "" "YN?")='Y'
		 Close active window
		clo

 Lines that begin with space (green text) are not executed and can be used for comments or to temporarily disable commands.
