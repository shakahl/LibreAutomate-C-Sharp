 Output after I run this code:
   Error (RT) in Macro1483:  this interface is not a collection, or it cannot be used with foreach.    ? 
   Exception in destructors of local variables of Macro1483.
 Then QM stops responding for several seconds. During that time, hard disk activity.
 Then works.
 Then I exit QM. It hides QM, but it does not exit. To run it again, I have to kill it in task manager.


out
int w1=child("" "Internet Explorer_Server" win("" "IEFrame"))
MSHTML.IHTMLDocument2 d=htm(w1)

IOleContainer oc=+d
IUnknown u
foreach u oc
	out u
