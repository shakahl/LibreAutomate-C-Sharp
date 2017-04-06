 inserts <help ...> link in QM tips or VC

str s ss sss
int w1=win ;;VC or QM
int wHelp
run "$qm$\QM2Help.chm" "" "" "" 0x800 win("QM Help" "HH Parent") wHelp
if(mes("Open the topic" "" "OCn")!='O') ret
Acc a=acc("" "PANE" wHelp "Internet Explorer_Server" "mk:@MSITStore*" 0x1004)
ss=a.Value
findrx(ss "\bID\w_\w+(?=\.html)" 0 1 s)
act w1
ss.getsel
sss=F"<help #{s}>{ss}</help>"
sss.setsel
