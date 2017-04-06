str user
GetUserComputer user
 for older QM versions, instead of GetUserComputer use this:
 int nn=257
 if(!GetUserName(user.all(nn) &nn)) ret
 user.fix(nn-1)

sel user
	case "User 1"
	dis+ "macro or folder" 2 ;;disable
	case "User 2"
	dis- "macro or folder" 2 ;;enable
	 ...
