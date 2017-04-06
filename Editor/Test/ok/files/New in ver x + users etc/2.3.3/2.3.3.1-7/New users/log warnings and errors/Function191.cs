 /Macro650

out
atend LogErrors
 act "hhhhhh"
 err

if mes("Error?" "" "YN")='Y'
	end ErrDescrAndLine("Error description.")

#ret
if 1
	 _error.description="Could not put data on FTP server"
	 end _error ;;if(!f.FilePutFromStr(data ftpfile))
	end "Could not put data on FTP server // if(!f.FilePutFromStr(data ftpfile))"

