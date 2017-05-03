Main code is C#, but it can contain parts of QM2-like code, enclosed eg in ``.

Example:
...C# code...
`type QWERTY x --y POINT'p`
 the above code would be replaced to:
 struct QWERTY { public in x; int y; public POINT p; };
...C# code...
`key "text" Y`
 would be replaced to:
 Keys.Send("text" VK.ENTER);  or  Keys.Send("'text' Y");
...C# code...

 ________________________

`if a&b
	`out 1
	`out 2

 =>

if((a&b)!=0) {
	Out(1);
	Out(2);
}
 ________________________

`for i 0 10
	`out 1
	`out 2

 =>

for(i=0; i<10; i++) {
	Out(1);
	Out(2);
}
 ________________________

`sel x
	case 1
	`out 1
	`out 2
	case 2
	`out 2
	case else
	`out 0

 =>

switch(x) {
	case 1:
	Out(1);
	break;
	case 2:
	Out(2);
	break;
	default:
	Out(0);
	break;
}
