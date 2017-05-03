 QM
Acc a.Find(""); err ret

 C#
var a=Acc.Find(""); if(a==null) return; //or TryFind
var a=Acc.FindOrError(""); //error if not found. It is OK and don't need to handle. Or don't need a throwing function, because will throw null exception when calling a member.

 TryFind does not throw when object not found. Other reasons to throw are only invalid window handle or other paramatars, and it should be fixed or handled before.

 From guidelines:
 DO NOT return error codes. Use exceptions.
 DO NOT have public members that can either throw or not based on some option.
 CONSIDER 'Tested-Doer' pattern (if(x.CanDoIt()) x.DoIt()) or 'Try-Parse' pattern (if(!x.TryDoIt()) ...).
 Other guidelines are obvious.

 REJECTED

 C#
Acc a=null; try{ a=Acc.Find(""); }catch{}
 or
var a=new Acc(); try{ a.Find(""); }catch{}


 C#
LongClassName a=null; try{ a=LongClassName.Find(""); }catch{}
 or
var a=new LongClassName(); try{ a.Find(""); }catch{}
