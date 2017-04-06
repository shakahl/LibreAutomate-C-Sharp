str s
int h=GetQmCodeEditor
int lens=SendMessage(h SCI.SCI_GETTEXTLENGTH 0 0)
s.fix(SendMessage(h SCI.SCI_GETTEXT lens+1 s.all(lens)))
out s
