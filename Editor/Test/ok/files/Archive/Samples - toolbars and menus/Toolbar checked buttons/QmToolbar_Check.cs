 /
function# check ;;check: 0 uncheck, 1 check, 2 toggle

 Checks, unchecks or toggles the check state of the clicked button of current QM toolbar.
 Returns the final state: 1 checked, 0 unchecked.

 REMARKS
 Call this from a QM toolbar item code.
 Gets button from mouse position.


int h=id(9999 TriggerWindow)
POINT p; xm p h 1
int i=SendMessage(h TB_HITTEST 0 &p)
if(i<0) ret

int isChecked=SendMessage(h TB_ISBUTTONCHECKED i 0)
if(check<=1 and isChecked=check) ret check
check=!isChecked
if(!SendMessage(h TB_CHECKBUTTON i check)) ret isChecked
ret check
