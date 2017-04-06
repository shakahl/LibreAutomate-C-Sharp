COLLECTION

The top-level entity is "collection".
It is a folder that contains script files, a collection file and maybe more files.
The collection file - list of scripts and their main properties in CSV format:
	name,flags,level,guid,td
		flags: folder, external, disabled.
		level: level in the folder structure. In CSV empty if same as previous.
		guid: Base64.
		td (type data): folder - description, scope, icon; external - path; other - icon path. Can be XML if need multiple properties, eg <x><text>...</text><scope if="yes"><p>notepad</p><w>...
		


SOURCE

Each script is in separate file. Can be in subfolders.
Triggers and many other properties are specified in scripts as [attributes].
TODO: how to specify programs (scope) for folders?
	Can be XML in collection -> td.
	Or special [attributes] in first script.
	Or use special file eg named "folder.properties" (a C# file containing [attributes]).


RESOURCES
Each resource is in a separate file.
Collection resources are in its folder.
Script resources also are in its folder. Users can explicitly place a script in a separate folder together with its resources.
Resource files can have a flag "resource", which means "add to assembly resources, and in exe use the resource". Or use a special path syntax, like the old ":resid path" syntax in QM2.
Click-recorded images - embedded in script (compressed + Base64).
Captured FindImage images - either in a special subfolder or embedded in script.


PRIVATE (auto-created files)

When compiling a script, attributes are parsed and their data saved in separate files (tables) for quick access:
Trigger tables.

Other private files:
Expanded folders (list of GUID) and open items  (list of GUID). Used to avoid frequent saving of the collection file.
Toolbar positions/settings.
Find dialog saved items.
Find text cache.
Tags.
Other settings.

Private files are created in collection's subfolder "$private". If folder is read-only - in user data folder.
Or can write all or parts of this to SQLite database file(s).

