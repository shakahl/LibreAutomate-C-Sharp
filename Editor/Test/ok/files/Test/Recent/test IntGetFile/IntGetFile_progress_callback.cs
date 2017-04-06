function# nbAll nbRead str&s fparam

if(!IsWindow(fparam)) ret 1 ;;Cancel
def PBM_SETPOS (WM_USER+2)
SendMessage id(3 fparam) PBM_SETPOS nbRead*100L/nbAll 0
