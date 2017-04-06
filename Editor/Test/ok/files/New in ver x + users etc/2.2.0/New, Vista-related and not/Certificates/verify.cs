out
ChDir "$program files$\SDK\Bin"
out RunConsole("signtool.exe" "verify /a /v qmcl.exe" _s)
out _s

