 /
function $urlList $saveFolder ARRAY(str)&ar [flags] [inetflags] ;;flags: 0x10000 show total progress

 Downloads multiple files with progress dialog.
 Error if something fails or user clicks Cancel.

 urlList - multiline list of URLs.
 saveFolder - folder where to save downloaded files. If does not exist, creates. If "", downloads to variable instead.
 ar - variable for results.
   This function creates 2-dim array.
   The first dimension will contain URLs (from urlList).
   The second dimension will contain paths of saved files. If saveFolder is "", it will contain file data. If fails to download a file, and user clicks Ignore, the element will be empty.
 inetflags - same as with <help>IntGetFile</help>.


type __IGFMP $urlList $saveFolder ARRAY(str)*ar flags inetflags

ar=0

str controls = "3"
str lb3
lb3=urlList

ShowDialog("__IGFMP_Dialog" &__IGFMP_Dialog &controls 0 0 0 0 &urlList)
if(!ar.len) end "failed"
