 /
function! hwb


RECT r; GetClientRect hwb &r

int hDoc
Htm el=htm("HTML" "" "" hwb)
el.Location(0 0 0 hDoc)
out "%i %i" r.bottom hDoc

ret hDoc<r.bottom-10
