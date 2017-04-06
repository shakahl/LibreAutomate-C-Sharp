 /
function# $prog $cl [str&sout]

 Runs a console program, waits and captures its output.
 Returns its exit code.
 Obsolete. Use <help>RunConsole2</help>.

 prog - executable program filename or full path. Can be "".
 cl - command line parameters. If prog is "", must include program, eg "prog.exe parameters", where program cannot contain spaces and $special folders$.
 sout - receives text that would be displayed in console. If omitted or 0, displays in QM output.

 REMARKS
 The text may look differently because:
   errors always are at the bottom;
   user input is not included.

 EXAMPLE
 ChDir "$program files$\sdk\bin"
 out RunConsole("prog.exe" "/a aaaa /b ''bbbb''")


str s ss
if(!empty(prog))
	s.expandpath(prog)
	if(findc(s 32)>=0) s.dospath(s 2)
	if(!empty(cl)) s.formata(" %s" cl)
	cl=s

Wsh.WshShell sh._create("WScript.Shell")

Wsh.WshExec ex = sh.Exec(cl); err end ERR_FAILED
rep() if(ex.Status=0) 0.02; else break

if(!&sout) &sout=ss
sout=ex.StdOut.ReadAll; err
str serr=ex.StdErr.ReadAll; err
if(serr.len)
	if(sout.len) sout+"[]"
	sout+"ERRORS:[]"
	sout+serr

sout.replacerx("\r(?!=\n)")
sout.replacerx("(?<!\r)\n" "[]")

if(&sout=&ss) out sout

ret ex.ExitCode
