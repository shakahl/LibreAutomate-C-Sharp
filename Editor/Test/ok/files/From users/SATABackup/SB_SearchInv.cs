function ARRAY(str)&F2 ~Path1 ~Path2

;; Input: Path1, Path2
;; Returns: Array of files backuped and deleted from source (Path1).
;;          This array includes all file versions, either deleted and not.
;; 	The filelist begins with '\' + relative path from Path2.

int i j
Dir d
str s

ARRAY(str) F1
GetFilesInFolder(F1 Path1 "*" 0|4)
GetFilesInFolder(F2 Path2 "*" 0|4)
ARRAY(str) F1aux.create(F1.len)
ARRAY(str) F2aux.create(F2.len)
ARRAY(str) F1MD5.create(F1.len)
ARRAY(str) F2MD5.create(F2.len)

str s1 s2
for i 0 F1.len
	d.dir(F1[i])
	s1.get(d.FileName(1) Path1.len)
	for j 0 F2.len
		d.dir(F2[j])
		s2.get(d.FileName(1) Path2.len)
		if (s1 = s2)
			F2.remove(j)
			j = F2.len
for i 0 F2.len
	F2[i].remove(0 Path2.len)
ret F2

