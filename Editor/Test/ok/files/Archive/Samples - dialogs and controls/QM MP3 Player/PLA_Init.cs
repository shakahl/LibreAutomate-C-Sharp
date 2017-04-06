 /PLA_Main
function hDlg

PLA_DATA- d

d.hdlg=hDlg
d.hlv=id(3 hDlg)
d.p._getcontrol(id(8 d.hdlg))
d.iPlaying=-1

SetWinStyle d.hlv LVS_REPORT|LVS_SHAREIMAGELISTS|LVS_SINGLESEL|LVS_SHOWSELALWAYS 1
SendMessage d.hlv LVM_SETEXTENDEDLISTVIEWSTYLE LVS_EX_INFOTIP|LVS_EX_FULLROWSELECT -1

TO_LvAddCol d.hlv 0 "File" 300

str s
rget s "folder" "\Examples\QM MP3 Player"
s.setwintext(id(12 hDlg))
PLA_AddFolder

 d.p.uiMode="invisible"
d.p._setevents("d__WMPOCXEvents")