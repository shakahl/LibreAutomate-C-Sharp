str sf.expandpath("$my qm$\test\ok.db3")
 str sf.expandpath("$my qm$\test\system.db3")
 str sf.expandpath("$my qm$\test\main.db3")
 str sf.expandpath("$my qm$\ok.qmc")
 str sf.expandpath("$pf$\quick macros 2\system.qml")
 str sf.expandpath("$my qm$\test\test.uql")
 str sf="G:\test\ok.db3"

out GetFileFragmentation(sf)

PF
 Sqlite x.Open(sf); x.Exec("VACUUM"); x.Close; out GetFileFragmentation(sf); PN

#if 0
 RunConsole2 F"Q:\Downloads\Contig.exe -a ''{sf}''"
RunConsole2 F"Q:\Downloads\Contig.exe ''{sf}''"


 RunConsole2 "Q:\Downloads\Contig.exe -a q:\app\ok.qml"
 RunConsole2 "Q:\Downloads\Contig.exe q:\app\ok.qml"

 RunConsole2 "Q:\Downloads\Contig.exe -a q:\app\sqlite3.dll"
 RunConsole2 "Q:\Downloads\Contig.exe q:\app\sqlite3.dll"

#else
 str sft=F"{sf}.tmp"
 if(CopyFileW(@sf @sft 0)) PN; if(!ReplaceFileW(@sf @sft 0 1 0 0)) end "failed"
  if(CopyFileW(@sf @sft 0)) PN; if(!MoveFileExW(@sft @sf MOVEFILE_REPLACE_EXISTING)) end "failed"
  if(CopyFileW(@sf @sft 0))
	  PN
	  __Handle ht=WINAPI2.CreateTransaction(0 0 0 0 0 0 0)
	  if(ht=-1) end "CT failed"
	  if(!WINAPIV.MoveFileTransactedW(@sft @sf 0 0 MOVEFILE_REPLACE_EXISTING ht)) end "MFT failed"
	  if(!WINAPI2.CommitTransaction(ht)) end "Commit failed"
	  CloseHandle ht; ht=0
Sqlite_Defragment sf 1
PN;PO
out GetFileFragmentation(sf)

 Sqlite x.Open(sf); x.Exec("VACUUM"); x.Close; out GetFileFragmentation(sf); PN
