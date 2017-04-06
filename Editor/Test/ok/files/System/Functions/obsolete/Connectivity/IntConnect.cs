 /
function# [$connection] [flags] [nredial] [$username] [$password] [$phonenumber] ;;flags: 1 no dialog, 2 no error.

 Dials a dial-up networking connection.
 If connection is not specified - default connection.
 Returns: 1 successfully connected, -1 was connected, 0 (if flag 2 is set) failed.
 If fails, throws error, unless flag 2 is set.
 Does not support Unicode.
 Alternatively, use IntDial, which shows standard dialog.

 Note: connect/disconnect/check connection functions don't work if
 you connect to the Internet not through Windows Dial-Up Networking.

 Password can be encrypted (Options -> Security) for RasConnect.


str s=connection; if(!s.len) RasGetDefConn(s)

if(IntIsConnected(s)) ret -1
 dial
int r
if(flags&1)
	r=RasConnect(s phonenumber username password)
else
	type ___RASDIALQM ~controls ~e4Use ~e5Pas ~e6Num ~e7Con
	___RASDIALQM d.controls="4 5 6 7"
	d.e7Con=s
	d.e6Num=phonenumber
	d.e4Use=username
	d.e5Pas=password
	r=ShowDialog("IntConnectDlg" &IntConnectDlg &d)

if(!r) ret 1
if(nredial>0 and r!=2) nredial-1; opt waitmsg -1; 5; goto dial
if(flags&2=0) RasError(r _s.format("cannot connect to %s" s))
