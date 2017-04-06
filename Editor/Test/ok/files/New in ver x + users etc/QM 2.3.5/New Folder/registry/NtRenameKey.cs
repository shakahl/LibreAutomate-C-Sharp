 rset _i "test" "\test"

dll ntdll #NtRenameKey KeyHandle UNICODE_STRING*NewName
RegKey k.Open("Software\GinDi\QM2\User\test")
BSTR b="renamed"
UNICODE_STRING s.Buffer=b.pstr
s.Length=b.len*2; s.MaximumLength=0
int hr=NtRenameKey(k &s)
if(hr) outx hr; out _s.dllerror("" "ntdll.dll" hr)
