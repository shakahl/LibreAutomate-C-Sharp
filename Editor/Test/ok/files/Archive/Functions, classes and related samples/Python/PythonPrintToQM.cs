 Redirects Python print() function output to QM output.

 REMARKS
 print() will be redirected in all Python code added by PythonExec or PythonAddCode that are called after calling this function in current thread.

 EXAMPLE
 PythonPrintToQM
 PythonExec ""
 
 #ret
 print("test")
 print("") # empty line
 print("one\ntwo") # multiline
 print(2) # number
 print("list", 1, 2, 3)
 print("list2", 1, 2, 3, sep=", ")


PythonAddCode ""

#ret
import sys
import ctypes
import win32gui

def printQM(*a, sep = ' '):
	s = ""
	for v in a:
		if(s != ""): s += sep
		s += str(v)
	
	if(printQM.hwndQM == 0): printQM.hwndQM=win32gui.FindWindow("QM_Editor", None)
	
	ctypes.windll.user32.SendMessageW(printQM.hwndQM, 12, -1, s)
printQM.hwndQM = 0

print = printQM
