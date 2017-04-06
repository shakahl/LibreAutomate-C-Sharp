int h=child("" "Ate32Class" win("Username1 : Username2- Instant Message" "AIM_IMessage") 0x5)
rep
	if(!IsWindow(h)) ret ;;window closed
	str a.getwintext(h)
	sel a 3
		case "*Usernamehere: sign out*"
		mou 45 45
		case "Sign Out"
		mac "another macro"
		break
	wait 0.1
