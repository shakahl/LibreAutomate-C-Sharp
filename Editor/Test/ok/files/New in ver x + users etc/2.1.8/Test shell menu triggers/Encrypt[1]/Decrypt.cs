function $Files
str EncFileName PW PWH DecFileName DecFilePath DecFile EncFile PWHFile SelFileName
int OVW DelSrc
OVW=1
DelSrc=0
if(!inp(PW "Enter password" "Encrypt!" "*" DelSrc "Delete encypted file?"));;get password,end if cancel
		end 
foreach SelFileName Files
	
	EncFileName=SelFileName ;;get file name
	PWH.encrypt(2|8 PW) ;;get password hash
	EncFile.getfile(EncFileName) ;;get file data
	PWHFile.left(EncFile 32) ;;get file password hash
	EncFile.remove(0 32) ;;remove pw hash
	
	if(PWH = PWHFile);;see if the pasword is good
		DecFile.decrypt(1 EncFile PW) 
		DecFilePath.left(EncFileName len(EncFileName)-4) ;;create a file name without the .qmp
		
		if(dir(DecFilePath))
			if(mes("File already exsists, overwrite orignal?" "Encrypt!" "YN?")='Y')
				DecFile.setfile(DecFilePath)							
		else
			DecFile.setfile(DecFilePath)
		if(DelSrc)
			del-(EncFileName)
	else
		mes "Bad password for at least one file!!" "Encypt!"
		end