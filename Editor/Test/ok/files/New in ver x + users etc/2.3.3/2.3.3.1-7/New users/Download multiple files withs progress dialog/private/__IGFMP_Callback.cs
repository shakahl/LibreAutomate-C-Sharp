 /
function# nbAll nbRead str&s __IGFMP_CB&c

if(!IsWindowVisible(c.hDlg)) ret 1 ;;Cancel

if(nbRead=0) _s.format("File %i KB" nbAll/1024); _s.setwintext(id(8 c.hDlg))
SendMessage id(4 c.hDlg) PBM_SETPOS MulDiv(nbRead 100 nbAll) 0

if c.sizeAll and nbRead
	SendMessage id(6 c.hDlg) PBM_SETPOS MulDiv(c.sizeRead+nbRead 100 c.sizeAll) 0
	if(nbRead=nbAll) c.sizeRead+nbRead
