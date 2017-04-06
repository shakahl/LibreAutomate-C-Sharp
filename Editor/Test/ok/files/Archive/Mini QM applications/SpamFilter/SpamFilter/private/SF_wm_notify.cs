function# hDlg wParam NMHDR&nh

str s ss; int i j; SFDELETED* sd

sel nh.code
	case LVN_ITEMCHANGED
	NMLISTVIEW& nl=+&nh; i=nl.iItem
	if(i<0 or nl.uNewState&LVIS_FOCUSED=0 or nl.uOldState&LVIS_FOCUSED) ret
	 gsel
	sel(nh.idFrom) case 3 j=1; case 6 j=2; case else ret
	SF_Select j i nh.hwndFrom
	
	case [NM_CLICK,NM_RCLICK,NM_DBLCLK]
	NMITEMACTIVATE& na=+&nh
	i=na.iItem; if(i<0) ret
	if(nh.code=NM_CLICK) goto gsel
	
	sel nh.idFrom
		case 3
		if(nh.code=NM_DBLCLK) goto g1
		s="    "; if(a[i].flags&0x8000) s[3]=2
		sel PopupMenu("Message[]Source[]-[]Delete[9]Delete[]-[]Select All[9]Ctrl+A" 0 0 s)
			case 1
			 g1
			s="$temp$\qmsftmsg.eml"; if(MailSaveMessage(a[i].m s)) run s
			case 2 ShowText "Source" s.from(a[i].m.RawBody) 0 1
			case 4
			 gdel1
			for(i 0 a.len) if(SendMessage(nh.hwndFrom LVM_GETITEMSTATE i LVIS_SELECTED)) a[i].flags|0x8000; j=1
			if(j) SF_Update 0 1; SF_SetTimer
			case 6 goto gsa
			
		case 6
		sd=+SF_LvGetLparam(nh.hwndFrom i); if(!sd) ret
		if(nh.code=NM_DBLCLK) goto g2
		sel PopupMenu("Message[]Source[]-[]Delete[9]Delete[]Restore[]-[]Select All[9]Ctrl+A")
			case 1
			 g2
			run sd.sf; err
			case 2 ShowText s ss.getfile(sd.sf) 0 1; err
			case 4
			 gdel2
			if('O'!=mes("Are you sure you want to delete selected messages?[][]These messages have been deleted from server and saved here in spam folder as eml files. This action will delete message files. This action is not undoable." "" "OC!")) ret
			for(i SendMessage(nh.hwndFrom LVM_GETITEMCOUNT 0 0)-1 -1 -1)
				if(SendMessage(nh.hwndFrom LVM_GETITEMSTATE i LVIS_SELECTED))
					sd=+SF_LvGetLparam(nh.hwndFrom i)
					if(dir(sd.sf)) del- sd.sf; err continue
					SendMessage(nh.hwndFrom LVM_DELETEITEM i 0)
			SF_Select
			case 5
			mes "Not implemented"
			 this implementation is not finished and unreliable
			 SmtpMail smtp
			 for(i SendMessage(nh.hwndFrom LVM_GETITEMCOUNT 0 0)-1 -1 -1)
				 if(SendMessage(nh.hwndFrom LVM_GETITEMSTATE i LVIS_SELECTED))
					 sd=+SF_LvGetLparam(nh.hwndFrom i)
					 if(!dir(sd.sf)) continue
					 smtp.AddMessage("" "" "" 16 "" "" "" "" sd.sf)
					 smtp.GetMessage(-1 m)
					 
					 m.ToAddr=""; m.CCAddr=""; m.BCCAddr="" ;;or may send to other people
					  here should get email address of the account that received this message, and set m.ToAddr
					 
					 MailHeader m "X-QMSF" "restored"; MailHeader m "X-QMSF-Comments"
					 j+1
			 if(j) smtp.Send(0x500); SF_Update 0 2
			case 7 goto gsa
	
	case LVN_KEYDOWN
	if(hid(hDlg)) ret
	NMLVKEYDOWN& nk=+&nh
	sel nk.wVKey
		case VK_DELETE if(!GetMod) sel(nh.idFrom) case 3 goto gdel1; case 6 goto gdel2
		case 'A' if(GetMod!2) ret
		 gsa
		LVITEM lvi.mask=LVIF_STATE; lvi.stateMask=LVIS_SELECTED; lvi.state=-1
		for(i 0 SendMessage(nh.hwndFrom LVM_GETITEMCOUNT 0 0))
			SendMessage(nh.hwndFrom LVM_SETITEMSTATE i &lvi)
	
	case LVN_DELETEITEM
	if(nh.idFrom=6) &nl=+&nh; sd=+SF_LvGetLparam(nh.hwndFrom nl.iItem); sd._delete

err+ ;;out _error.description
