 Run this.

 _______________________

 variables
type OSKKEY str'name x y !vk !pressed
type OSKVAR ARRAY(OSKKEY)'a __Font'font __MemBmp'mb hwnd color tr
OSKVAR- v

 _______________________

 You can edit these values.

 keyboard position and size
int x(ScreenWidth/2-170) y(ScreenHeight-130) cx(330) cy(90)

 other parameters
v.color=0xe000 ;;pressed key color
v.tr=500 ;;how many milliseconds to highlight the pressed key after it is released

 Define keys.
 Can be only character keys.
 Every line begins with a character.
 Then optionally follow x and y offsets.
 If x and y missing, draws to the right from the previous key.
lpstr keys=
 q 0 0
 w
 e
 r
 t
 y
 u
 i
 o
 p
 a 10 30
 s
 d
 f
 g
 h
 j
 k
 l
 ;
 z 20 60
 x
 c
 v
 b
 n
 m
 ,
 .
 /

 _______________________

 init variables
str s; lpstr sk sx sy; int kx ky
foreach s keys
	OSKKEY& k=v.a[]
	sel tok(s &sk 3 " " 1)
		case 3 kx=val(sx); ky=val(sy)
		case else kx+30
	k.x=kx; k.y=ky
	k.name=sk
	k.vk=VkKeyScan(sk[0])&0xff

v.font.Create("Courier New" 20 1)
v.mb.Create(cx cy)

 display keyboard
v.hwnd=OnScreenDraw(x y cx cy &OSK_OsdProc)

 intercept keys
int hhook=SetWindowsHookEx(WH_KEYBOARD_LL &OSK_KeyProc _hinst 0)
MessageLoop
