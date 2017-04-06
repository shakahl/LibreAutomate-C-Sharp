 This does not work if the component is not registered. When calling DllGetClassObject, it looks for typelib in registry, and fails when does not find.

typelib Project1 "Q:\Projects\from_notebook\VbActiveXdll\VbDll.dll"
 Project1.Class1 c._create
Project1.Class1 c=CreateComObjectUnregistered("Q:\Projects\from_notebook\VbActiveXdll\VbDll.dll" uuidof(Project1.Class1) uuidof(Project1._Class1))

out c.DoubleArg(3)

