function! $macro

 Deletes scheduled task of a macro.
 Returns 1 if successfully deleted, 0 if the task does not exist. Error if exists and failed to delete.

 macro - QM item name or +id.

 EXAMPLE
 ScheduledTask x
 x.Delete("macro1000")


if(macro<0x10000) _i=macro; macro=_s.getmacro(_i 1)
ret Delete2(F"QM - {macro}")
err+ end _error
