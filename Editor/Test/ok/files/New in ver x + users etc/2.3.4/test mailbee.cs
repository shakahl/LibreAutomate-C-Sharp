 /exe
ExeEnableMailFunctions
err mes- "Failed to register MailBee component" "Test Mailbee" "x"

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
SendMail "support@quickmacros.com" "test" "test" 0 "" "" "" "" "" _s
err mes- "failed" "" "x"
mes "OK" "" "i"

 BEGIN PROJECT
 main_function  test mailbee
 exe_file  $my qm$\test mailbee.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {F381BDCC-5673-4436-930D-6818348CFC4F}
 END PROJECT



 __UseComUnregistered_CreateManifest "mailbee.dll"
