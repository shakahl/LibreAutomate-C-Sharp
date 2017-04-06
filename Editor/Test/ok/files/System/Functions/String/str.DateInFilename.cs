function$ $filenameWithDateFormat [$path] [DATE'date]

 Formats filename containing date/time.
 Returns: self.

 filenameWithDateFormat - filename containing date/time format fields used with str.timeformat. To create the string, you can use the Text dialog from the floating toolbar.
 path - append to the beginning. Can optionally end with \. The function expands special folders and environment variables.
 date - if used and not 0, formats with that date. Else formats with current date.

 REMARKS
 If the date/time contains invalid file name characters (/, :, etc), replaces them with -.

 Added in: QM 2.3.2.

 EXAMPLE
 str s
 s.DateInFilename("file-{D}.log" "$temp qm$")
 out s


__Str_GetNotThis(this filenameWithDateFormat _s)
if(empty(path)) this.fix(0)
else this.expandpath(path); if(!this.end("\")) this+"\"

str st
if(date) st.timeformat(filenameWithDateFormat date); else st.timeformat(filenameWithDateFormat)
st.ReplaceInvalidFilenameCharacters("-")

this+st
ret this
