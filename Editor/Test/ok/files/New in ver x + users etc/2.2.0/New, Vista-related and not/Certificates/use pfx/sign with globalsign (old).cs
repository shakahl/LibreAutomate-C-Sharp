 Signs a file.

out
ChDir "$qm$"

str cl=
 sign /f "Q:\Code Signing\GlobalSign\globalsign.pfx" /p slapta1 /t http://time.certum.pl /v qmcl.exe
out RunConsole("signtool.exe" cl)
 sign /f MyPfx.pfx /p your_password /t http://timestamp.verisign.com/scripts/timestamp.dll /v qmcl.exe
