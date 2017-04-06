function $text

 Writes text to the console of the process started by Exec.

 text - text to write. The function does not append new line (Enter).


if(!_hProcess) end ERR_INIT

int n=len(text)
if(!n) ret
if(!WriteFile(_siWrite text n &n 0)) end "" 16
