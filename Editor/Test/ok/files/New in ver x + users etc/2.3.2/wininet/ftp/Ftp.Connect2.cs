function# $server [$username] [$password] [port] [onerrorcontinue] [flags] ;;flags: 1 passive

 Connects to FTP server.
 Returns connection handle.
 Error if failed.
 Call this function before calling other functions of Ftp class.

 server - server, like "ftp.xxx.com".
 username - user name.
 password - password. Can be encrypted.
 port - FTP port. Default: 21.
 onerrorcontinue - if nonzero, on failure returns 0 and does not throw error.
 flags (QM 2.3.2):
   1 - passive FTP. Use to avoid firewall problems.


Disconnect
str pw.decrypt(16 password "86dbos00~'@n")
m_hi=InternetConnectW(m_hitop @server iif(port port INTERNET_DEFAULT_FTP_PORT) @username @pw INTERNET_SERVICE_FTP iif(flags&1 INTERNET_FLAG_PASSIVE 0) 0)
if(m_hi) ret m_hi
Error(1)
if(!onerrorcontinue) end _s.from(ES_FAILED "  " lasterror)
