function# nbAll nbRead str&s fparam

if(!IsWindowVisible(fparam)) ret 1 ;;dialog closed?
SendMessage fparam PBM_SETPOS iif(nbAll>0 MulDiv(nbRead 100 nbAll) 0) 0
