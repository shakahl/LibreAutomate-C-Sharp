function hDlg ctrlid $text [flags] ;;flags: 1 ctrlid is hwnd, 2 change text

 Assigns tooltip text to a control, or changes tooltip text.
 The same CToolTip variable can be used with multiple controls.


TOOLINFO tti.cbSize	= sizeof(TOOLINFO);
tti.uFlags	= TTF_IDISHWND;
tti.hwnd	= hDlg;
tti.uId	= iif(flags&1 ctrlid GetDlgItem(hDlg, ctrlid));
tti.lpszText = text;
SendMessage(m_htt, iif(flags&2 TTM_UPDATETIPTEXT TTM_ADDTOOL), 0, &tti);
