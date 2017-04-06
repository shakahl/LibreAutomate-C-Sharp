 /
function# $dest [&TTL] [timeout] [flags] ;;flags: 1 process messages

 Sends an ICMP echo request to an internet or network computer.
 It is called "ping", and is used to see if we can connect to a computer, and how quickly.
 Returns roundtrip time, in milliseconds. If cannot connect, returns 0. Does not throw errors.
 Note that some ICMP packets may be lost. Then the function returns 0. Call it several times to make more reliable. Don't ping too frequently.

 dest - ip (eg "123.456.78.90"), or web server address (eg "www.qweryzxcv.com"), or network computer name, or "" for this computer. The function returns 0 if dest is invalid or cannot be resolved.
 TTL - receives TTL. Default: 0. If don't know what is TTL, Google will tell you.
 timeout - timeout in milliseconds. Default: 1000. The function returns 0 if not connected during that time.
 flags - 1 process Windows messages. Use it when calling this function from a dialog. 

 Unstable:
   Cannot end thread without terminating, even with flag 1.
   After terminating, does not work until QM restarted.
   Also, possible exceptions. Then that URL did not work for some time, even if used ping.exe.

 See also: <IntPing> (it is better if you want to check your Internet connection).

 EXAMPLE
 str dest="www.google.com"
 int t ttl
 t=Ping(dest ttl)
 if(t) out "Ping %s: bytes=32 time=%i TTL=%i" dest t ttl
 else out "Ping %s: failed" dest


if(!timeout) timeout=1000
if(&TTL) TTL=0

def INADDR_NONE 0xffffffff
type IP_OPTION_INFORMATION !Ttl !Tos !Flags !OptionsSize !*OptionsData
type ICMP_ECHO_REPLY Address Status RoundTripTime @DataSize @Reserved !*Data IP_OPTION_INFORMATION'Options
SetEnvVar "icmp_dll" iif(_winver=0x500 "icmp" "iphlpapi")
dll- "%icmp_dll%"
	#IcmpCreateFile
	#IcmpCloseHandle IcmpHandle
	#IcmpSendEcho2 IcmpHandle Event ApcRoutine !*ApcContext #DestinationAddress !*RequestData @RequestSize IP_OPTION_INFORMATION*RequestOptions !*ReplyBuffer ReplySize Timeout

int ipa=inet_addr(dest)
if(ipa=INADDR_NONE)
	if(!GetIpAddress(dest _s)) ret
	ipa=inet_addr(_s)

str sd.RandomString(32 32)
str rb.all(8*sizeof(ICMP_ECHO_REPLY)+sd.len 2 0)
ICMP_ECHO_REPLY& r=rb
__Handle ev
type __IcmpHandle -m_h
__IcmpHandle h=IcmpCreateFile ;;must be after other variables
int ok

if(flags&1)
	ev=CreateEvent(0 0 0 0)
	opt waitmsg 1
	SetLastError 0
	ok=IcmpSendEcho2(h ev 0 0 ipa sd sd.len 0 rb rb.len timeout)
	ok=ok=ERROR_IO_PENDING or (ok=0 and GetLastError=ERROR_IO_PENDING) ;;msdn says IcmpSendEcho2 returns ERROR_IO_PENDING, but actually it returns 0, and GetLastError returns ERROR_IO_PENDING
	if(ok) wait 0 H ev; err ret
else
	ok=IcmpSendEcho2(h 0 0 0 ipa sd sd.len 0 rb rb.len timeout)

if(ok and (!r.Status or r.Status=11000))
	int t=r.RoundTripTime; if(!t) t=1
	if(&TTL) TTL=r.Options.Ttl

ret t
