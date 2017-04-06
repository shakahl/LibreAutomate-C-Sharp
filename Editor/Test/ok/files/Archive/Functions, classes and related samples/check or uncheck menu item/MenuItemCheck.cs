 /
function action $itemText [$menuClass] ;;action: 0 none (just return state), 1 check, 2 uncheck, 3 toggle

 Checks, unchecks or toggles an item in a pop-up menu in currently active window. Or just gets its state.
 Returns 1 if the item was checked when calling this function, or 0 if not.
 Before calling this function, the pop-up menu must be invoked. For example you can use key command that presses Alt+underlined letter.
 If the pop-up menu is still not visible, this function waits max 10 s. On timeout throws error. Also throws error if the specified item not found.
 This function always closes the pop-up menu.

 action - see above.
 itemText - item text.
 menuClass - pop-up menu window class. Default is "#32768", which is class of standard menus. This function also works with some nonstandard pop-up menus, but then must be specified pop-up menu window class. For example, in Word 2003 it is "MsoCommandBarPopup".

 EXAMPLE
 spe ;;to make faster, we don't need autodelay after key
 key Ae ;;invoke pop-up menu. It is necessary because check mark often is added/removed just before invoking the pop-up menu.
 int wasChecked=MenuItemCheck(2 "Wrap Lines") ;;uncheck and get previous state
 out wasChecked
 spe -1 ;;restore normal autodelay to be used further


if(empty(menuClass)) menuClass="#32768"

int h=wait(10 WV win("" menuClass)) ;;wait for pop-up menu
err end "the pop-up menu must be visible"

Acc a=acc(itemText "MENUITEM" h "" "" 0x1000)
err end "item not found"

int checked=a.State&STATE_SYSTEM_CHECKED!=0
int close click
sel action&3
	case 0 close=1
	case 1 if(checked) close=1; else click=1
	case 2 if(checked) click=1; else close=1
	case 3 click=1

if(click) a.DoDefaultAction; err end ES_FAILED ;;check or uncheck
else key F10 ;;close the pop-up menu
wait 5 -WV h; err

ret checked
