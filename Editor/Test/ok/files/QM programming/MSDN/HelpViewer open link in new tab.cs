spe 10
int w=child(mouse 1)
act w
if WinTest(w "Internet Explorer_Server")
	Acc a.FromMouse(1)
	int r=a.Role
	 int r=a.Role(_s); out _s
	sel r
		case ROLE_SYSTEM_LINK
		key+ C; lef; key- C
		
		 case ROLE_SYSTEM_STATICTEXT
		 a.Navigate("parent2")
		  r=a.Role(_s); out _s
		 if(r=ROLE_SYSTEM_PAGETAB) goto gCloseTab
		  gCloseTab
		 a.Navigate("child") ;;or a.Find(a.a "PUSHBUTTON" "" "")
		  r=a.Role(_s); out _s
		 a.DoDefaultAction
		 
		 case ROLE_SYSTEM_PAGETAB goto gCloseTab
		
		case else
		 key CF4 ;;sometimes does not work
		
		BlockInput 1
		w=GetAncestor(w 2)
		scan "image:h42C2D30F[]image:h4BA6701D" w 0 1|2|16|0x1000 ;;2 images because sometimes the tab not yellow
		lef; mou
		BlockInput 0
else
	 open in new tab
	key+ C; lef; key- C
