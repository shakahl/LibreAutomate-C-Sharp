out

interface# ITest :IDispatch
	[g]#m
	[p]m(x)
	[g]IDispatch't
	[p]t(IDispatch'x)
	CreateTest()
	UseTest(IDispatch'a)
	{C7D9B459-0108-4F4A-9483-A7F60F18AF0A}

CsScript x.AddCode("")
ITest t=x.CreateObject("Test")
t.m=5
out t.m
t.CreateTest
IDispatch d=t.t
d.m=6
out d.m
t.UseTest(d)

#ret
using System;
using System.Runtime.InteropServices;

[Guid("C7D9B459-0108-4F4A-9483-A7F60F18AF0A")]
public interface ITest
{
int m {get;set;}
Test t {get;set;}
void CreateTest();
void UseTest(Test a);
}

public class Test :ITest
{
public int m {get;set;}
public Test t {get;set;}
public void CreateTest() { t=new Test(); }
public void UseTest(Test a) { Console.Write(a.m+10); }
}
