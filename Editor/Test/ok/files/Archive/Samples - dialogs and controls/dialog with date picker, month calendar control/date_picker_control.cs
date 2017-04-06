SYSTEMTIME- t_st1 t_st2 t_st3 ;;we share these variables with the dialog procedure
if(!ShowDialog("dialog_with_date_picker" &dialog_with_date_picker 0 _hwndqm)) ret

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

 other two controls
d.fromsystemtime(t_st2)
out d
d.fromsystemtime(t_st3)
out d

 def DTS_UPDOWN 0x0001 
 def DTS_SHOWNONE 0x0002 
 def DTS_SHORTDATEFORMAT 0x0000 
 def DTS_LONGDATEFORMAT 0x0004 
 def DTS_TIMEFORMAT 0x0009 
 def DTS_APPCANPARSE 0x0010 
 def DTS_RIGHTALIGN 0x0020 
