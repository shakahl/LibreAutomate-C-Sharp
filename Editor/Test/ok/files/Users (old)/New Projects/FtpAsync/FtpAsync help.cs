class __WininetAsync -m_hitop -m_hi ~lasterror
class FtpAsync :__WininetAsync -__FTP_FIND_DATA*m_fd

out

FtpAsync f.Connect("ftp.quickmacros.com" "quickmac" "*" 0 0 1)
1
f.DirSet("public_html/test")
str ss
1

out f.FileGet("test.txt" "$temp$\test.txt")
 out f.FilePut("$qm$\sqlite.txt" "test.txt")
 out f.FilePut("$qm$\winapi2.txt" "test.txt")
1
