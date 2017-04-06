 /
function $source $destFolder ARRAY(str)&aSources ARRAY(str)&aDestinations [flags] ;;flags: 1 no intermediate folder

 Gets paths of all source and destination files to be used in a copy operation.

 source - file, folder or list of files and/or folders. Also can be drive(s). Error if does not exist.
 destFolder - destination folder.
 aSources - receives full paths of all files (but not folders) to be copied.
 aDestinations - receives full paths of all files (but not folders) that would be created in destFolder after a copy opertion.


aSources=0; aDestinations=0

str sDF sDF2 s1 s2
sDF.expandpath(destFolder); sDF.rtrim("\")

foreach s1 source
	s2.expandpath(s1); s2.rtrim("\")
	if(!dir(s2 2)) end "source not found"
	
	if _dir.fd.dwFileAttributes&FILE_ATTRIBUTE_DIRECTORY
		if flags&1
			sDF2=sDF
		else
			int i=findcr(s2 '\')
			if(i>0) sDF2.from(sDF s2+i)
			else _s.left(s2 1); sDF2.from(sDF "\" _s) ;;E:
		
		Dir d
		foreach(d _s.from(s2 "\*") FE_Dir 0x4)
			str sPath=d.FileName(1)
			aSources[]=sPath
			aDestinations[].from(sDF2 sPath+s2.len)
	else
		aSources[]=s2
		aDestinations[].from(sDF s2+findcr(s2 '\'))
