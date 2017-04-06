 /Macro1573
function int'mm str'tn str'tr

 EXAMPLE
 ScheduleOnce 1 "Test" "Function3"

  Adds number of minutes from now to schedule task
DATE d.getclock
d=d+(6.9444444444444444444444444444444e-4*mm)

  Deletes old task if couldn't run. 
  Create task will fail if task with same name exists
str Del=
F
 schtasks
 /Delete
 /TN {tn}
 /F
Del.findreplace("[]" " ")
Del.trim
RunConsole2(Del str'out1)
out out1

  Defines time to run for command line formatting
str st.timeformat("{HH}:{mm}:{ss}" d)
str sd.timeformat("{yyyy}/{MM}/{dd}" d)

  Formats schtasks command line
_s.searchpath("qmcl.exe")
str cl=
F
 schtasks
 /create
 /tn {tn}
 /tr "\"{_s}\" T MS \"{tr}\""
 /sc once
 /st {st}
 /sd {sd}
cl.findreplace("[]" " ")
cl.trim

RunConsole2(cl str'sout)
out sout
