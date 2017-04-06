int w=win("Welcome to BANK FOR AGRICULTURE AND AGRICULTURAL CO-OPERATIVES - Windows Internet Explorer" "IEFrame")
act w
Htm e=htm("SELECT" "form:accountCombo_input" "" w "0" 2 0x121 30) ;;this code created with the "Find html element" dialog. If cannot capture the SELECT tag, capture the outer element and use the arrow buttons until you'll get SELECT tag.
e.Mouse(1)
Htm e1=htm("LI" "50004 - *" "" w "0" 23 0x21 5)
e1.Mouse(1)
out 2
