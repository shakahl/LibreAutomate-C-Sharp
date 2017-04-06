 /
function# nbAll nbRead hDlg

if(!IsWindow(hDlg)) ret 1 ;;Cancel
str s.format("%i KB / %i KB" nbRead/1024 nbAll/1024); s.setwintext(id(5 hDlg))
SendMessage id(3 hDlg) PBM_SETPOS nbRead*100L/nbAll 0
