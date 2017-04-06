DATE d="5/5/2005"
SYSTEMTIME st
d.tosystemtime(st)

 int mc=win("" "SysMonthCal32") ;;if popup
 int mc=child("" "SysMonthCal32" win("" "DropDown")) ;;if child (note: window may be different)
int mc=id(3 win("Form" "#32770")) ;;another example

__ProcessMemory pm.Alloc(mc 1000)
pm.Write(&st sizeof(st))

SendMessage mc MCM_SETCURSEL 0 pm.address
 ret

  however MCM_SETCURSEL does not notify parent window.
  we could use eg key RL, but sometimes even that does not work.
  then try to send MCN_SELCHANGE message to the parent window.
