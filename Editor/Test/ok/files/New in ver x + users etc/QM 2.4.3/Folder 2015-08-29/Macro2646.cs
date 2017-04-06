SetTimer _hwndqm 8654 2000 &sub.Timer

#sub Timer
function a b c d
out GetWindowThreadProcessId(_hwndqm 0)
out GetCurrentThreadId
