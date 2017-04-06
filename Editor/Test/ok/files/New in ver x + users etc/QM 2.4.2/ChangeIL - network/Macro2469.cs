 out StartProcess(15 "\\gintaras\q\app\Macro2468.exe")
 out StartProcess(15 "$system$\notepad.exe")
 out StartProcess(15 "$system$\cmd.exe" "/c start \\gintaras\q\app\Macro2468.exe")
 out StartProcess(15 "$my qm$\Function298.exe")

 system "start \\gintaras\q\app\Macro2468.exe"

 run "$system$\cmd.exe" "/c start \\gintaras\q\app\Macro2468.exe"

dll "qm.exe" !__StartProcessFromService integrity $path [$cl]

 out __StartProcessFromService(2|0x100 "cmd.exe" "/c ''''\\gintaras\q\app\Macro24 68.exe'' /coo ''moo''''")
out __StartProcessFromService(2|0x100 "cmd.exe" "/c start '''' /W ''\\gintaras\q\app\Macro24 68.exe'' /coo ''moo''")

 RunConsole2 "start.exe \\gintaras\q\app\Macro2468.exe"
 out RunConsole2("cmd.exe /c ''''\\gintaras\q\app\Macro2468.exe'' /coo ''moo''''")

 start \\gintaras\q\app\Macro2468.exe
 start "\\gintaras\q\app\Macro2468.exe"
 start \\gintaras\q\app\Macro24 68.exe
 start "" "\\gintaras\q\app\Macro24 68.exe"