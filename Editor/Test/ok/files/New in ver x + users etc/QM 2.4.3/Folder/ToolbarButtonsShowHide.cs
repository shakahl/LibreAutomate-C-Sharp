 /
function nButtons

 Hides or shows buttons of a QM user toolbar.

 nButtons - the number of buttons to hide/show. Actually it is the number of lines in toolbar text.

 This function must be called from that toolbar. It hides/shows buttons that follow the clicked button. It makes the clicked button checked when hides.


int htb=id(9999 TriggerWindow)
POINT p; xm p htb 1
int button=SendMessage(htb TB_HITTEST 0 &p)
int firstButton=button+1
int hide=!SendMessage(htb TB_ISBUTTONHIDDEN firstButton 0)
SendMessage(htb TB_CHECKBUTTON button hide)
int i
for i firstButton firstButton+nButtons
	if !SendMessage(htb TB_HIDEBUTTON i hide)
		 if failed, it is probably a separator, id -1
		if SendMessage(htb TB_SETCMDID i -i)
			SendMessage(htb TB_HIDEBUTTON -i hide)
