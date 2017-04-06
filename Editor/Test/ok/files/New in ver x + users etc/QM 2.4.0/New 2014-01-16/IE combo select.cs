int w=win("Welcome to BANK FOR AGRICULTURE AND AGRICULTURAL CO-OPERATIVES - Windows Internet Explorer" "IEFrame")
act w
Htm e=htm("SELECT" "form:accountCombo_input" "" w "0" 2 0x121 3) ;;this code created with the "Find html element" dialog. If cannot capture the SELECT tag, capture the outer element and use the arrow buttons until you'll get SELECT tag.
e.SetFocus
key AD ;;Alt+Down Arrow - show drop-down list
 then navigate to the element with arrows, and press Tab to select
key D(#2) T       ;; Down(*2) Tab

 out e.HTML
 e.CbSelect(2)
 e.CbSelect("50004*")
