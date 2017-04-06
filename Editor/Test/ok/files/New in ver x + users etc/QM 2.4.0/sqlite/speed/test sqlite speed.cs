dll "qm.exe" !TestReadQmSqliteFile $dbFile
str sf; rget sf "file debug" "Software\GinDi\QM2\settings"
 sf="$my qm$\test\ok.db3"
 sf="G:\test\ok.db3"

if 1
	 RunConsole2 F"Q:\Downloads\Contig.exe ''{sf.expandpath}''"
	 out GetFileFragmentation(sf)
	DeleteFileCache sf
	 DeleteFileCache "$qm$\sqlite3.dll"
	 _s.expandpath(sf); _s.fix(2); DeleteFileCacheAll _s
	 _s.getfile("$qm$\qmsetup.dll") ;;move HDD head somewhere
 #ret
0.5

 out __sqlite.sqlite3_config(__sqlite.SQLITE_CONFIG_SINGLETHREAD)
 out __sqlite.sqlite3_config(__sqlite.SQLITE_CONFIG_MULTITHREAD)
 ARRAY(int) a; GetIdsOfItemsWhereNeedTextAtStartup a; 0.1

 PF
 _s.getfile(sf); _s.all; PN
 FileAddToCache sf; PN ;;1 frag 115512, 38 frag 587103
TestReadQmSqliteFile(sf)
 call "Function249" sf &a
 PN; PO

 ok
 speed: 17  63596  1208578  6371  (137 fragments, text in same table)
 speed: 40  24794  266576  21537  (1 fragment, text in same table)
 speed: 47  37203  84228  197400  (137 fragments)
 speed: 11  29848  28310  62059  (1 fragment)
 speed: 9  61819  60931  30210  
 speed: 9  23270  61114  30381  
 speed: 12  21837  48720  11094  (ANALYZE called at the beginning)
 speed: 9  51902  54421  35914  
 speed: 12  2697  48589  33527  
 speed: 12  18091  52854  11351  
 speed: 12  30250  51723  24954  
 speed: 9  24445  51753  243750  (1 frag, + texts)
 speed: 11  20987  46262  113434/145732  (1 frag, + texts, 4kb pages)
 speed: 118973  10  799  44752  49613  (cache, 1 frag, + texts, 4kb pages)
 speed: 9  43699  114169  1332133  (usage, 75 frag, 9.2MB, 8kb pages)
 speed: 8  68965  177556  1598093  (usage, 79 frag, 9.2MB, 1kb pages)

 system
 speed: 11  31367  27853  34125  (26 fragments)
 speed: 12  27504  4358  5235  (1 fragment)
 speed: 12  2652  7950  3343  
 speed: 8  2663  7607  22007  

 main
 speed: 18  24820  363  3143  (2 fragments)


  using main.db3
 speed: 118326  66225  67365  499  
 speed: 2185  35620  1044  464  
 speed: 1880  12483  14473  
 speed: 123204  133653  486  
 speed: 117183  198788  499  
  using ok.db3
 speed: 63683  76217  1211346  (136 fragments)
 speed: 72057  38823  229624  (defragmented)
 speed: 74  17604  268260  
 speed: 52  24196  222412  
 speed: 63  17755  238655  
 speed: 75  11565  222654  
 speed: 64  896  99720  (reload)
 speed: 65  16393  262658  (no text)
 speed: 70  15169  224127  (no text)
 speed: 67  925  64922  (no text, reload)
 speed: 57  16358  150819  6540  (no text, compress)
 speed: 62  888  61475  6224  (no text, compress, reload)
  using system.db3
 speed: 50  12398  34728  (defragmented)
 speed: 48  17283  35003  
 speed: 71  1231  49310  
 speed: 81  1319  56264  
 speed: 72  1256  37386  
 speed: 64  1259  47523  
 speed: 46  1059  18883  (reload)

 text in separate table
 speed: 84383  23577  28585  (after reboot)
 speed: 62  12187  28227  54937   
 speed: 61  891  27369  5292  (reload)
 speed: 47  10351  28090  15526  (compress)
 speed: 60  14301  28736  27392  (compress)
 speed: 47  15834  28464  36290  (compress)
 speed: 49  927  27566  5461  (compress, reload)
  using system.db3
 speed: 53  22412  4692  19990  
 speed: 61  33526  4709  7818  
 speed: 57  13842  4790  19970  
 speed: 50  873  3905  3543  (reload)
 speed: 48  15294  4706  17628  (compress)
