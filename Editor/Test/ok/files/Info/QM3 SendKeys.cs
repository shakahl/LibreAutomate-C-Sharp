 QM2

key YT(#3) "aaa" Y Cc CA{fa} 0.5 F2 (103)

 QM3
 __________________________________________________________

 SELECTED
 __________________________________________________________

 Use two functions. Both have variable number of string parameters. In each pair of parameters, the first is interpreted as keys and the second as text, and vice versa.

 void Key(params string[] keys_text_keys_text_andSoOn)
Key("keys", "text", "keys", "text", ...);
 void Text(params string[] text_keys_text_keys_andSoOn)
Text("text", "keys", "text", "keys", ...);
 or Keyt()

 Examples:
Key("Tab Enter");
Text("text");
Key("Enter Tab*3", "aaa", "Enter Ctrl+Alt+c Alt+f+a WAIT(0.5) F2 VK(103)");
Text(s1, "Tab", s2, "Tab", s3, "Enter");

 ____________________________

 String key names (case insensitive)
Alt
Back or Backspace or BS
CapsLock
Ctrl
Del or Delete
Down
End
Enter
Esc or Escape
F1-F24
Home
Ins or Insert
Left
Menu or App
Num0-Num9 or N0-N9
NumDiv NumMult NumSub NumAdd NumDot NumEnter
NumLock
PageDown or PgDn
PageUp or PgUp
Pause
PrintScreen or PrtSc, PrtScrn
Right
RightAlt or AltGr
RightCtrl
RightShift
RightWin
ScrollLock
Shift
Space
Tab
Up
Win
_BrowserBack etc
_Sleep
 Other keys
A-Z - alphabet keys. Case insensitive (can be a-z).
1-0 - number keys (the row below F keys).
` - = [ ] ; ' , . / \ - other text keys.
 Mouse buttons (only for KeyIsPressed/Toggled)
MouseLeft etc
 Functions in the keys string (case insensitive)
DOWN(keys) - press key down and don't release now. Can be several keys.
UP(keys) - just release, don't press down. Example: Key("DOWN(Ctrl Alt) f s UP(Alt Ctrl)"); is the same as Key("Ctrl+Alt+f+s");
UP() - release all pressed keys.
 problem: Down and Up also are keys. Could be PRESS and RELEASE.
 better use this:
 "Ctrl+Shift+A+B" - simultaneously presses 4 keys (or only modifiers). Releases all when there are no more +.
 "Ctrl+Shift+(" - use ")" to release all.
 "Ctrl+Shift+" - auto-releases all after next key/mouse send event.
 when need more explicit control: Ctrl*down ... Ctrl*up. Also can be Tab*5*down (5*down, then up).
 Compare:
 Key("PRESS(Ctrl Alt)", ..., "RELEASE(Alt Ctrl)");
 Key("Ctrl+Alt+(", ..., ")");
 Key("Ctrl*down Alt*down", ..., "Alt*up Ctrl*up"); - explicit release, even if not pressed by Key or user etc. And maybe, if not released explicitly, don't auto-release when script ends.
 Example with mouse: "Ctrl+(", Mouse.Click(), ")", or just "Ctrl+", Mouse.Click(). Let Mouse.Click() return null string.
 
KEY(vk, sc=0, flags=0) - virtual-key code, optional scan code and optional flags: 1 - extended key. Commas optional. Can be specified only virtual-key code or only scan code, and other arguments omitted or 0, then they will be calculated. Examples: Key($"KEY(10 20 1) KEY({VK_CLEAR})");
CHAR(n) - Unicode character code. Sends in VK_PACKET.
WAIT(n) - wait n seconds. Example: Key("Alt+F+S WAIT(0.5) Enter");
 Operators/expressions in the keys string
mod+key or mod+key+key... or mod+mod...+key - hotkey. Examples: Key("Ctrl+C Alt+F+S Ctrl+Shift+Win+K");
key*n - repeat, eg Tab*3. Can repeat functions too, eg DOWN(A)*10. Cannot repeat whole expressions; for example Ctrl+A*5 repeats just A, not Ctrl+A.

 All keys, functions and expressions must be separated by one or more blank characters (spaces or newlines). For example, "ABC" is invalid, must be "A B C".

 Also could add TOGGLE(), but maybe better make it a separate method.
TOGGLE(keys, on|off) - toggle a "Lock" key.

 __________________________________________________________

 OTHER POSSIBLE
 __________________________________________________________

 QM2 key codes

 String
Key("YT(#3) 'aaa' Y CAc C{fa} 0.5 F2 (103)");

 Cascaded methods for keys/text/wait/etc parts
Key("YT(#3)").Text("aaa").Key("Y CAc C{fa}").Wait(0.5).Key("F2 (103)").Send();

 __________________________________________________________

 Key constants

Key(K.ENTER, K.TAB, Times(3), "aaa", K.ENTER, Ctrl(Alt('c')), Alt('f', 'a'), Wait(0.5), K.F2, 103);
Key(K.ENTER, Repeat(K.TAB, 3), "aaa", K.ENTER, Ctrl(Alt('c')), Alt('f', 'a'), Wait(0.5), K.F2, 103);
 __________________________________________________________

 Cascaded methods

 For each key and other part
Keys.Enter.Tab.Repeat(3).Text("aaa").Enter.Ctrl.Alt.C.AltDown.F.A.AltUp.Wait(0.5).F2.VK(103).Send();
var k=Keys.Enter.Tab.Repeat(3).Text("aaa").Enter.Ctrl.Alt.C.AltDown.F.A.AltUp.Wait(0.5).F2.VK(103); k.Send();
Key(Keys.Enter.Tab.Repeat(3).Text("aaa").Enter.Ctrl.Alt.C.AltDown.F.A.AltUp.Wait(0.5).F2.VK(103));

 For each key, but use separate arguments for keys/text/wait/etc parts
Key(K.Enter.Tab.Repeat(3), "aaa", K.Enter.Ctrl.Alt.C.AltDown.F.A.AltUp.Wait(0.5).F2.VK(103));
Key(K.Enter.Tab.Repeat(3), "aaa", K.Enter.Ctrl.Alt.C.AltDown.F.A.AltUp, 0.5, K.F2.VK(103));
 __________________________________________________________

 String

Keys("Enter Tab*3 'aaa' Enter Ctrl+c Alt+f+a WAIT(0.5) F2 VK(103)");

 Optionally short key names. Better don't, as it is unreadable and does not allow to use uppercase letter key names.
Keys("E T*3 'aaa' E C+A+c A+f+a WAIT(0.5) F2 VK(103)");

 Prefer text
Keys("text"); //just text
Keys("text [Enter]"); //text and keys
Keys("[Enter Tab*3] aaa [Enter Ctrl+Alt+c Alt+f+a WAIT(0.5) F2 VK(103)]");

 Can be separate arguments for keys/text/wait/etc parts. Then simple text parts cannot contain [keys].
Keys(K("Enter Tab*3"), "aaa", K("Enter Ctrl+Alt+c Alt+f+a"), 0.5, K("F2 VK(103)"));
Keys("Enter Tab*3".AsKeys(), "aaa", "Enter Ctrl+Alt+c Alt+f+a".AsKeys(), 0.5, "F2 VK(103)".AsKeys());
//Keys((K)"Enter Tab*3", ...); //not possible
KeysAsText("text [also text]");
Keys(K("Enter Tab*3"), T("aaa [also text]"), K("Enter Ctrl+Alt+c Alt+f+a"), 0.5, K("F2 VK(103)"));
Keys(K("Enter Tab*3"), T("aaa [also text]"), K("Enter Ctrl+Alt+c Alt+f+a Wait(0.5) F2 VK(103)"));
Keys("Enter Tab*3", true, "aaa", false, "Enter Ctrl+Alt+c Alt+f+a", true, 0.5, "F2 VK(103)", false);
Keys(KEYS, "Enter Tab*3", TEXT, "aaa [also text]", KEYS, "Enter Ctrl+Alt+c Alt+f+a", 0.5, KEYS "F2 VK(103)");
Keys(K("Enter Tab*3").T("aaa [also text]").K("Enter Ctrl+Alt+c Alt+f+a Wait(0.5) F2 VK(103)"));

 notes:
 Can be "[keys]text[keys]" or "[keys] text [keys]", it't the same. But if "[keys]  text  [keys]", will type " text ".
 If single argument, it is text with [embedded keys etc]. If multiple, single string parts are raw text.
 __________________________________________________________
 __________________________________________________________
 __________________________________________________________

 Number of keys/text/paste in folders "My", "QM Programming" and "Archive".
Only keys: 167
Only text: 4
Keys+text: 
Keys+var: .........  (1 +wait)

Paste: 48
Hybrid: 24
