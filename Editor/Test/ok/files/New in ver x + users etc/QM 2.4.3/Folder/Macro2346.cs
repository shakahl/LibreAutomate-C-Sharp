out
CsScript x.AddCode("")

 DECIMAL R=x.Call("f_decimal" 10.5)
str sg=x.Call("f_guid")
out sg

 IDispatch t=x.CreateObject("Test")
 out t.RetInt
 DECIMAL R=t.RetDecimal
 out R

interface# ITest :IDispatch
	#RetInt()
	DECIMAL'RetDecimal()
	@RetBool
	ARRAY(BSTR)RetStringArr

ITest t=x.CreateObject("Test")
out t.RetInt
DECIMAL R=t.RetDecimal
out R
out t.RetBool
ARRAY(BSTR) a=t.RetStringArr
out "%i %i" a.len a.psa


#ret
using System;
using System.Runtime.InteropServices;

[Guid("C7D9B459-0108-4F4A-9483-A7F60F18AF0A")]
public interface ITest
{
int RetInt();
Decimal RetDecimal();
bool RetBool();
String[] RetStringArr();
}

public class Test :ITest
{
public static decimal f_decimal(decimal b) { Console.Write(b); return b; }
public static String f_guid() { return typeof(ITest).GUID.ToString(); }

public int RetInt() { return 3; }
public Decimal RetDecimal() { return 4.5m; }
public bool RetBool() { Console.Write(sizeof(bool)); return true; }
public String[] RetStringArr() { return new String[2]; }
}
