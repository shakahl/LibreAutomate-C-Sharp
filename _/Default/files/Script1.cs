//{{
//{{ using
using System; using System.Collections.Generic; using System.Text; using System.Text.RegularExpressions; using System.Diagnostics; using System.Runtime.InteropServices; using System.IO; using System.Threading; using System.Threading.Tasks; using System.Windows.Forms; using System.Drawing; using System.Linq; using Au; using Au.Types; using static Au.NoClass; using Au.Triggers; //}}
//{{ main
unsafe partial class Script :AuScript { [STAThread] static void Main(string[] args) { new Script()._Main(args); } void _Main(string[] args) { //}}//}}//}}//}}

//To run this C# script, click the Run button on the toolbar.

Print("Function Print writes text and variables to the output pane.");
AuDialog.Show("Example", "Message box.");
