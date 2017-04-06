str sFiles sFile
if(!DialogSelectFilesFolders(sFiles)) ret

foreach sFile sFiles
	out sFile
