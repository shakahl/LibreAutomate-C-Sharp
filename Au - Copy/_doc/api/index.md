# Automation library

### Namespaces
- Au - main classes of this library, except triggers.
- Au.Types - types of function parameters and return values, exceptions, extension methods, etc.
- Au.Triggers - action triggers: hotkeys, autotext, mouse, window.
- Au.Util - miscellaneous public utility/helper functions used by this library.

### .NET Assembly files
- Au.dll - contains code of the above namespaces.

### Native dll files
- Dll/64bit/AuCpp.dll - used by Au.dll in 64-bit processes.
- Dll/32bit/AuCpp.dll - used by Au.dll in 32-bit processes.
- Dll/64bit/sqlite3.dll - used by the SQLite wrapper class in 64-bit processes.
- Dll/32bit/sqlite3.dll - used by the SQLite wrapper class in 32-bit processes.

These files are in the editor folder. When you create an .exe file from a script, they are automatically copied to the .exe folder, except sqlite3.dll.

Other dll files in the editor folder are not part of the library. They are undocumented.
