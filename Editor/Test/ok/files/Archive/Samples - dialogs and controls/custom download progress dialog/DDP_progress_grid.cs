function# nbAll nbRead str&s hDlg

if(!IsWindowVisible(hDlg)) ret 1 ;;dialog closed?

DlgGrid g.Init(hDlg 4)
if nbRead=nbAll
	g.CellSet(0 1 "downloaded")
else
	int p=iif(nbAll>0 MulDiv(nbRead 100 nbAll) 0)
	g.CellSet(0 1 F"{p} %")
