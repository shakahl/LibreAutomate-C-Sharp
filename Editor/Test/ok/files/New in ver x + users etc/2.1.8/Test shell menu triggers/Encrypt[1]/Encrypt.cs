function $Files
spe -1
str DecFile,EncFile,DecFileName,EncFileName, PW, PWH,Ext, SelFileName

int DelSrc=1
if(inp(PW "Enter password" "Encypt!" "*" DelSrc "Delete Source?"))
	foreach SelFileName Files
		
		DecFileName=SelFileName ;;get file name
	
		DecFile.getfile(DecFileName)  ;;error if folder or file does not exsist
		err
			mes "File does not exsist" "Encypt!"
			end
		
		EncFile.encrypt(1 DecFile PW) ;;encrypt file
		EncFileName.from(DecFileName ".qmp") ;;new file name
		PWH.encrypt(2|8 PW) ;;create password hash to add to beginning of file
		EncFile.from(PWH EncFile)
		EncFile.setfile(EncFileName)
		if(DelSrc)
			del-(DecFileName)
