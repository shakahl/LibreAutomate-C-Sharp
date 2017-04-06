function $fileOld $fileNew str&sHeader [flags] ;;flags: 1 don't expand paths

 Creates text patch file header.

 fileOld - file that is considered "old".
 fileNew - file that is considered "new".
 sHeader - receives header text.

 Both files must exist.
 File times will be UTC.


str sf1 sf2
if(flags&1) sf1=fileOld; sf2=fileNew; else sf1.expandpath(fileOld) sf2.expandpath(fileNew)

 time
Dir d1 d2; DateTime t1 t2; lpstr ft="{yyyy-MM-dd} {TT}.{FF}"
if(!d1.dir(sf1) or !d2.dir(sf2)) end "failed to get file properties"
d1.TimeModified(+&t1 0 0 1)
d2.TimeModified(+&t2 0 0 1)

sHeader.format("--- %s  %s[]+++ %s  %s[]" sf1 t1.ToStrFormat(ft) sf2 t2.ToStrFormat(ft))

err+ end _error
