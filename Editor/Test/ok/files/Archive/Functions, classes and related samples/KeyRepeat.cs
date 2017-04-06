 /
function vk ^duration [speed]

 Repeatedly presses a key.

 vk - virtual-key code. See QM Help, "virtual-key codes" topic.
 duration - number seconds to hold key down.
 speed - number of milliseconds between repeated keypresses. Default 50.

 EXAMPLES
 KeyRepeat VK_SPACE 5 ;;spacebar, 5 seconds
 mac "KeyRepeat" "" VK_SPACE 5 ;;the same, but continue macro immediately
 KeyRepeat 'A' 0.5 1 ;;key A, 0.5 seconds, fastest


duration*1000
if(speed<1) speed=50
spe speed

int t1(GetTickCount) t2

rep
	key+ (vk)
	if(duration<0) continue
	t2=GetTickCount
	if(t2>t1+duration or t2<t1) break

spe -1
key- (vk)
