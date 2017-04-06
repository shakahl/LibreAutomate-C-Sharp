 get user name of curent account
str user; GetUserInfo &user
out user

 is it my account?
sel user
	case "G"
	 it's me
	
	case else
	 it's another account, let's switch user
	shutdown 6
	15 ;;wait until in the "switch user" screen
	if(!EnsureLoggedOn(1)) end "failed to unlock computer"
	out "info: switched user to run this macro"

 now the rest of the macro
out "macro"
