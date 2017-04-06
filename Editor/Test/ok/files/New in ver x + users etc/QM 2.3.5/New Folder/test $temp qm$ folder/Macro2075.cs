/exe
 SendMail "support@quickmacros.com" "test" "aaa" 0x50000
SmtpMail m
int i
for(i 0 3) m.AddMessage("support@quickmacros.com" "test" F"{i}")
m.Send(0x50000)
 mes 1

 BEGIN PROJECT
 main_function  Macro2075
 exe_file  $my qm$\Macro2075.qmm
 flags  6
 guid  {813F1AF4-74CD-4667-A00E-7648DA2653EB}
 END PROJECT
