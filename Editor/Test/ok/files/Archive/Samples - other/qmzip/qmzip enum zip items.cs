 note: requires qmzip.dll. In QM 2.3.4 and later it is not installed with QM.
 http://www.quickmacros.com/forum/viewtopic.php?f=1&t=3507&p=15364#p15364

out

ref qmzip qmzip_def

str zf.searchpath("$desktop$\test.zip")
if(!zf) end "zip file not found"

int hzip=qmzip.OpenZip(zf.unicode 0 ZIP_FILENAME)
if(!hzip) end "failed"
 note: in QM 2.3.0 and later, with OpenZip and CreateZip, file name must be Unicode UTF-16

ZIPENTRY z
int i n
int zr=qmzip.GetZipItem(hzip -1 &z); if(zr) goto g1 ;;call with index -1 to get info about the zip file, particularly number of items
n=z.index

for i 0 n
	zr=qmzip.GetZipItem(hzip i &z); if(zr) goto g1
	lpstr name=&z.name
	DATE d1.fromfiletime(z.mtime); str sd1.time(d1 "%x %H:%M")
	out "name=''%s'',  size=%i,  compressed size=%i,  date modified=%s" name z.unc_size z.comp_size sd1

 g1
qmzip.CloseZip(hzip)
if(zr) end "failed"
