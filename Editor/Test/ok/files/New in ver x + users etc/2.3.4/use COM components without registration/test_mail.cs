 /exe 1
_s=
 smtp_server mail.quickmacros.com
 smtp_port 25
 smtp_user support@quickmacros.com
 smtp_password 2A16EFC757EF5389C86FE3F3B1FCE48E07
 smtp_auth 2
 smtp_secure 0
 smtp_timeout 60
 smtp_email support@quickmacros.com
 smtp_displayname support@quickmacros.com
 smtp_replyto support@quickmacros.com
SendMail "support@quickmacros.com" "test" "vvvvvvvvvvvvvvv" 0 "" "" "" "" "" _s

 note: gmail fails if Avast on
_s=
 smtp_server smtp.gmail.com
 smtp_port 465
 smtp_user qmgindi@gmail.com
 smtp_password 3318B2C945B213CB582D8C1FF577859A05
 smtp_auth 2
 smtp_secure 1
 smtp_timeout 30
 smtp_email qmgindi@gmail.com
 smtp_displayname "Gintaras Didzgalvis"
 smtp_replyto ""
 SendMail "support@quickmacros.com" "test" "$desktop$\test\test.txt" 2 "" "" "" "" "" _s





 BEGIN PROJECT
 main_function  test_mail
 exe_file  $my qm$\test_mail.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {D4803FBE-E3CE-4778-BCE5-766BB131731B}
 END PROJECT
