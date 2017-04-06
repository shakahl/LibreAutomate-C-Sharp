int hlv=child("" "SysListView32" win("Program Manager" "Progman"))
SetWinStyle hlv LVS_SMALLICON 1|8
 SetWinStyle hlv LVS_SMALLICON 2|8 ;;this restores big icons
 Other alternative styles are LVS_REPORT and LVS_LIST, but don't work.
