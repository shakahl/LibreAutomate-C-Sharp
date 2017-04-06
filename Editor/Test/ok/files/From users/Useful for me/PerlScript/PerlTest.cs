 This basically sends trivial statements to each of the Perl functions to make sure things are running properly.

mes "testing PerlAddCode"
PerlAddCode "my $v;"

mes "testing PerlExec"
PerlExec "$v = 8;"

mes "testing PerlEval. 16 should be printed in QM output if AddCode, Exec, and Eval worked."
out PerlEval("$v*2;")

mes "testing PerlExec2"
PerlExec2 ("print qq~hello\n\nThis window will disappear in a moment.\n~; sleep(1);");
1.25

mes "testing PerlError, an error should be reported."
out PerlEval("print 'buggycode;")

out "PerlTest is complete!"