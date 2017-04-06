dll "qm.exe" !__StartProcessFromService integrity $path [$cl]

out __StartProcessFromService(2 "notepad.exe")
 out __StartProcessFromService(2|0x100 "cmd.exe" "/c start '''' /W ''\\gintaras\q\app\Macro24 68.exe'' /coo ''moo''")
