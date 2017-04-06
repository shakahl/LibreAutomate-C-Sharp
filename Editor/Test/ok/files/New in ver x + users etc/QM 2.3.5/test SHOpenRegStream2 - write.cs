__Stream x.is=SHOpenRegStream2(HKEY_CURRENT_USER "Software\GinDi\QM2\User\qmshex" "test" STGM_WRITE)
out x.is
x.is.Write(L"test" 10 &_i)
out _i
