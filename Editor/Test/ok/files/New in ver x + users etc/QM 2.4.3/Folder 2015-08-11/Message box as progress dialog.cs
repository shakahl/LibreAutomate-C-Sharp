int hwndStatic=sub.ShowMessage

for _i 0 5
	out _i
	str s=F"Searching ...test ({_i})"; s.setwintext(hwndStatic); err
	0.5


#sub ShowMessage
mac "sub.MessageThread"
int hwndMessage=wait(10 WV win("Error" "#32770" "" 0 "cClass=Static[]cText=9373524757"))
atend sub.CloseMessage hwndMessage
ret child("9373524757" "Static" hwndMessage)


#sub MessageThread
mes "9373524757                                    [][]" "Error" "x"


#sub CloseMessage
function hwndMessage
clo hwndMessage; err
