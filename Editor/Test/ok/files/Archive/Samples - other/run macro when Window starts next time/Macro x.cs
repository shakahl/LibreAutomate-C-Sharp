\
if(!rget(_s "QM - macro x" "Software\Microsoft\Windows\CurrentVersion\Run")) ret ;;don't run accidentally
rset "" "QM - macro x" "Software\Microsoft\Windows\CurrentVersion\Run" 0 -1 ;;delete the value

mes 1
