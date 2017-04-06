IntLoginDialog "http://www.quickmacros.com/forum/index.php"

for _i 0 2
	IntGetFile "http://www.quickmacros.com/forum/index.php" _s
	if(findw(_s "Logout")>=0) break ;;find something that exists only when logged-in
	if(_i or !IntLoginDialog("http://www.quickmacros.com/forum/ucp.php?mode=login")) end "failed to log in"
out _s
