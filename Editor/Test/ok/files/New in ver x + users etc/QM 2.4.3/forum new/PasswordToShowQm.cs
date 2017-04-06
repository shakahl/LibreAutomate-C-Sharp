EnableWindow _hwndqm 0
Transparent _hwndqm 0
int show=inpp("x" "Password to show QM")
Transparent _hwndqm 256
EnableWindow _hwndqm 1
if(show) act _hwndqm
else clo _hwndqm
