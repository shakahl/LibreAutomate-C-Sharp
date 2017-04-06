str attachmentsList=
 C:\Quotes\2014\08-05 Smith  -  Johnson  -  Comparison.pdf
 C:\Quotes\2014\08-05 Smith - Johnson - Gen $4500 Monthly 6yrs Shared 3% Comp.pdf
 C:\Quotes\2014\08-05 Smith-Johnson-Thrivent $4500 Monthly 6yrs Shared 3% Comp.pdf

int w=wait(30 WA win("Write: " "Mozilla*WindowClass"))
str sa
foreach sa attachmentsList
	key Aftf
	int w2=wait(30 WA win("Attach File(s)" "#32770"))
	 key An
	 key (sa) Y
	int c=child("" "ComboBox" w2 0x0 "id=1148") ;;combo box 'File name:'
	sa.setwintext(child("" "Edit" c))
	key Y
