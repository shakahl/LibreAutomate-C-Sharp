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
using System.Collections.Generic;

using System.Text;

namespace Utils
{
    public class MD5Agent
    {
static void Main() { Console.WriteLine(Encode("test")); }

        public static string Encode(string pass)
        {
        System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(pass);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs )
            {
                s.Append(b.ToString("x2").ToLower());
            }
            pass = s.ToString();
            return pass  ;
        }
    }
}