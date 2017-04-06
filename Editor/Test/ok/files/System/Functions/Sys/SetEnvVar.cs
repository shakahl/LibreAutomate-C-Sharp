 /
function! $name ~value [flags] ;;flags: 1 delete

 Adds, changes or deletes an environment variable.
 Returns: 1 success, 0 failed.

 REMARKS
 value can be string or numeric.
 Works only with variables of this process. Has no effect on system variables.
 To change system variables you can use setx.exe, it is in Windows Vista and later.

 Added in: QM 2.3.0.


if(flags&1) value.all
ret 0!SetEnvironmentVariableW(@name @value)
