str List

List=
 *** FirstLine ***
 John: sells 
 Tony: sells 
 Mike: pays1 $0.02
 Rose: sells 
 Alex: sells 
 Mary: pays1 $0.01
 Doug: sells2 
 *** LastLine ***

int FirstLine LastLine

FirstLine=0
LastLine=8

double Amount

Amount=0.03

Calculate FirstLine LastLine List Amount
 Amount=0.06

out
out "Amount: %.5G" Amount

if Amount=0.06;; <<<<<<<<<< ERROR <<<<<<<<<<
	out "OK" 
else out "ERROR!"