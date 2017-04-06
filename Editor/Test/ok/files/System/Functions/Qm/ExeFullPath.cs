 /
function~

 Gets full path of this exe file.
 If used .qmm file, gets qmmacro.exe path.
 If you need only path without filename, instead use special variable _qmdir, or str s.expandpath("$qm$\").

 Added in: QM 2.3.3.


BSTR b.alloc(300)
GetModuleFileNameW(0 b 300)
_s.ansi(b)
_s.dospath(_s 1)
_s.lcase
ret _s
