 These functions can be used to execute Python code in QM.

 SETUP

 Download and install Python 32-bit.
 There are multiple Python implementations. I tested the "traditional" implementation (CPython) from www.python.org and ActiveState Python.

 ActiveState Python is easier to install, but it is an old Python version, I found only version 2.7.10.
   Download and install Python 32-bit Windows version from
      http://www.activestate.com/activepython/downloads
   I tested it only on Windows 10.

 To install CPython, need two or more steps:
   1. Download and install Python 32-bit Windows version from
         https://www.python.org/downloads/
      I tested version 3.5.2 on Windows 10 and 7, and version 3.4.3 on Windows 8.1. I recommend version 3.4.3, read below why. I didn't test 64-bit version, it probably would not work with QM.
      IMPORTANT: When installing, check "Add to PATH". After installing, restart QM to update its copy of PATH variable.
   2. Download and install PyWin32 from
         https://sourceforge.net/projects/pywin32/files/pywin32/
      Choose the version that matches your Python version, for example pywin32-220.win32-py3.5.exe for Python 3.5.x or pywin32-220.win32-py3.4.exe for Python 3.4.x.
      IMPORTANT: Run the setup program as administrator.
   Possible problems with Python 3.5.x:
      On Windows 7: error "api-ms-win-crt-runtime-l1-1-0 dll missing".
         I solved this not easily. Tried several ways, and now don't know what was actually needed, but the problem finally disappeared after installing KB2999226: https://support.microsoft.com/en-us/kb/2999226.
         I tried to install all Windows updates, Visual C++ 2015 Redistributable package, KB2999226. Then uninstall/install the Python programs and restart QM.
         Or you can try an older Python version, eg 3.4.3.
      On Windows 8.1: Python 3.5.2 and 3.6.0a2 setup failed. Version 3.4.3 + pywin32-220.win32-py3.4.exe worked without problems.

 TEST

 Run macros in "test" folder.
