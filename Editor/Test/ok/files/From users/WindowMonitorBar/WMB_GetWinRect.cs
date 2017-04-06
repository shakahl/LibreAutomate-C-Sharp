
function w1 &RET str&OUTPUT
RECT r
GetWindowRect(w1 &r)
OUTPUT = _s.from("w " r.right-r.left " h " r.bottom-r.top)