 Creates pfx (joins cer and pvk into single file).

out
ChDir "$program files$\SDK\Bin"
del- "MyPFX.pfx"; err

str cl=
 -pvk MyPrivateKey.pvk -spc MyPublicKey.cer -pfx MyPFX.pfx -po your_password
out RunConsole("pvk2pfx.exe" cl)
