 /
function $qmItemList

 Automates creating exe from all QM items specified in qmItemList (multiline).


int iid=qmitem ;;remember currently open QM item to restore finally

act _hwndqm
str s
foreach s qmItemList
	mac+ s ;;open the QM item
	men 33233 _hwndqm ;;Make Exe...
	int wMA=wait(30 WA win("Make exe" "#32770" "qm"))
	err end "failed. Make sure that exe settings are saved, so that would not show dialog 'Where to save settings'."
	but 1 wMA ;;click OK
	wait 0 -WC wMA ;;wait until Make Exe dialog closed
	0.2 ;;make this number bigger if error "Compiler busy"

mac+ iid
