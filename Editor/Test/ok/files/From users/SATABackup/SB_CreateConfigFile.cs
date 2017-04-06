; Configuration SB. INI File explanation
; How it works
; Global        > Category that stores the log file.
; Logfile       > Path and filename for log changes.
; ConfigX       > Is the category. Each category is a different source/dest and has it's own properties
; Orig          > Folder where Original folders are stored.
; Dest          > Folder where Backup should be copied.
; FileX         > X is to set different settings for filetypes.
;               File0 is default setting for those filetypes not configured explicitly.
;               Format: A,B,C
;               Read below what is each parameter.
;               FileX is a configurable extension with his own parameters
;               Format: ext,A,B,C
;                   A:
;                       -1 means no backup. This file doesn't being copied.
;                       0 means no versioning. Only a copy should be done
;                       X, bigger than 0 means X versions stored. It will copy the original file and X older versions of them
;                   B:
;                       Number of days before versions should be deleted (counts since modified date).
;                       -1 means NEVER
;                       Note: For security, any value lower than 10 will be converted to -1.
;                   C:
;                       Number of days before files should be deleted if original file doesn't exist any more (counts since modified date)
;                       -1 means NEVER
;                       Note1: For security, any value lower than 10 will be converted to -1.
;                       Note2: All the versions will be removed with the backup file!
;               Note: If A,B or/and C doesn't exist, SB_Backup suposes -1 (NO BACKUP!!!!), except
;                     if 'A' belongs to 'File0'. In this case, default is 10.


ARRAY(str)+ Sources.create(0)
ARRAY(str)+ Configs.create(0)
str+ INIFile
str+ Log

if(!ShowDialog("SB_ConfigBackupMenu" &SB_ConfigBackupMenu))ret
ARRAY(str) TempArray
int i j
str s sc sf
if (Log.len>3)
	rset Log "Logfile" "Global" INIFile
else
	rset Log "Logfile" "Global" INIFile -1
for i 0 Configs.len
	TempArray = Sources[i]
	sc.format("Config%i" (i+1))
	rset TempArray[0] "Orig" sc INIFile
	rset TempArray[1] "Dest" sc INIFile
	TempArray = Configs[i]
	rset TempArray[0] "File0" sc INIFile
	for _i 1 TempArray.len
		sf.format("File%i" _i)
		rset TempArray[_i] sf sc INIFile
out "%s saved" INIFile