
 change these

int debug=1 ;;set to 0 to actually send. If 1, displays message source and does not send.
str xlsFile="$desktop$\MassEmail97.xls"
int startRange=4 ;;Excel row index of the first recipient
int endRange=5 ;;Excel row index of the last recipient
str sendersName="David"
str subject="Test MassEmail97"
str message=
 Message text.
 Message text.

 ________________________

if(debug) out ;;clear output

ExcelSheet es.Init("" 8 xlsFile) ;;open the file in hidden Excel process
ARRAY(str) a
es.GetCells(a F"B{startRange}:D{endRange}") ;;get the specified range into 2-dim array

SmtpMail sm
int i
for i 0 a.len ;;for each row
	str body=F"{a[0 i]} {a[1 i]},[][]{message}[][]Sincerely, {sendersName}[]"
	if(debug)  out F"------------ to {a[2 i]} --------------[]{body}"
	sm.AddMessage(a[2 i] subject body)

int flags=0x100 ;;progress dialog
if(debug) flags|0x10000|0x20000 ;;don't send; preview
sm.Send(flags) ;;send using default accout. You can set it through 'Send email message' dialog.
