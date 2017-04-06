 /
function $source_file $dest_file

str sf df s
sf.getfile(source_file)
foreach s sf
	if(findc(s ':')<0) s.decrypt(9 s "password")
	df.formata("%s[]" s)
df.setfile(dest_file)
