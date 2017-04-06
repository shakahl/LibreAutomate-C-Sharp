Use multiple source files. They are text files.
Binary data can be in separate files. Some can be in script as Base64, especially if it is private to that script or source file.
Each source file can contain multiple scripts and modules.
In treeview the files can be anywhere, not only at root. Their scipts are children of the files.
The main file is a Sqlite database. It contains the treeview structure, caches, settings, user-defined tables, etc.
When a source file becomes very big, show a warning.

Source files can be local or URL.
Maybe allow to specify filePath/URL in script:
package "http:..." ;;auto download a source file
using Namespace1 ;;use a namespace from the source file

TODO: test Git etc.
