 activate IE, and set focus to the web content control
int w1=child("" "Internet Explorer_Server" win("Internet Explorer" "IEFrame"))
act w1
 if mouse is not in it, move into it
if(child(mouse) != w1) mou 10 10 w1 ; int mouseMoved=1
 Ctrl + scroll
key+ C
MouseWheel 3
key- C
 restore mouse
if(mouseMoved) mou
