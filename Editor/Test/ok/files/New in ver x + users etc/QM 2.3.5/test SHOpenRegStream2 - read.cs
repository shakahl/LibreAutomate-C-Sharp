out
BSTR b.alloc(4)
Q &q
__Stream x.is=SHOpenRegStream2(HKEY_CURRENT_USER "Software\GinDi\QM2\User\qmshex" "test" STGM_READ)
Q &qq
out x.GetSize
 x.is.Stat
x.is.Read(b.pstr 10 &_i)
Q &qqq
outq
out _i
out b
