 out internet.IntGoOnline(1)

 str s
 internet.IntGetFile("http://www.quickmacros.com/index.html" s)
 out s

 out internet.IntIsConnected ;;needs dialup connection

 out internet.IntPing("http://www.google.com")
 out internet.IntPing("http://www.quickmacros.com/index.html")

 internet.SendMail etc tested

 AddTrayIcon
 str s="a.txt"
 FtpUpload "" "ftp.quickmacros.com" "quickmac" "*" s "$desktop$" "public_html" _hwndqm

 SpamFilter tested

 internet.