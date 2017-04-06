 Wraps pastebin API.
 <link>http://pastebin.com/api</link>

 The functions throw error if failed, for example if an argument is invalid.

 EXAMPLES

str devKey="xxxxxxxxxxxxxxxxxxxxxxxxxxxx"
str user="xxxxxxxx"
str password="xxxxxxxx"
str text=
 test
 Pastebin class
str response

#compile "__Pastebin"
Pastebin x.Init(devKey)
sel ListDialog("Paste as guest[]Paste as user[]List user's pastes[]Get a paste[]Delete a paste" "Run example" "Pastebin")
	case 1 ;;paste as guest
	response=x.Paste("text pasted as guest" "example" 1 "10M")
	out response
	run response
	
	case 2 ;;paste as user
	x.Login(user password)
	response=x.Paste("text pasted as user" "example" 2)
	out response
	run response
	
	case 3 ;;list
	x.Login(user password)
	response=x.List()
	out
	out response
	
	case 4 ;;get
	x.Login(user password)
	response=x.Get("45VaC3eG")
	out response
	
	case 5 ;;delete
	x.Login(user password)
	x.Delete("45VaC3eG")
