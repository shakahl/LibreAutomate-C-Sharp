 \
function# iid FILTER&f
 ClearOutput
if(!f.hwnd2) ret -2

 this is a modified version of a filter function whose author is pi

 int t1=perf

sel _s.getwinexe(f.hwnd) 1
	case "ppc"
	if(wintest(f.hwnd "" "ClassClass"))
		mac "ppc_f1"
	
	case "vmware"
	if(wintest(f.hwnd "Sun Solaris - VMware Workstation" "VMUIFrame"))
		mac "vm_solaris_out"
	
	case "PrintScreen" mac "catchget"
	
	case "TwinHanDTV" mac "twinhan_fav"
	
	case "thunderbird" mac "thunderbird_middle"
	
	case "firefox" mac "firefox_middle"
	
	case "powerproexename"
	if(wintest(f.hwnd "" "powerpronote"))
		mac "ppro_note_run"
	
	 case "DesktopRuler" wmin(cla,h)
	
	case "blackbox"
	if(GetWinId(f.hwnd2)=22) mac "o3tt3rli"
	 else if(wintest(f.hwnd "" "ExploreWClass")) explorer_get_file
	
	case "strokeit"
	 sel GetWinId(f.hwnd2)
		 case 1011 del_item("strokeit")
		 case 1010 qm_paste_macro
	
	case "qm"
	if(wintest(f.hwnd "" "QM_Editor"))
		sel GetWinId(f.hwnd2)
			 case 2202 del_item("qm",iid)
			case 2201 ClearOutput
			case 2210 mac "qm_editor_getsel"
	else if(wintest(f.hwnd "DESKBAR")) mac "qm_deskbar_middle"
	
	case else ret -2 ;;default action in other programs

 int t2=perf
 out t2-t1
