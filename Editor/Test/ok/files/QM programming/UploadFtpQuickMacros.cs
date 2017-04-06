if(!matchw(_qmdir "q:\app\" 1)) end "QM must run from Q:\app"

AddTrayIcon "" "Uploading QM files"
str s=
 ..\quickmac.exe>beta/quickmac.exe
 ..\quickmac.exe>dev/quickmac.exe
 ..\quickmac.exe>quickmac.exe
 
 $temp$\test.zip>../sat/test.zip
 $temp$\backupThunderbird.pcv>../sat/backupThunderbird.pcv


FtpUpload "" "ftp.quickmacros.com" "quickmac" "[*1C34A1384097E6080AD0DA8A5B14A32B02*]" s "$qm$\web" "public_html" _hwndqm
