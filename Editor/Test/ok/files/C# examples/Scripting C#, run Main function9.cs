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
using System.Numerics;

public class Example
{
   public static void Main()
   {

        BigInteger n1 = BigInteger.Pow(9999999999, 3);
        BigInteger n2 = BigInteger.Multiply(9999999999, 99999999);
        try
        {
           Console.WriteLine("The greatest common divisor of {0} and {1} is {2}.",
                             n1, n2, BigInteger.GreatestCommonDivisor(n1, n2));
        }
        catch (ArgumentOutOfRangeException e)
        {
           Console.WriteLine("Unable to calculate the greatest common divisor:");
           Console.WriteLine("   {0} is an invalid value for {1}",
                             e.ActualValue, e.ParamName);
        }
   }
}