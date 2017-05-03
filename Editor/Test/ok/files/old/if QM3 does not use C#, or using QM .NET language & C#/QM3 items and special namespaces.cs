 Two QM item types - script and module.
 Scripts are executable, modules not.
 Scripts and modules can declare anything: namespaces, classes, functions, dll, interfaces, types, enums, global variables, libraries.
 Everything that is declared in a script is private to it. Then will not need to open and parse all the garbage scripts.
 In modules can be declared public and private things.

 Use two special namespaces - System (or qm) and User (or my). They can be used anywhere (except User ns in System folder) without
 Everything that is in System modules without namespace, is added to the System/qm namespace. Its identifiers can be used anywhere, without 'using System' or System.Identifier.
 Everything that is in non-System modules without namespace, is added to the User/my namespace. Its identifiers can be used anywhere except in System folder, without 'using User' etc.
 Other (maybe only user-defined) namespaces must be explicitly specified in scripts that use them, eg 'using MyNamespace;'. Also add an option to declare and implicit namespace, like System.
 Namespaces and classes can span multiple modules. Users can extend existing namespaces (including System namespaces) and classes by adding functions etc in any module.
 To keep track of all this, use caching. When compiling a script, parse all scripts whose symbols are still not cached or the script is modified after caching. Also when displaying in code editor or object browser.

 Triggers also are specified in scripts, therefore need to parse all.
   Then maybe use single QM item type that can contain private and public stuff.
