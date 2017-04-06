PythonExec ""

 If using Python 2.x, eg from ActiveState, replace MessageBoxW with MessageBoxA.


#ret
import ctypes
MessageBox = ctypes.windll.user32.MessageBoxW
MessageBox(None, 'Hello', 'Window title', 0)
