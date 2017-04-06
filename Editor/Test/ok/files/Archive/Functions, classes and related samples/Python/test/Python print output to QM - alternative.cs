out
PythonExec ""


#ret

def Script():
	print("test")
	print("") # empty line
	print("one\ntwo") # multiline
	print(2) # number


# ---------------------------------------------------------------------------------
# Put your Python script in the Script() function. Don't need to change other code.

import sys
import ctypes
import os
import win32gui

class PrintRedirector(object):
	def __init__(self):
		self.hwndQM=win32gui.FindWindow("QM_Editor", None)
	
	def write(self, message):
		if(message=='\n'): return
		ctypes.windll.user32.SendMessageW(self.hwndQM, 12, -1, message)

sys.stdout = PrintRedirector()
try: Script()
finally: sys.stdout = sys.__stdout__
