out
 int h=win("Notepad")
 str s ss
 out s.getwinexe(h)
 out ss.getwinexe(h 1)

 str s
 out GetProcessExename(4104 &s 0)
 out s

 out qm.ProcessNameToId("notepad")
 out qm.ProcessNameToId("ąčﯔﮥ q")

ARRAY(str) a; int i
 qm.EnumProcessesEx(0 &a)
qm.EnumProcessesEx(0 &a 0)
for i 0 a.len
	out a[i]
	