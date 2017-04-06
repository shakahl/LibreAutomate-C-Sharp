/exe
 IntGetFile "http://www.quickmacros.com/quickmac.exe" _s 0 0 1
 out _s.len

 out
 HtmlDoc d.SetOptions(3)
 d.InitFromWeb("http://www.quickmacros.com/index.html")
 out d.GetText


 _s.all(100000 2 'a')
  SendMail "support@quickmacros.com" "sub" _s 0x100
 SendMail "support@quickmacros.com" "sub" _s 0x100 "" "" "" "" 0 "qmgindi@gmail.com"


 MailSetup

 MailMessage "support@quickmacros.com" "" "" "sub" "booooo" "$my qm$\Copy.gif" 0


 BEGIN PROJECT
 main_function  Macro2290
 exe_file  $my qm$\Macro2290.qmm
 flags  6
 guid  {3F362408-73BB-4D97-AFD8-611D528B4048}
 END PROJECT
