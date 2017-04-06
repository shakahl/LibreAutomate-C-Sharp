 /
function $fileOrFolder [$permissions] [str&sResults]

 Changes security permissions (access rights, ACL) of a file or folder.

 fileOrFolder - path of the file or folder.
 permissions - string in this format:
   [/T] [/E] [/C] [/G user:perm] [/R user [...]] [/P user:perm [...]] [/D user [...]]
     /P user:perm  Replace specified user's access rights.
                   Perm can be: N  None
                                R  Read (read&execute, list, read)
                                C  Change (modify, read&execute, list, read, write)
                                F  Full control
     /G user:perm  Grant (add) specified user access rights. Perm can be same as with /P, except N.
     /D user       Deny specified user access.
     /R user       Revoke specified user's access rights (only valid with /E).
     /T            Changes ACLs of specified files in the current directory and all subdirectories.
     /E            Edit ACL instead of replacing it.
     /C            Continue on access denied errors.
   Here [ ] means optional.
   If omitted or empty, instead shows current permissions in QM output or stores in sResults.
 sResults - variable that receives output text of CACLS.EXE.

 REMARKS
 This function runs Windows program CACLS.EXE (hidden) and passes fileOrFolder and permissions to it in command line.
 Wildcards can be used to specify more than one file.
 You can specify more than one user.
 May fail if /E not used.
 QM must be running as admin.

 Added in: QM 2.3.4.
 <c 0xFFFFFF>Keywords: set.</c>

 EXAMPLE
 str f="$Common AppData$\__TestMyApp451"
 mkdir f
 ChangeFileSecurity f "/E /P Users:F" ;;allow full control to the Users group


str f s
if(!f.searchpath(fileOrFolder)) end ERR_FILE

int R=RunConsole2(F"$system$\CACLS.EXE ''{f}'' {permissions}" s)
if(R) end F"{ERR_FAILED}. {_s}"

if(&sResults) sResults=s
else if empty(permissions)
	str sa=
 Abbreviations:
    N None, R Read, C Change, F Full control.
    CI - Container Inherit. The ACE will be inherited by directories.
    OI - Object Inherit. The ACE will be inherited by files.
    IO - Inherit Only. The ACE does not apply to the current file/directory.
    ID - Inherited. The ACE was inherited from the parent directory's ACL.
	out "%s[][]%s" s.rtrim sa

err+ end _error
