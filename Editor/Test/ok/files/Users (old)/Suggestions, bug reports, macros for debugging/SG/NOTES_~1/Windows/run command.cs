 Inactive/hide options in run command
 ====================================
int flag=16
 run "C:\Windows\Metapad.exe" "hello" "" "" flag
 wait 1
 hid- "hello"
run "C:\Windows\Notepad.exe" "" "" "" flag
wait 1
hid- "Notepad"

 bug:
 flag=4  - wont make window inactive
 flag=16 - with metapad: after hid-, shows greyed-out edit window
 		 - with notepad: shows currupted display after unhide


 metapad - free notepad replacement - can be had from http://www.liquidninja.com/metapad/
 (Ive just put it in windows directory, and copied it to notepad.exe, replacing the old notepad)