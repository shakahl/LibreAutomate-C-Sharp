 /
function nCode wParam MSLLHOOKSTRUCT*h

out "%i %i" h.pt.x h.pt.y

ret CallNextHookEx(bimhook nCode wParam h)
