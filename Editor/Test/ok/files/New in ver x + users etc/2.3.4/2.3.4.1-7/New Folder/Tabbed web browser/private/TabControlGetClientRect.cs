 /
function htb hDlg RECT&r

GetClientRect htb &r
SendMessage htb TCM_ADJUSTRECT 0 &r
MapWindowPoints htb hDlg +&r 2
