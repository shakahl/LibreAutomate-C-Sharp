str s
if(CU_GetFile("http://www.quickmacros.com/support.html" &s))
	out s
	if(CU_PutFile("http://www.quickmacros.com/support2.html" &s))
		mes "file has been successfully copied"
		