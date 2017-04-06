 /
function $itemText [$tsMenu]

 Executes specified TS menu item.

 itemText - TS menu item text.
 tsMenu - TS menu name.


if(empty(tsMenu)) tsMenu="TS Menu4" ;;change this to your default TS menu name

 create temp popup menu
str s.getmacro(tsMenu)
str ss.getmacro("temp_ETSMI"); err
if(ss!s) newitem("temp_ETSMI" s "Menu" "" "\User\Temp" 3)
 run item
mac "temp_ETSMI" itemText

err+ end _error

 How it works?
 Creates a temporary popup menu with text of the TS menu.
 Popup menus have a feature that TS menus don't: an item can be executed with mac without showing the popup menu.
