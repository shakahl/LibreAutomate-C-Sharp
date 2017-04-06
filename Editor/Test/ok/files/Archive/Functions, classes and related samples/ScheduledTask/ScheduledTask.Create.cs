function $macro [flags] [$runAs] ;;flags: 2 delete task if not scheduled to run again, 4 disabled, 0x200 hidden

 Creates scheduled task to run a macro.
 If the task already exists, at first deletes it.
 Does not set schedule and does not save the task. Use ScheduleDaily etc and Save later.
 Error if fails.

 macro - QM item name or +id. It should be function, but also can be macro or item of other type.
 runAs - as in task properties Task tab. Default: current user.


if(macro<0x10000) _i=macro; macro=_s.getmacro(_i 1)
Create2(F"QM - {macro}" "$qm$\qmcl.exe" F"T MS ''{macro}''" flags runAs)

err+ end _error
