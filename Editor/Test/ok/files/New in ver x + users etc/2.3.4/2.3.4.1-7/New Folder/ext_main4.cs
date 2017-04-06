out
str sf="$qm$\Macro18-.exe"; cop- "$qm$\Macro18.exe" sf
 goto gSend
str s.getfile(sf)

int* p


exe_erase s 0x12c 0x134


s.setfile(sf)
 gSend
run sf
act "Process Explorer"
4
clo "QM Message"
act _hwndqm
