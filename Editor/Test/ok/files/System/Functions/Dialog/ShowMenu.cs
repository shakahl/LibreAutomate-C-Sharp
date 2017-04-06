 /
function# $items [hwndOwner] [POINT&where] [flags] [tpmFlags] ;;flags: 1 enable keyboard, 2 auto-create item ids, 4 set item states.  tpmFlags: 4 align center horz, 8 align right, 16 align center vert, 32 align bottom, ...

 Creates and shows popup menu.
 Returns selected item id or 0.

 items - list of menu items.
   QM 2.4.2. Can be menu definition. You can create menu definitions with the Menu Editor. Look in floating toolbar -> More Tools.
 hwndOwner - owner window. Must belong to current thread.
 where - POINT variable containing menu position. Default: 0 - mouse position.
 flags:
   1 - enable keyboard navigation and accelerator characters (characters preceded by &).
      Don't need this flag if hwndOwner used and is active.
      Temporarily deactivates the currently active window.
   2 - auto-create item ids.
      The ids will match line numbers in the list, starting from 1.
      If this flag not used, item id must be specified at the beginning of each line.
      This flag is not used with menu definition.
   4 (QM 2.4.3) - if item text begins with ".number" (or "id.number" or ">.number"), the number specifies item state or/and type.
      The number is one or more states/types: 1 disabled, 2 checked, 4 radio chack (item type), 8 default (a menu can have 1 default item).
      Example items string: "1 Normal[]2.1 Disabled[]3.2 Checked[]4.3 Checked and disabled". Here the first number is item id, then follows dot, state, space and text.
 tpmFlags - <google>TrackPopupMenuEx flags</google>.
   The function adds TPM_RETURNCMD flag if not present, but removes if present.

 See also: <MenuPopup.AddItems>, <MenuPopup.Create>, <MenuPopup.Show>, <MenuPopup help>, <DT_SetMenuIcons>, <ListDialog>, <ShowDropdownList>.
 Added in: QM 2.3.2.

 EXAMPLES
 str s=
  1 Text (1 is item id; Text is label)
  -
  2 Use - line for separator
  3 Use | line for vertical break
  >Submenu
  	15 Tabs at the beginning are ignored
  	16 Tab in the middle	right-aligns text
  	<
  >30 Another submenu (with id)
  	31 &Ampersand (&&) underlines the character
  	-32 (separator with id)
  	Menu item id is optional
  	<
 
 int i=ShowMenu(s)
 out i

  example with menu definition
 str md=
  BEGIN MENU
  &Open :1
  &Save :2
  -
  >Submenu
  	Item1 :11
  	Item2 :12
  	<
  END MENU
 
 int j=ShowMenu(md); out j


MenuPopup m
if(!findrx(items __S_RX_MD 0 RX_ANCHORED)) m.Create(items)
else m.AddItems(items flags&2!0 flags&4)

ret m.Show(hwndOwner where flags&1 tpmFlags)
