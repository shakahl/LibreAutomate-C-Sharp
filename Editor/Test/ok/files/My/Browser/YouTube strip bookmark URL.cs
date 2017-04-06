OnScreenDisplay "Right click the bookmark" 30 0 0 0 0 0 8
wait 30 MR; err ret
key i
int w=wait(30 WA win("Properties for" "MozillaDialogClass"))
key Al
str s.getsel
s.replacerx("&.+")
s.setsel
key Y
