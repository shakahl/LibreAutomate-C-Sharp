SYSTEMTIME- t_st1 ;;we share these variables with the dialog procedure
if(!ShowDialog("dialog_with_month_cal" &dialog_with_month_cal 0 _hwndqm)) ret

DATE d.fromsystemtime(t_st1) ;;convert to DATE (for easy display)
str s=d
out s

 display date and time separately
DATE d2
int i=d.date ;;get integer part, that is date
d2=i
out d2
d2=d.date-i ;;get fractional part, that is time
out d2
