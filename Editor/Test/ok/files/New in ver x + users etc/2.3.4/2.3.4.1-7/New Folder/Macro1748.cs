_s.searchpath("$user profile$\..")
_s+"\*"
Dir d
foreach(d _s FE_Dir 0x1) ;;for each user account
	str sf=d.FileName
	str sff=d.FileName(1)
	sel(sf 1) case ["All Users","Default","Default User","Public"] continue
	out F"------- {sf} ---------"
	
	sff+"\My Documents\My QM\Main.qml"
	if sff
		 del sff ;;move to recycle bin. Disabled when testing.
		out F"Deleted: {sff}"
	else
		out F"Not found: {sff}"
		
	 sff+"\My Documents\My QM\*"
	 Dir d1
	 foreach(d1 sff FE_Dir)
		 str sPath=d1.FileName(1)
		 out sPath
		
	

