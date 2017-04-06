function row !check ;;check: 1 check, 0 uncheck

 Checks or unchecks checkbox.

 row - 0-based row index.


LVITEM li.stateMask=0xF000
li.state=iif(check 0x2000 0x1000)
Send(LVM_SETITEMSTATE row &li)
