 Creates spc from cer (for testing only).

out
ChDir "$program files$\SDK\Bin"
del- "MySPC.spc"; err

out RunConsole("cert2spc.exe" "MyPublicKey.cer MySPC.spc")
