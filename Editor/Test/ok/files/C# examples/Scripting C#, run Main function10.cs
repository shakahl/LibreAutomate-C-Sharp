/exe
PF
CsScript x.Init
x.SetOptions("references=System.Core")
PN
x.AddCode("")
PN
rep 1
	x.Main()
	PN
PO

 BEGIN PROJECT
 END PROJECT

#ret
//C# code
using System;

public class StringParsing
{
   public static void Main()
   {
      TryToParse(null);
      TryToParse("1");
      TryToParse("9.0");
      TryToParse("1,234");
      TryToParse("   -123   ");
      TryToParse("+4");
      TryToParse("(1);");
      TryToParse("01FA");
   }

   private static void TryToParse(string value)
   {
      int number;
      bool result = Int32.TryParse(value, out number);
      if (result)
      {
         Console.WriteLine("Converted '{0}' to {1}.", value, number);
      }
      else
      {
         if (value == null) value = "";
         Console.WriteLine("Attempted conversion of '{0}' failed.", value);
      }
   }
}
