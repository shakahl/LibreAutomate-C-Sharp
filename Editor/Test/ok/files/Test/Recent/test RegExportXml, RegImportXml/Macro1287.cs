str f.expandpath("$desktop$\r.dat")
str rk="software\gindi\qm2\user"

 these fail on Vista/7 User account
SetPrivilege SE_RESTORE_NAME
SetPrivilege SE_BACKUP_NAME

sel list("save[]restore")
	case 1
	del- f; err
	RegKey k.Open(rk)
	out RegSaveKey(k f 0)
	
	case 2
	if(!k) out RegCreateKeyEx(HKEY_CURRENT_USER rk 0 0 0 KEY_ALL_ACCESS 0 &k.hkey &_i)
	out RegRestoreKey(k f REG_FORCE_RESTORE)
