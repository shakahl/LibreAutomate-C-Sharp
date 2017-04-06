 Creates and shows a popup menu.
 Also has functions to disable, check items, etc.
 A MenuPopup variable also can be used as menu handle with Windows API menu functions.

 Added in: QM 2.3.2.

 EXAMPLES
 These examples use MenuPopup.AddItems. You can instead create menu definition in the Menu Editor (or at run time), and pass the md variable to MenuPopup.Create or ShowMenu.

str s=
 1 Text (1 is item id; Text is label)
 -
 2 Use - line for separator
 >Submenu
 	15 Tabs at the beginning are ignored
 	16 Tab in the middle	right-aligns text
 	<
 |
 25 Use | line for vertical break
 >30 Another submenu (with id)
 	31 &Ampersand (&&) underlines the character
 	-32 (separator with id)
 	Menu item id is optional
 	<

MenuPopup x.AddItems(s)
int i=x.Show
out i

  or

 int i=ShowMenu(s)
 out i

 ------------

s=
 1 normal
 2 disabled
 3 checked
 -
 4 radio group
 5 radio group, checked
 6 radio group
 -
 7 delete
 8 delete
 9 delete
 10 bold
 11 left-text	right-text

MenuPopup m.AddItems(s)

m.DisableItems("2")
m.CheckItems("3")
m.CheckRadioItem(4 6 5)
m.DeleteItems("7-9")
m.SetBold(10)

i=m.Show

str s1 s2
if(i and m.GetItemText(i s1 s2))
	out s1
	out s2
