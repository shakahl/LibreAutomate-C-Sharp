function ARRAY(str)&F1 ~Path1 ~Path2

;; Input: Path1, Path2
;; Returns: Array of modified/new files in Path1. Path2 is the backup
 	The filelist has an initial '\' + relative path from Path1.
;; To compare two files, compares the date, filename and size as reference
;; (It's an MD5 of all these strings. It's not an MD5 of the file because of the low speed!).

int i j
Dir d
str s

ARRAY(str) F2
GetFilesInFolder(F1 Path1 "*" 0|4)
GetFilesInFolder(F2 Path2 "*" 0|4)
ARRAY(str) F1aux.create(F1.len)
ARRAY(str) F2aux.create(F2.len)
ARRAY(str) F1MD5.create(F1.len)
ARRAY(str) F2MD5.create(F2.len)

for _i 0 F1.len
	if d.dir(F1[_i]) ;;if file exists
		_s = d.TimeModified2
		s.get(d.FileName(1) Path1.len)
		F1aux[_i].format("%s%s%i" s _s d.FileSize) ;; Filename
		F1MD5[_i] = _s.encrypt(2|8 _s.format("%s" F1aux[_i]))
for _i 0 F2.len
	if d.dir(F2[_i]) ;;if file exists
		_s = d.TimeModified2
		s.get(d.FileName(1) Path2.len)
		F2aux[_i].format("%s%s%i" s _s d.FileSize) ;; Filename
		F2MD5[_i] = _s.encrypt(2|8 _s.format("%s" F2aux[_i]))

for i F1.len-1 -1 -1
	for j 0 F2.len
		if (F1MD5[i] = F2MD5[j])
			F1.remove(i)
			break
	if (j = F2.len)
		F1[i].remove(0 Path1.len)
ret F1