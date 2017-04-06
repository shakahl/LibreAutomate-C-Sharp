str s.all(1000 2)
out NtQueryInformationProcess(GetCurrentProcess 27 s 1000 &_i)
out _i
UNICODE_STRING* u=s
 out u.Length
str sp.ansi(u.Buffer)
out sp

int d=GetLogicalDrives
int i
for i 0 32
	if d&1
		str sd.format("%c:" 'A'+i)
		str ss.all(10000 2)
		QueryDosDevice(sd ss ss.len) ;;fails if lpDeviceName 0
		ss.fix
		out ss
		if(sp.begi(ss) and sp[ss.len]='\')
			sp.replace(sd 0 ss.len)
			out sp
			break
	d>>1

