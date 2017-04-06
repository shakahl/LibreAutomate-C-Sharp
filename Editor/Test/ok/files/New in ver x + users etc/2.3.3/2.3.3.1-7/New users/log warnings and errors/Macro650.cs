 Function191
 #ret

out
atend LogErrors
 act "hhhhhh"
 err

 if 1
	 end EndErr("Could not put data on FTP server")

 #ret
if mes("Error?" "" "YN")='Y'
	end "Could not put data on FTP server // if(!f.FilePutFromStr(data ftpfile))"
