 This macro creates folders that are specified in the "dirlist" macro. Requires QM 2.1.5.

str sf sl
sl.getmacro("dirlist") ;;get the list. Or, use getfile to get the list from a file.

foreach sf sl
	if(!sf.len or sf.beg(" ")) continue ;;skip empty lines and comments (lines that begin with a space)
	mkdir sf
