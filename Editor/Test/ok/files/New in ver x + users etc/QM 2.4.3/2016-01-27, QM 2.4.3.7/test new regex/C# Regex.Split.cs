out
str s rx
s="one two three four"
rx=" "

CsScript x
x.AddCode("")

IDispatch obj=x.CreateObject("Test")
obj.RX(s rx)


#ret
using System;
using System.Text.RegularExpressions;

public class Test
{
   public void RX(string s, string rx)
   {
Regex x = new Regex(rx);
string[] a=x.Split(s);
foreach (string match in a)
{
   Console.WriteLine("'{0}'", match);
}

}
}
