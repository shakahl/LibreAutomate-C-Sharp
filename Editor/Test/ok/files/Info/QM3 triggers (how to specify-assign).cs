#region begin_script
...
#endregion

Trigger.Hotkey["Ctrl+k"] = Func1; //class Trigger, static readonly variable or property Hotkey (of another class eg Hotkeys, using its indexer); assigns a delegate
Trigger.Hotkey["Ctrl+k"] = delegate(Trigger.HotkeyData t) { Out("test"); };
Trigger.Hotkey["Ctrl+k"] = t => Out("test");
Trigger.Window["Name", "Class"] = t => { Out("test"); };
//Trigger.Hotkey("Ctrl+k") = Func1; //error, because C# does not support properties with parameters, and does not support 'return ref'
}

static void Func1(Trigger.HotkeyData t)
{
}

//OR

[Trigger.Hotkey("Ctrl+k")]
static void Func1(Trigger.HotkeyData t)
{
}

[Trigger.Hotkey("Ctrl+Alt+k")]
[Trigger.Hotkey("Ctrl+Shift+k")] //multiple triggers
static void Func2(Trigger.HotkeyData t)
{
}

//When QM compiles a script, it gets attributes from the assembly (through reflection) and saves trigger definitions in a cache.
//Then QM gets trigger definitions from cache and creates trigger tables.
//Also add option to activate script's triggers only when the script is launched: [assembly: TriggerOptions....] and maybe for each trigger separately. Or for this use triggers defined in code, not attributes.
//Scrips can be compiled manually or automatically.
//Non-compiled scripts (eg containing errors) are displayed eg with red text in the list. Their triggers don't work.
//A script that runs as separate .exe process can get trigger definitions from its assembly and create trigger tables in that process. Or for this use only triggers defined in code, not attributes.

//By default, every script runs in a separate appdomain. Also can run in a separate process or exe.
//Add option (or auto, if has triggers etc) to not exit the appdomain. Then next time, when the script launched or a trigger raised, its main function (or the function that has the trigger) runs in the same appdomain. Benefits: faster startup, retains values of static variables, etc.

}
