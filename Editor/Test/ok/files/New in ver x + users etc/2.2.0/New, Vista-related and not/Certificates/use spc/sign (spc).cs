 Signs a file.

out
cop- "$qm$\qmcl.exe" "$program files$\SDK\Bin"
ChDir "$program files$\SDK\Bin"

str cl=
 sign /csp MySPC.spc /k MyPrivateKey.pvk /t http://timestamp.verisign.com/scripts/timestamp.dll /v qmcl.exe
out RunConsole("signtool.exe" cl)

 Does not work. Works when using wizard.
