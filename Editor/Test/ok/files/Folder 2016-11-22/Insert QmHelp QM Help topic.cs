int w=win("QM Help" "HH Parent")
if(w) act w; else w=QmHelp("IDH_REFERENCE")
if(mes("Open the topic, then click OK." "" "OCn")!='O') ret
Acc ap=acc("" "PANE" w "Internet Explorer_Server" "mk:@MSITStore*" 0x1004)
str s=ap.Value
findrx(s "\bID\w_\w+(?=\.html)" 0 1 s)
err+ mes _error.description; ret
InsertStatement F"QmHelp ''{s}''"
