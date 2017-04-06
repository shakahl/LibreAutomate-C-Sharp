/Dialog_Editor

 BEGIN DIALOG
 0 "" 0x10CF0A44 0x100 0 0 223 38 "Form"
 1 Button 0x54030001 0x4 120 16 48 14 "OK"
 2 Button 0x54030000 0x4 172 16 48 14 "Cancel"
 END DIALOG

function# hDlg message wParam lParam

if(!hDlg)
	ClearOutput
	ShowDialog("Form" &Form)
	ret

sel message
	case WM_INITDIALOG
	DT_Init(hDlg lParam)

	sel 2
		case 1
		 IUnknown+ ocx1._createcontrol(hDlg uuidof("CDNotification.CDNotify"))
		 out ocx1
		
		case 2 ;;STI (tray)
		typelib STILib {43772685-E77F-11D1-9568-042405C10000} 1.0
		STILib.STI- ocx2._create
		out "---created----"
		ocx2._setevents("ocx_Events")
		out "---events set----"
		ocx2.Appear
		 
		 case 3 ;;my created control (VC)
		 typelib+ AXCONTROLLib {FC0D080B-EC58-4D25-864A-749A0C5F4560} 1.0
		 AXCONTROLLib.AxControl- ocx3._create
		  ocx.AboutBox
		 ocx3.Method(1)
		 
		 case 4 ;;NetConnect
		 typelib NetConnect {955E547D-2DE1-4704-B343-CDEDC21F2566} 1.0
		 opt waitmsg 1
		 NetConnect.NetConnect- ocx4._create
		  ocx4.AutoConnection
		 ocx4.DefaultConnection
		 
		 case 5 ;;Timers
		 typelib April16_Timers {31946E9C-3E50-4A83-9710-EB0019066670} 2.0
		 April16_Timers.STimer- ocx5._create
		 ocx5._setevents("Timers")
		 word en(-1) in(1)
		 ocx5.Interval=&in
		 ocx5.Enabled=&en
		 
		 case 6 ;;TextToSpeach
		 typelib ACTIVEVOICEPROJECTLib {EEE78583-FE22-11D0-8BEF-0060081841DE} 1.0
		 ACTIVEVOICEPROJECTLib.DirectSS- ocx6._create
		  ocx6.AboutDlg(_hwndqm "k")
		  out ocx6.Speaker(0)
		  #opt dispatch 1
		 ocx6.Sayit="one"
		 
		 case 7 ;;MSForms
		 typelib MSForms {AC2DE821-36A2-11CF-8053-00AA006009FA} 2.0
		 MSForms.UserForm- ocx7._create
		  ocx7.DesignMode=0
		 ocx7._SetHeight(100)
 
		 case 8
		 typelib HyperLinkControl {ED6EFBE9-2DE5-11D2-9B4A-006097731E48} 1.0
		 HyperLinkControl.HLink c._create
		  c.URL="http://www.quickmacros.com/index.html"
		  c.OpenURL

	ret 1
	 ----
	case WM_COMMAND
	int msg(wParam >> 16) ctrlid(wParam & 0xFFFF)
	sel msg
		case BN_CLICKED
		sel ctrlid
			case IDOK
			DT_Ok hDlg
			 ----
			case IDCANCEL
			EndDialog(hDlg 0)
		 ----
		case EN_CHANGE
		case LBN_SELCHANGE
		case CBN_SELENDOK
	ret 1
	 ----
	case WM_DESTROY
	DT_DeleteData(hDlg)
