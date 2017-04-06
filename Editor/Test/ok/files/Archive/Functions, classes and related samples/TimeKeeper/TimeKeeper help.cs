 Measures time spent in macro (or part of macro), and writes to a log file.

 EXAMPLE

#compile "__TimeKeeper"
 TimeKeeper x.Begin("$desktop$\test TimeKeeper.txt" "" "test" 1|2) ;;use file
TimeKeeper x.Begin("" "" "test" 1|2) ;;use QM output

mes "macro"

x.End("success")
