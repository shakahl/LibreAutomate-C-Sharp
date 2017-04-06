int w1=win("My QM" "CabinetWClass")

 scan "$myqm$" w1 0 3 2 12

 wait 0 S "$myqm$" w1 0 3|0x400 2 12
wait 0 S "$myqm$" w1 0 3|0x500 2 12

1; mou
