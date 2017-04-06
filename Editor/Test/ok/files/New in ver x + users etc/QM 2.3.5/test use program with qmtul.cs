 /exe
ifa("+QM_Editor") ret

 mou 1 1
 1
 key "test"
 if(!empty(_command)) key V (_command)
 1
 key B(#50)
 1
 if(inp(_s "password")) key (_s) Y
 else ret 4

 ret -8

#if 0
str s1 s2 s3
outw win "" s1
outw child "" s2
outw child("" "Edit") "" s3
LogFile F"{s1}[]{s2}[]{s3}" 0 "qmtul.log"

key "p" Y
#else

 str password="p"
 AutoPassword "" password 2
 err
	 LogFile "AutoPassword failed" 0 "qmtul.log"
	 key Y
	 2
	 AutoPassword "" password 2
	 err
		 LogFile "AutoPassword failed again" 0 "qmtul.log"
		 ret 1 ;;error
 0.5
 key Y
  info: AutoPassword works regardless of what control is focused. If it fails, there isn't a password field. Then this macro presses Enter and tries AutoPassword again.

Acc a.Find(win "TEXT" "" "class=Edit" 0x1005) ;;find password field
err
	key Y
	2
	a.Find(win "TEXT" "" "class=Edit" 0x1005)
	err
		LogFile "no password field" 0 "qmtul.log"
		ret 1 ;;error
act child(a) ;;set focus.  a.Select(1) does not work, that is why AutoPassword too.
key "password" Y

 note: out will not work. Use LogFile.

 int h=child ;;focused control
 if !h or !wintest(h "" "Edit")
	 
 Acc a.FromFocus
 LogFile a.Name 0 "qmtul.log"

 key "p" Y


 BEGIN PROJECT
 main_function  test use exe with qmtul
 exe_file  $qm$\unlock.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {242A0A86-0D38-429E-99B2-188CBBDC177B}
 END PROJECT
