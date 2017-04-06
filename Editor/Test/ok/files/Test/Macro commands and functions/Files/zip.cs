ClearOutput
str zf="$desktop$\test.zip"
str sf.getmacro("zip list")
str sw

out "zip"
zip zf sf 1 sw
if(sw.len) out sw

out "unzip"
zip- zf "$desktop$\unzip" 0 sw
if(sw.len) out sw

