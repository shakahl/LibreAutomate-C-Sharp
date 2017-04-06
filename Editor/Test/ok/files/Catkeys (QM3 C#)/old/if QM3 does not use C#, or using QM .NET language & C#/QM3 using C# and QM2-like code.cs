There are 4 item types that can contain code: QM script, QM function library, C# script, C# class library.

The QM script language is a simple CLI language, similar to QM2. Has functions, but possibly does not have classes. Create classes in C# class libraries.

SCRIPT
Scripts are executable.
Each script runs in a separate AppDomain. Or can be converted to exe.
Each AppDomain or exe can have 1 or more its private threads.
Functions, classes and namespaces defined in a script are private to the scipt (cannot be shared with other scripts).
To share global variables between domains, could be used some container class and serialization.

LIBRARY
Libraries contain functions, classes and namespaces that can be used in scripts and other libraries.
A library is compiled to a dll, which can be a temporary dll (used in scripts running in QM) or normal dll (can be used in exe).
Libraries are not executable.

 _________________________________________________________

Triggers, menus, toolbars and autotexts are defined in scripts. To make easier to define, use a preprocessor. Or use a special syntax in QM language.
Other item types - folder and file-link. Maybe also resource, table.
At startup load assemblies of scripts containing triggers etc.

 _________________________________________________________

Scripts and libraries are separate text files.
Scripts and libraries can have multiple files: menu New -> Add file to this script/library.
A script file can have multiple executable scripts (functions). Each of them can have a trigger.
The main file just contains links to these files. Also the folder structure, some settings, cached data, user-defined tables, maybe some resources.
