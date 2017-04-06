Scripts can be single-file or multi-file (project).
Each single-file or project script is compiled to a separate assembly.
For each first-time-started assembly is created appdomain, and the assembly is loaded there.
The appdomain and assembly are not shutdown/unloaded while using the same version of the script.
When the script is modified, the appdomain is shutdown and the assembly file deleted.
An assembly can have one or more script entry functions. They are non-static. They can have triggers. When starting the script not by one of these triggers, is called Main().
Each time we start a script entry function, a new ScriptClass object is created and the function called. It runs in new thread.
Non-static ScriptClass fields can be used to share variables between public/private functions of that running script instance.
Static ScriptClass fields can be used to share variables between multiple running instances of that script, even if they run non-simultaneously, because the appdomain/assembly is not unloaded.
