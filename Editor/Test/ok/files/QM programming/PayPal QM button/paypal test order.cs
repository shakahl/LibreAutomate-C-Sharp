 This probably will not work because several things changed.
 Now instead can be used "Instant Payment Notification (IPN) simulator":
   Login to https://developer.paypal.com/ using normal PayPal account credentials.
   Click Dashboard. The simulator link is at the left.


str licenseTo
int quantity

 licenseTo="licKKK"
 quantity=5
licenseTo="A ąčę ɞɲʊ"

 __________________________

act "Firefox"
 g1
int w=win("Untitled Document - Mozilla Firefox" "Mozilla*WindowClass" "" 0x804)
if(!w)
	run "$qm$\web\test\paypal-test-sandbox-button.html"
	2; goto g1
 mes- "Continue?" "" "OC"

Acc a.FindFF(w "INPUT" "" "alt=PayPal - The safer, easier way to pay online![]name=submit" 0x1004 10)
Acc a1.FindFF(w "INPUT" "" "name=os0" 0x1004 2)
a1.Select(1); key CaX
if(!empty(licenseTo)) key (licenseTo)
 ret
a.Mouse(1)
w=wait(60 WV win("Pay with a PayPal account - PayPal - Mozilla Firefox" "Mozilla*WindowClass" "" 0x804))
a.FindFF(w "INPUT" "" "name=q0" 0x1004 20)
if quantity>1
	a.Select(1)
	_s=quantity; key Ca (_s)
 ret
a.FindFF(w "INPUT" "" "name=login_email" 0x1004 20)
a.Select(1); key Ca "test_1342245219_per@quickmacros.com"
 mes- "Continue?" "" "OC"
a.FindFF(w "INPUT" "" "name=login_password" 0x1004 2)
a.Select(1); key "342245168"
a.FindFF(w "INPUT" "" "id=submitLogin" 0x1004 2)
a.Mouse(1)
 ret
w=wait(60 WV win("PayPal Electronic Communications Delivery Policy Consent - Mozilla Firefox" "Mozilla*WindowClass" "" 0x804))
a.FindFF(w "INPUT" "" "name=esign_opt_in[]id=esignOpt" 0x1004 2)
a.Mouse(1)
a.FindFF(w "INPUT" "" "value=Agree and Continue[]name=agree_submit.x[]id=agree" 0x1004 2)
a.Mouse(1)
w=wait(10 WV win("Review your information - Mozilla Firefox" "Mozilla*WindowClass" "" 0x804))
a.FindFF(w "INPUT" "" "value=Pay Now[]name=continue[]id=continue_abovefold" 0x1004 2)
a.Mouse(1)
