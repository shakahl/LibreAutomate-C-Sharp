out

CsScript x.AddCode("")
IDispatch t=x.CreateObject("Test")
t.m=5
out t.m
t.CreateTest
IDispatch d=t.t
d.m=6
out d.m
t.UseTest(d)

#ret
using System;

public class Test
{
public int m {get;set;}
public Test t {get;set;}
public void CreateTest() { t=new Test(); }
public void UseTest(Test a) { Console.Write(a.m+10); }
}
