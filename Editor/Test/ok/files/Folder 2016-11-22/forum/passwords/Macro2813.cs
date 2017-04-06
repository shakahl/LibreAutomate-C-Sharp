 act "Notepad" ;;yes
 act "Firefox" ;;yes
act "Internet Explorer" ;;no


spe 20

str garbage = "$3LtK9mkXb3H9Z;ALjLAI@SGXq5`_/K?P5pF<YdRg:j=w''h6E,+;%aKM?Ede8rd)TaB8^ziZAc^4Bv,U3*<(BwLGy?68^H-JxMsVCY'eZH/?1h&Jei1yiXrp9,wiZ8::I9+^UmH<r61f?C@pR67nPuJ2o\\;3!dm^Z8jdB(93a0i/Pw[v+zTQrO[n(Be[68J]O>("
str pw ="thisismyrealpassword"
str s
int x
int y

rep 5
	wait 0 ML
	s.get(garbage x 10)
	key (s)
	x+10
	
	wait 0 ML
	s.get(pw y 4)
	key (s)
	y+4
	
	s.get(garbage x 5)
	key (s)
	x+5
	
	wait 0 ML
	'B
