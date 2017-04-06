function style [flags] ;;style: 1 cannot edit first col, 2 cannot add/delete rows, 4 set row type, 8 numbered, 16 cannot edit, 32 check boxes, 64 2-click edit, 128 drag&drop, 0x100 tree;  flags: 0 set, 1 add, 2 remove, 4 listview exstyle

 Sets control style.

 style:
    If flags 4 is not set, specifies QM_Grid control style. Style flags:
        GRID.QG_NOEDITFIRSTCOLUMN (1) - user cannot edit first column.
        GRID.QG_NOAUTOADD (2) - user cannot add, delete or move rows.
        GRID.QG_SETROWTYPE (4) - see <help #IDP_QMGRID#details>grid row properties</help>.
        GRID.QG_AUTONUMBER (8) - in first column display row numbers. Also, FromX functions and RowsDeleteAll will not preserve first column, regardless of flags.
        GRID.QG_NOEDIT (16) - user cannot edit cells.
        GRID.QG_CHECKBOXES (32) - adds list view extended style LVS_EX_CHECKBOXES.
           Also, when getting text, if flag QG_GET_SELECTED (4), gets only checked items.
           Also, if LVS_EX_BORDERSELECT, checks/unchecks when clicked in any non-editable area of the row.
           Sends LVN_ITEMCHANGED notification only on click, and not when checked/unchecked using functions or messages.
        GRID.QG_EDITIFSELECTED (64, QM 2.4.0) - on click, begin edit mode only if the row was selected and focused.
        GRID.QG_DRAGDROP (128, QM 2.4.2) - user can drag and drop rows.
        GRID.QG_TREE (0x100, QM 2.4.2) - assume that rows in the grid are indented to display a tree.
    More info in QM Help -> Grid control -> grid.h.

 flags:
    1 - add the styles without changing other styles.
    2 - remove the styles without changing other styles.
    4 - style is list view control extended style (LVS_EX_...). For reference look for LVM_SETEXTENDEDLISTVIEWSTYLE in MSDN.

 This function does not set WS_, WS_EX_ and LVS_ styles. To set them use dialog editor or SetWinStyle.


int f m
sel flags&3
	case 0 f=style; m=-1
	case 1 f=style; m=style
	case 2 m=style

if(flags&4) Send(LVM_SETEXTENDEDLISTVIEWSTYLE m f)
else Send(GRID.LVM_QG_SETSTYLE f m)
