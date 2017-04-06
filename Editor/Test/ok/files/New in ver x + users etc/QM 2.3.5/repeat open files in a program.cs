int interactive=1 ;;replace 1 to 0 if don't need a message box

ARRAY(str) a
GetFilesInFolder a "c:\folder with xml files" "*.xml" 4
if(a.len=0) end "No files found. Try to change GetFilesInFolder arguments."
int i
for i 0 a.len ;;repeat for each xml file
	str& sPath=a[i] ;;sPath variable contains full path of current xml file
	out sPath ;;remove this if don't need to see file paths in QM output
	
	if interactive
		sel mes(sPath "Process this file?" "YNC")
			case 'N' continue ;;No - skip this file
			case 'C' break ;;Cancel - stop macro
	
	 Here add code that opens the file (sPath), presses buttons etc.
	 All lines must begin with tab, or will not be repeated.
	