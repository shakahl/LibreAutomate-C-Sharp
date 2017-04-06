function h

str sv sn sc
___DE_CONTROL& c=subs.GetControl(h)
_hsel=h
if(c.cid) _Name(c h sn sv); else sv="dialog"; sn=c.txt
sv.setwintext(_hselName)
sn.setwintext(_hselText)
_textChanged=0; _arrowMovSiz=0
subs.SetMark
sc.getwinclass(h)
int enText=!(SelStr(1 sc "ActiveX" "QM_Grid") or QmSetWindowClassFlags(sc 0x80000000)&4) ;;disable text editing for classes where text has special meaning
EnableWindow _hselText enText
SendMessage _htb TB_ENABLEBUTTON 1022 enText ;;Text
SendMessage _htb TB_ENABLEBUTTON 1021 SelStr(1 sc "QM_Grid")!0 ;;Properties
SendMessage _htb TB_ENABLEBUTTON 1008 c.cid!0 ;;Delete
