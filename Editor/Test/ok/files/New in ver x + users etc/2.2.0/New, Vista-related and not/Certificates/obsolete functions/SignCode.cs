 /
function# str'exe [flags] ;;exe must be in $qm$ or path relative to $qm$;  flags: 1 no message box on success

ChDir "$qm$"

 GlobalSign does not offer timestamping, so use one of the following
str ts="http://timestamp.verisign.com/scripts/timstamp.dll" ;;sometimes slow
 str ts="http://time.certum.pl"

str sf
if(exe.endi(".sys")) sf="sign /v /ac MSCV-GlobalSign.cer /n ''Gintaras Didzgalvis'' /t ''%s'' %s" ;;MS cross-sertificate needed for drivers
else sf="sign /v /n ''Gintaras Didzgalvis'' /r ''GlobalSign Root CA'' /t ''%s'' %s"

str cl.format(sf ts exe)

str sout
int rc=RunConsole("signtool.exe" cl sout)
if(!rc and flags&1) ret 1
mes "%s[]Return code: %i" "" "i" sout rc
ret !rc

 Using pfx file it would be (but does not work for driver):
 cl.format("sign%s /f globalsign.pfx /p slapta1 /t ''%s'' /v %s" ac ts exe)


 For Certum:
  /
 function $exe ;;exe must be in $qm$ or path relative to $qm$
 
 ChDir "$qm$"
 
 str cl.format("sign /f GD.pfx /p 49di76va /t http://time.certum.pl /v %s" exe)
 str sout
 int rc=RunConsole("signtool.exe" cl sout)
 mes "%s[]Return code: %i" "" "i" sout rc
