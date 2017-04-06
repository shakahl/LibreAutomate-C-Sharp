/exe
PF
CsScript x.Init
x.SetOptions("references=System.Core")
PN
x.AddCode("")
PN
rep 1
	x.Call("Program.Main")
	PN
PO

 BEGIN PROJECT
 main_function  Scripting C#, run Main function1
 exe_file  $my qm$\Scripting C#, run Main function1.qmm
 flags  6
 guid  {D31D9EA8-3655-4F24-BCB4-1712A9D99FE6}
 END PROJECT

#ret
//C# code
    /*

     * C# Program to Divide Sequence into Groups using LINQ

     */

    using System;

    using System.Linq;

    using System.IO;
using System.Runtime.CompilerServices;

    public class Program

    {
[MethodImpl(MethodImplOptions.NoOptimization)] 
        public static void Main()  

        {

            var seq = Enumerable.Range(100, 100).Select(x => x / 10f);

            var grps = from x in seq.Select((i, j) => new { i, Grp = j / 10 })

                       group x.i by x.Grp into y

                       select new { Min = y.Min(), Max = y.Max() };

            foreach (var grp in grps)

                Console.WriteLine("Min: " + grp.Min + " Max:" + grp.Max);


        }

    }