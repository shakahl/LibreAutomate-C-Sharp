 1
str sf="$my qm$\test\ok.db3"
 str sf="$my qm$\test\main.db3"
 str sf="G:\ok.db3"
PF
sf.expandpath
RunConsole2 F"Q:\Downloads\Contig.exe ''{sf}''"
 DeleteFileCache sf
DeleteFileCacheAll "C:"
PN
_s.getfile(sf)
PN; PO

 speed: 52249  1065721  
 speed: 56252  1065482  
