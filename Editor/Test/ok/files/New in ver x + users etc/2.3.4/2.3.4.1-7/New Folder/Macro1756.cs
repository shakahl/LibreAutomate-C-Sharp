double Amount

 Amount=0.06
Amount=0.03+0.02+0.01

out
out "Amount: %.5G" Amount
out "Amount: %.20G" Amount ;;0.060000000000000005

if Amount=0.06;; <<<<<<<<<< ERROR <<<<<<<<<<
	out "OK" 
else out "ERROR!"

Amount=Round(Amount 2)

if Amount=0.06
	out "OK" 
else out "ERROR!"