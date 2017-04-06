int test=1
str s rx
sel test
	case 0
	s="one -12 two"
	rx="\d+"
	
	case 1
	s.getmacro("ShowDialog")
	rx="[\-!~]([k-z]+|\d+)\b"
	
	case 2
	s.getmacro("TO_Window")
	rx="(?m)^[ ;]*BEGIN DIALOG *[][ ;]*((?:.+[])+)[ ;]*END DIALOG *(?:[])?(?:[ ;]*DIALOG EDITOR: +([^[]]+)(?:[])?)?"

WakeCPU

CsScript x
x.AddCode("")

IDispatch obj=x.CreateObject("Test")
PF
rep 3
	int i=obj.RX(s rx)
	PN
PO
out i

 findrx:
 speed: 59  19  18  
 .NET
 speed: 260  220  215  
 speed: 225  198  196  static (witch caching)


#ret
using System;
using System.Text.RegularExpressions;

public class Test
{
   public bool RX(string s, string rx)
   {
      //return true;
  	  // Regex x = new Regex(rx);
  	  // return x.IsMatch(s);

		//return Regex.IsMatch(s, rx);
		
		int i; bool r=false;
		for(i=0; i<1; i++)
		{
			r=Regex.IsMatch(s, rx);
			 //Regex x = new Regex(rx);
  	    	 //r=x.IsMatch(s);
		}
		return r;
   }
}
