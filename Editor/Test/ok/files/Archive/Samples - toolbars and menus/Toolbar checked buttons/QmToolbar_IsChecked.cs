 /
function#

 Gets the checked state of the clicked button of current QM toolbar.
 Returns 1 if checked, 0 if not.

 REMARKS
 Call this from a QM toolbar item code.
 Gets button from mouse position.


int h=id(9999 TriggerWindow)
POINT p; xm p h 1
int i=SendMessage(h TB_HITTEST 0 &p)
if(i<0) ret

ret SendMessage(h TB_ISBUTTONCHECKED i 0)
