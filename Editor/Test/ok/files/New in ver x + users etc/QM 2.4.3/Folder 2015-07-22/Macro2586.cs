 above case IDB_UNDO:
		case ID_EDIT_HTML_WITHOUTCSS: case ID_EDIT_HTML_WITHTABSANDSPACES: case ID_EDIT_HTML_WITHOUTPRE:
			i=code==ID_EDIT_HTML_WITHOUTCSS ? 1 : (code==ID_EDIT_HTML_WITHTABSANDSPACES ? 2 : 4);
			if(dlg::MenuCheckToggle(g_hm.edit, code)) sett.htmlopt|=i; else sett.htmlopt&=~i;
			break;

 below dlg::MenuCheckMulti(g_hm.edit,
		ID_EDIT_HTML_WITHOUTCSS, sett.htmlopt&1,
		ID_EDIT_HTML_WITHTABSANDSPACES, sett.htmlopt&2,
		ID_EDIT_HTML_WITHOUTPRE, sett.htmlopt&4,
