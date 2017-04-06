 Creates cer and pvk (for testing only).

out
ChDir "$program files$\SDK\Bin"
del- "MyPrivateKey.pvk"; err
del- "MyPublicKey.cer"; err

str cl=
 -sv MyPrivateKey.pvk -n "CN=MySoftwareCompany" MyPublicKey.cer
out RunConsole("makecert.exe" cl)
