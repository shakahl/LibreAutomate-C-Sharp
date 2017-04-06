 /
function hwndUpdown hwndBuddy rangeMin rangeMax [position]

 Initializes up-down control (msctls_updown32).
 Call from dialog procedure, under WM_INITDIALOG.
 Also, include 2 in msctls_updown32 style in dialog definition (eg change 0x54000000 to 0x54000002).


SendMessage hwndUpdown UDM_SETBUDDY hwndBuddy 0
SendMessage hwndUpdown UDM_SETRANGE32 rangeMin rangeMax
SendMessage hwndUpdown UDM_SETPOS32 0 position
