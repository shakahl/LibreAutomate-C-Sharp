/exe
 out
dll "qm.exe" long'Crc64 !*data1 len1

str sd.getfile("q:\my qm\test\cs.dll")
 str sd="test"
long R1 R2
 out Crc64(sd sd.len)

PF
CsScript x.Init
PN
rep(1000) R1=Crc64(sd sd.len) ;;13
PN
R2=x.x.Test(sd sd.len)
 x.LoadFromMemory(sd sd.len) ;;23
PN
PO
out F"{R1} {R2}"

 BEGIN PROJECT
 main_function  Macro1447
 exe_file  $my qm$\Macro1447.qmm
 flags  6
 guid  {1D8D6E50-CF8A-4944-9E99-2E7E3BF43E30}
 END PROJECT
