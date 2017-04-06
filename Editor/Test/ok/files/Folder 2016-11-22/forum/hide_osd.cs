 /
function# iid FILTER&f

if(!wintest(f.hwnd "QM time email etc" "QM_OSD_Class")) ret -2

mac "sub.Move" "" f.hwnd
ret -1

err+


#sub Move
function hwnd

 outw hwnd

hid hwnd
3
hid- hwnd

err+
