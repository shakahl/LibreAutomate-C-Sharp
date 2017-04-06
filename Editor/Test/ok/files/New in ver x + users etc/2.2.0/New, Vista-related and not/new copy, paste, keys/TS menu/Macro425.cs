out
0.01
str s="ZONE5"
 keybd_event 16 0 0 0
int i
for i 0 s.len
	keybd_event s[i] 0 0 0
	Sleep 0
	keybd_event s[i] 0 2 0
	Sleep 0
 keybd_event 16 0 2 0


 typelib AUTOIT {F8937E53-D444-4E71-9275-35B64210CC3B} 1.0
  typelib AUTOIT "C:\Program Files\AutoIt3\AutoItX\AutoItX3.dll"
 AUTOIT.AutoItX3 ai._create
 
  ai.Opt("SendKeyDelay" 0)
  ai.Opt("SendKeyDownDelay" 0)
 ai.Send("zone5" 0)

 act
 str s.getmacro("TB Main")
 ai.Send(s 0)

0.3
key " "
0.1
