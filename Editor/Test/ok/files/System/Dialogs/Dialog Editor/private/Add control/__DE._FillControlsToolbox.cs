 /Dialog_Editor

SendMessage _htv TVM_SETIMAGELIST 0 _il

int hi
sub.Add(0 "Button" 1 "Button" BS_MULTILINE|WS_TABSTOP|WS_GROUP 0 48 14 "Button")
sub.Add(0 "Check" 2 "Button" BS_AUTOCHECKBOX|BS_MULTILINE|WS_TABSTOP 0 48 10 "Check")
sub.Add(0 "Edit" 3 "Edit" ES_AUTOHSCROLL|WS_TABSTOP|WS_GROUP WS_EX_CLIENTEDGE 96 12)
sub.Add(0 "Static text" 9 "Static" 0 0 48 12 "Text")
sub.Add(0 "Group, or multi-drag container (drag with Shift)" 6 "Button" BS_GROUPBOX|WS_GROUP 0 108 100)
hi=TvAdd(_htv 0 "Button")
sub.Add(hi "Option first" 7 "Button" BS_AUTORADIOBUTTON|BS_MULTILINE|WS_TABSTOP|WS_GROUP 0 58 10 "Option first")
sub.Add(hi "Option next" 8 "Button" BS_AUTORADIOBUTTON|BS_MULTILINE 0 58 10 "Option next")
sub.Add(hi "Button OK" 1 "<OK>" BS_DEFPUSHBUTTON|WS_TABSTOP|WS_GROUP 0 48 14 "OK")
sub.Add(hi "Button Cancel" 1 "<Cancel>" WS_TABSTOP|WS_GROUP 0 48 14 "Cancel")
hi=TvAdd(_htv 0 "Edit")
sub.Add(hi "Edit multiline" 25 "Edit" WS_VSCROLL|ES_AUTOVSCROLL|ES_MULTILINE|ES_WANTRETURN|WS_TABSTOP|WS_GROUP WS_EX_CLIENTEDGE 96 48)
sub.Add(hi "Edit password" 24 "Edit" ES_PASSWORD|WS_TABSTOP|WS_GROUP WS_EX_CLIENTEDGE 96 12)
sub.Add(hi "Edit number" 23 "Edit" ES_NUMBER|WS_TABSTOP|WS_GROUP WS_EX_CLIENTEDGE 32 12)
sub.Add(hi "Edit read-only" 22 "Edit" WS_VSCROLL|ES_AUTOVSCROLL|ES_READONLY|ES_MULTILINE WS_EX_STATICEDGE 96 48)
sub.Add(hi "Rich edit" 21 "RichEdit20A" WS_VSCROLL|ES_AUTOVSCROLL|ES_DISABLENOSCROLL|ES_MULTILINE|ES_WANTRETURN|WS_TABSTOP|WS_GROUP WS_EX_CLIENTEDGE 96 48)
sub.Add(hi "QM edit" 3 "QM_Edit" ES_AUTOHSCROLL|WS_TABSTOP|WS_GROUP WS_EX_CLIENTEDGE 96 12)
hi=TvAdd(_htv 0 "ComboBox, ListBox")
sub.Add(hi "Combo read-only" 4 "ComboBox" CBS_DROPDOWNLIST|WS_VSCROLL|CBS_AUTOHSCROLL|WS_TABSTOP|WS_GROUP 0 96 12)
sub.Add(hi "Combo editable" 4 "ComboBox" CBS_DROPDOWN|WS_VSCROLL|CBS_AUTOHSCROLL|WS_TABSTOP|WS_GROUP 0 96 12)
sub.Add(hi "Combo simple" 4 "ComboBox" CBS_SIMPLE|WS_VSCROLL|CBS_AUTOHSCROLL|CBS_NOINTEGRALHEIGHT|WS_TABSTOP|WS_GROUP 0 96 48)
sub.Add(hi "List" 5 "ListBox" LBS_NOTIFY|WS_VSCROLL|WS_TABSTOP|WS_GROUP|LBS_NOINTEGRALHEIGHT WS_EX_CLIENTEDGE 96 48)
sub.Add(hi "List multi-select" 5 "ListBox" LBS_NOTIFY|WS_VSCROLL|LBS_MULTIPLESEL|WS_TABSTOP|WS_GROUP|LBS_NOINTEGRALHEIGHT WS_EX_CLIENTEDGE 96 48)
sub.Add(hi "QM combo read-only" 4 "QM_ComboBox" CBS_DROPDOWNLIST|CBS_AUTOHSCROLL|WS_TABSTOP|WS_GROUP 0 96 12)
sub.Add(hi "QM combo editable" 4 "QM_ComboBox" CBS_DROPDOWN|CBS_AUTOHSCROLL|WS_TABSTOP|WS_GROUP 0 96 12)
hi=TvAdd(_htv 0 "Static")
sub.Add(hi "Icon" 10 "Static" SS_ICON 0 16 16)
sub.Add(hi "Bitmap" 10 "Static" SS_BITMAP 0 16 16)
sub.Add(hi "Horizontal line" 11 "Static" SS_ETCHEDHORZ 0 97 2)
sub.Add(hi "Vertical line" 12 "Static" SS_ETCHEDVERT 0 2 17)
sub.Add(hi "Text with links (SysLink)" 35 "SysLink" WS_TABSTOP|WS_GROUP 0 96 12 "<a>Link1</a>, <a>link2</a>")
hi=TvAdd(_htv 0 "Advanced")
sub.Add(hi "Other controls..." 13 "<other>")
sub.Add(hi "ActiveX controls..." 14 "<ax>" WS_TABSTOP|WS_GROUP 0 96 48)
sub.Add(hi "Web browser" 26 "ActiveX" WS_TABSTOP|WS_GROUP 0 96 48 "SHDocVw.WebBrowser {8856F961-340A-11D0-A96B-00C04FD705A2}")
sub.Add(hi "Grid" 28 "QM_Grid" WS_TABSTOP|WS_GROUP|WS_CLIPCHILDREN|LVS_REPORT|LVS_OWNERDATA|LVS_SHAREIMAGELISTS WS_EX_CLIENTEDGE 96 48 "0[]A[]B")
#ifdef USE_INFO_CONTROL
sub.Add(0 "Info (not in exe; not in DE unless def USE_INFO_CONTROL 1)" 22 "QM_DlgInfo" 0 WS_EX_STATICEDGE 96 48)
#endif


#sub Add c
function htviParent $tvText [image] [$cls] [style] [exstyle] [cx] [cy] [$txt]

___DE_ADDCONTROL& r=_aadd[]
r.cls=cls; r.style=style; r.exstyle=exstyle; r.cx=cx; r.cy=cy; r.txt=txt

TvAdd _htv htviParent tvText _aadd.len image
