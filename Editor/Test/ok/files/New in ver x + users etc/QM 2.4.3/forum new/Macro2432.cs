 if image A found, and image B not found

int w=win("Downloader" "DownloadDialog")

int a = scan("image:hE64E7CFD" id(101 w) 0 16) ;;note: removed flags 1|2
if a
	int b = scan("image:h6FE351F4" id(101 w) 0 16)
	if !b
		dou 9 25 id(101 w) 1
