out

CsScript x.AddCode("")
IDispatch t=x.CreateObject("Test")

 IDispatch k=t.k ;;error
ARRAY(str) b=t.GetListAsArray ;;OK
for(_i 0 b.len) out b[_i]

IDispatch a=t.a ;;OK
out a.Count
out a.Item(0)
a.Item(0)="newstring"

#ret
using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;

public class Test
{
public List<string> k { get; private set;}
public ArrayList a { get; private set;}

public Test()
{
k=new List<string>(); k.Add("string in List");
a=new ArrayList(); a.Add("string in ArrayList");
}

public string[] GetListAsArray() { return k.ToArray(); }
}
