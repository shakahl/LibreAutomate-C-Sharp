 /
function# $computer str&ip [flags] ;;flags: 1 get all

 Gets IP address of a computer.
 Returns: 1 success, 0 failed.

 computer - computer name. It can be:
    Local computer. Use "" as name.
    Network computer name.
    Internet server address, like "www.quickmacros.com".
    Can be like computer:port. Then IP will be like ip:port.
 ip - receives IP address string.
 flags:
   1 - get list of IP addresses of the computer.

 Added in: QM 2.3.0.

 EXAMPLE
 str ip
 if(GetIpAddress("PC-3" ip))
	 out ip


if(!InitWindowsDll(0)) ret
ip=""

 if computer:port, separate, and later append port to IP
str cn port
int nt=tok(computer &cn 2 ":")
if(nt>1) computer=cn

if(empty(computer)) computer=0
else _s.unicode(computer); computer=_s.ansi(_s CP_ACP) ;;CP_UTF8->CP_ACP. Windows does not allow to use Unicode in computer name, but can be nonascii. Cannot use GetAddrInfoW because it is not on all OS.
hostent* hp = gethostbyname(computer); if(!hp) ret
int i
for i 0 1000
	if(hp.h_addr_list[i] && hp.h_length>=4) else break
	byte* b=hp.h_addr_list[i]
	int* ipi=b; if(*ipi=0x100007F) continue ;;"127.0.0.1"
	if(i) ip+"[]"
	ip.formata("%u.%u.%u.%u" b[0] b[1] b[2] b[3])
	if(nt>1) ip.formata(":%s" port)
	if(flags&1=0) break
ret ip.len!=0
