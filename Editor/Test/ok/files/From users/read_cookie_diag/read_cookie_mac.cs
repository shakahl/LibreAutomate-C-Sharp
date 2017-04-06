//set variables
str logon logon_form cookie_dir cookie_text form_data cookie_dir_wildcard cookie_path

//get user logon name
 if(rget(logon "Logon User Name" "Software\Microsoft\Windows\CurrentVersion\Explorer")) ;;there is no such registry value on Vista
if(GetUserComputer(logon))

//format str "logon_form" for use in filename
	logon_form=logon
	logon_form.lcase
	logon_form.replacerx(" " "_" 1)
	
//format cookie directory and wildcard search string	
	cookie_dir.from("C:\Documents and Settings\" logon "\Cookies\")
	cookie_dir_wildcard.from(cookie_dir logon_form "*local*.*")
	del cookie_dir_wildcard ;;kills any existing cookies created by this process
	err ;;not yet created
	
//open html form in dialog
// FIXME --> nicked from QM "diag w/ browser"
// HELP --> How to resume macro upon HTML submit button press??
	str+ site = "c:\formtest\cookie.html"
	str controls = "3"
	str ax3SHD
	if(!ShowDialog("read_cookie_diag" &read_cookie_diag &controls)) ret

//enumerate string match and read file
	lpstr cookie_actualname=dir(cookie_dir_wildcard)
	rep
		if(cookie_actualname = 0) break
		cookie_path.from(cookie_dir cookie_actualname)
		cookie_text.getfile(cookie_path)
		cookie_actualname = dir
		form_data=cookie_text
		
	// Process form data
		form_data.replacerx("%20" " " 1)
		out form_data

// FIXME --> enumerate/parse data next






