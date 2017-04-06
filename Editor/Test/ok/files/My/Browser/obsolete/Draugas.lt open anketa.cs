Acc a=acc(mouse)

  When you click a member link in draugas.lt, it asks to sign in.
  To open the anketa without signing in, place mouse over the link and run this macro. Opens in new tab.
  However it does not work in search results, because these links are directly to the sign in page, not to the anketa page.
  Updated: now don't need this.
 
 str s=a.Value
 if !s.beg("http")
	 s.gett(s 0 "&")
	  out s
	 run s
	 ret

 ____________________________

 Place mouse on a member image in draugas search results and run this macro.
 Does not work if there is no image.
 Also works in srautas.

 get src of image from mouse
FFNode f.FromAcc(a); err ret
 may be IMG (url in src) or A (url in style)
str s=f.Attribute("src")
if(!s.len) s=f.Attribute("style") ;;background image of A
 out s

 extract member id
str sid
if(findrx(s "/(\d+)\w+\." 0 1 sid 1)<0) ret

 open member's page
run F"http://pazintys.draugas.lt/narys.cfm?narys={sid}"
