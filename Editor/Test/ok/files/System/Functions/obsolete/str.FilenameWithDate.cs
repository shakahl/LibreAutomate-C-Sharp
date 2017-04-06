 /
function$ $filenameWithDateFormat [$path]

 Formats filename containing current date/time.
 If the date/time contains invalid filename characters (/, :, etc), replaces them with -.
 Obsolete. Use <help>str.DateInFilename</help>.

 filenameWithDateFormat - filename containing date/time format fields used with str.time. To create the string, you can use the Text dialog from the floating toolbar.
 path - append to the beginning. Can optionally end with \. The function expands special folders and environment variables.

 EXAMPLE
 str s
 s.FilenameWithDate("qm %x.log" "$my qm$")
 out s


if(!empty(path)) this.expandpath(path); if(!this.end("\")) this+"\"
else this.fix(0)

str st.time(filenameWithDateFormat)
st.ReplaceInvalidFilenameCharacters("-")

this+st
ret this
