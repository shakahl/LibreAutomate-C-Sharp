function $items [firstId] [flags] ;;flags: 4 set item states

 Adds one or more menu items.
 Can be called multiple times. Appends to existing items.

 items - list of menu items.
   Note: the format is similar but not the same as of menu definitions that you create with the Menu Editor and use with MenuPopup.Create.
 firstId:
   When adding 1 item at a time (items is 1 line) - id of the item.
   When adding multiple items (items is multiple lines) - id of the first added item. Other items will have incrementing ids.
   If omitted or 0, item ids must be specified in the list, like "1 item1[]2 item2[]3 item3".
   The Show function will return id of the selected item, or 0 if closed without selecting.
 flags (QM 2.4.3):
   4 - if item text begins with ".number" (or "id.number" or ">.number"), the number specifies item state or/and type.
      The number is one or more states/types: 1 disabled, 2 checked, 4 radio chack (item type), 8 default (a menu can have 1 default item).
      Example items string: "1 Normal[]2.1 Disabled[]3.2 Checked[]4.3 Checked and disabled". Here the first number is item id, then follows dot, state, space and text.

 REMARKS
 For separator use "-".
 For vertical separator use "|". When adding 1 item at a time, use "|[]Next item" (not "|" and "Next item").
 For submenus use > and < like in <help #IDH_POPUP>QM popup menus</help>. Cannot add 1 item at a time (like ">a" "b" "<"); add whole submenu string (like ">a[]b[]<").

 See also: <ShowMenu>, <MenuPopup.Create>, <DT_SetMenuIcons>.

 Example in class help.


str s=items
ARRAY(lpstr) a
if(!tok(s a -1 "[]" 1)) ret

if(!m_h) m_h=CreatePopupMenu

sub.CreateSubmenu(m_h a 0 firstId flags)


#sub CreateSubmenu
function# hm ARRAY(lpstr)&a i idAdd flags

MENUITEMINFOW mi
int vert i0
lpstr s se

for i i a.len
	s=a[i]; s+strspn(s "[9]")
	
	memset &mi 0 sizeof(mi); mi.cbSize=sizeof(mi); mi.fMask=MIIM_TYPE|MIIM_ID
	
	i0=i
	sel s[0]
		case '-': s+1; mi.fType=MFT_SEPARATOR
		case '|': vert=MFT_MENUBARBREAK; continue
		case '>':
			mi.hSubMenu=CreatePopupMenu
			i=sub.CreateSubmenu(mi.hSubMenu a i+1 idAdd flags)
			s+1; mi.fMask|MIIM_SUBMENU
		case '<': break
	
	if idAdd
		mi.wID=i0+idAdd
	else
		mi.wID=__Val(s se)
		if(se>s) s=se+(se[0]=32)
	
	if mi.fType!MFT_SEPARATOR
		if s[0]='.' and flags&4
			int f=__Val(s+1 se)
			if se>s+1
				s=se+(se[0]=32)
				if(f&11) mi.fMask|MIIM_STATE
				if(f&1) mi.fState|MFS_DISABLED
				if(f&2) mi.fState|MFS_CHECKED
				if(f&4) mi.fType=MFT_RADIOCHECK
				if(f&8) mi.fState|MFS_DEFAULT
		
		mi.dwTypeData=@s
	mi.fType|=vert; vert=0
	InsertMenuItemW(hm 0xffff 1 &mi)

ret i
