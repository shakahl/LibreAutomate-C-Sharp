function#

 Gets the exit code of the process started by Exec.
 Returns the exit code.
 Returns STILL_ACTIVE (259) if the process is still running.


int ec
if(!GetExitCodeProcess(_hProcess &ec)) end "" 16
ret ec
