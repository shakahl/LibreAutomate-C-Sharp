 /exe

_s.all(100000 2 'a')
 SendMail "support@quickmacros.com" "sub" _s 0x100
SendMail "support@quickmacros.com" "sub" _s 0x100 "" "" "" "" 0 "qmgindi@gmail.com"

mes "wait a while"

ARRAY(MailBee.Message) am
int nMessages=ReceiveMail("" 0x100 "" 0 am)
out nMessages

 BEGIN PROJECT
 main_function  Macro2313
 exe_file  $my qm$\Macro2313.qmm
 flags  6
 guid  {FFEC6B9C-5358-4104-95CC-B7B797713D0D}
 END PROJECT
