str macro="ShowDialog"
int n=10 ;;1-based line index

 open macro and go to line n
mac+ macro
SendMessage GetQmCodeEditor SCI.SCI_GOTOLINE n-1 0

 display a link in QM output or other control that supports tags
str s=
F
 <><open "{macro} /L10">open ShowDialog and go to line {n}.</open> QM 2.3.2.
out s
