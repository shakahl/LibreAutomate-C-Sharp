int c=GetQmCodeEditor

str newText=F" $BkM []" 
 int j=SendMessage(c SCI.SCI_GETCURRENTPOS 0 0)
 SendMessage c SCI.SCI_GOTOPOS j+len(newText)-2 0

int j=SendMessage(c SCI.SCI_GETCURRENTPOS 0 0)
int l=SendMessage(c SCI.SCI_LINEFROMPOSITION j 0)
SendMessage c SCI.SCI_GOTOLINE l 0
SendMessage c SCI.SCI_INSERTTEXT -1 newText
j=SendMessage(c SCI.SCI_GETLINEENDPOSITION l 0)
SendMessage c SCI.SCI_GOTOPOS j 0
0.25
int x y cx cy
GetCaretXY x y cx cy
ShowTooltip "Type the bookmark's label" 3 x y+cy 0 1