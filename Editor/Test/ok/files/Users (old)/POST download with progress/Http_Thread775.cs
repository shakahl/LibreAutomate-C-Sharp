 \
function *pdlg
int dlg=ShowDialog("Http.GetUrl" 0 0 0 1)
opt waitmsg 1
*pdlg=dlg
wait 0 WD dlg
