int iid=qmitem
mac+ "Macro1998"
key Cp
int w=wait(0 win("Properties - Macro1998" "#32770"))
but 1068 w ;;push button 'Schedule...'
w=wait(30 WA win("Task Scheduler" "#32770"))
wait 0 -WC w
key Z
mac+ iid
