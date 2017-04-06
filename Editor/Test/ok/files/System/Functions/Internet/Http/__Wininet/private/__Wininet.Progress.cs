function# sizeAll sizeSent str*sp

if(m_hdlg and !IsWindow(m_hdlg)) goto gc

 not too frequent
int-- tp
if(GetTickCount-tp<100 and sizeSent and sizeSent!=sizeAll) ret 1

if(m_hdlg) SendNotifyMessage(m_hdlg WM_APP sizeAll sizeSent) ;;info: if m_hdlg, we are in thread

if(m_fa and sp and call(m_fa sizeAll sizeSent sp m_fparam)) goto gc

tp=GetTickCount
ret 1
 gc
lasterror="cancel"
