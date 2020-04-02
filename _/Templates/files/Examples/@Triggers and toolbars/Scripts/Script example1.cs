/*/ c \@Triggers and toolbars\Functions\Class1.cs; /*/ //.
using Au; using Au.Types; using System; using System.Collections.Generic;
partial class Script : AScript { [STAThread] static void Main(string[] a) => new Script(a); Script(string[] args) { //;;;

//This is an example script used by ATask.Run examples in files "Hotkey triggers" (Ctrl+Shift+3) and "Common toolbars".
//It runs in separate process. You can edit and run it without restarting the main script process (triggers and toolbars).

ADialog.Show("Example");

//Although this script is in the project folder, it is not part of the project (only the first script is).
//Therefore other files of the project folder are not included in the compilation, and their classes/functions are not available.
//But you can explicitly specify class files. Use the Properties dialog. See the "c ..." in the first line.
Class1.Function1();

}}
