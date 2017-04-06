Dir d
if d.dir("E:\" 1) ;;if exists
	long sz=d.FileSize
	out sz
	out d.FileAttributes
	out d.IsFolder
	str sd=d.TimeModified2
	out sd
	out d.FileName
	out d.FileName(1)
	

