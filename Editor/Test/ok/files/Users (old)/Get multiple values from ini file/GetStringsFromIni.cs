 /
function $iniFile str*a [flags] [$encryptionKey] ;;flags: 1 values are qm-escaped

 Reads multiple string values from an ini file to a str array.

 iniFile - ini file.
 a - array of str variables.
   The first variable must contain value names separated by =. Example "value1=value2=value10".
   Subsequent variables will receive corresponding values.
 flags: 1 - values in the ini file are escaped like strings in QM code. For example, '' used for ", [] for new line.
 encryptionKey - if used, the ini file is encrypted using the key, using str function encrypt with algorithm 9.


str sf.getfile(iniFile)
if(len(encryptionKey)) sf.decrypt(9 sf encryptionKey)

str& sn=a[0]
ARRAY(str) an
tok sn an -1 "="

int i
for i 0 an.len
	str& sv=a[i+1]
	 out an[i]
	str rx.format("^[ \t]*%s[ \t]*=([^[]]*)$" an[i])
	 out rx
	if(findrx(sf rx 0 9 sv 1)<0) sv.all
	sv.trim
	if(matchw(sv "''*''")) sv.get(sv 1 sv.len-2)
	if(flags&1) sv.escape
	 out sv
	