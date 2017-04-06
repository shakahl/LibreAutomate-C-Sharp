function'IDispatch $className

 Creates an object (class instance).
 Returns IDispatch interface that can be used to call public non-static functions.

 className - full class name, including namespace name if need.

 REMARKS
 Use <help>CsScript.AddCode</help> to add code that contains the class.
 If the class is in a namespace, className must include namespace name, like "Namespace1.Class1".
 The class must be public.
 If the class has a constructor, it must have a public constructor without arguments.

 See also: <CsScript example - create object>, <CsScript example - use interface>.


opt noerrorshere 1

if(!x) end ERR_INIT

ret x.CreateObject(className)
