We cannot get values of caller's variables.

Instead use ScriptBase instance variables:
int _autoDelay;
Options _options; //members: bool waitMsg, slowKeys...

If a function wants to change, it must restore:
using(RestoreScriptOptions k) {
...
}
Or let it call Key etc overload that has 'options' parameter: var o=new Options(); o.speed=10; Key("...", o);

But cannot do it, because Key etc belong to other classes and they cannot access the ScriptClass instance.
	Unless we somehow store options in a memory where library functions can access. Eg in a thread variable.
	Or all QM automation library functions (or at least those that use options) are members of ScriptBase. See the last case.

_________________________

OR

Add speed etc parameters to functions that use it:
Key("Ctrl+K", speed=10, slow=true, noSync=true);

Default is -1, or use an overload. Then the functions uses the script instance speed and options.

When recording with Slow option:
bool slowKeys=true, slowMouse=0;
Key("Ctrl+K", slow=slowKeys);
Text("Ctrl+K", slow=slowKeys);
Click(10, 10, w, slow=slowMouse);

Or add just one 'options' parameter:
var o=new Options(); o.Speed=10; o.WaitMsg=true;
Key("Ctrl+K", o);
Click(50, 50, w, o);

_________________________

OR

var m=Macro();
m.Speed=10;
m.Run("ccc.exe");
m.SendKeys("Ctrl+K");

_________________________

OR

All QM automation library functions (or at least those that use options) are members of ScriptBase.

Option.Speed=100; Option.WaitMsg=true;
Key("Ctrl+K");
Click(50, 50, w);

Option is a field or property of ScriptBase.
Key, Click etc are members of ScriptBase, therefore can access Option.

But what if user wants to call Key etc in other classes? Will need to make ScriptBase their base. And it will be other ScriptBase instance. And cannot use in static functions. And cannot use in class instance functions eg Acc.Mouse. Not good.

_________________________

OR

Add optional/overload speed parameters to functions that use it. Because often used. Can be even thread variable.
For options use Option class.

Key("Ctrl+K", 0.01);

int speed=10;
Key("Ctrl+K", speed);
Key("Ctrl+K", speed);

Then, when recording with Real speed option:
key Ck; 1.5
->
Key("Ctrl+K", 1.5);


Then, when recording with Variable speed option:
double F=1.0;
Key("Ctrl+K", 1.5*F);

Not good. Speed and 'press keys etc and wait n s' are two different things, because speed is used not only AFTER command.
But maybe it's OK. Because if specified to wait eg 1 s after command, then inside command we can use quite big autodelay.

For options:
var o=new Options(); o.slowKeys=true;
Key("Ctrl+K", 1.5);

_________________________

OR

Let functions that use speed/options also have classes where you can set speed etc.

var k=new KeyClass(); k.speed=10; k.slowKeys=true;
k.Key("Ctrl+K");

But it is almost the same as passing options parameter:
var o=new Options(); o.Speed=10; o.WaitMsg=true;
Key("Ctrl+K", o);

_________________________

OR

class ScriptBase
{
ScriptOptions _opt=new ScriptOptions();
}

_opt.speed=10; _opt.slowKeys=true;
Key("Ctrl+L", _opt);

//When don't want or cannot use/modify _opt (eg in a library function), use local variable:
var o=new ScriptOptions(); o.speed=10; o.slowKeys=true;
Key("Ctrl+L", o);
//or
Key("Ctrl+L", new ScriptOptions(){speed=10, slowKeys=true});

This is better than:
Key("Ctrl+L", 10, KeyFlags.slow);

_________________________

OR

Use 2 classes: LocalOptions and ThreadOptions.
LocalOptions contains all options.
ThreadOptions contains options that, when script sets, libraries also use them in most cases. For example, speed and slowKeys; but not hidden, waitcpu etc. In other words, only those that would not cause the script to fail when changed.
Let ScriptBase has a thread variable ThreadOptions _opt.

Or don't use LocalOptions. Instead add parameters to each function.

