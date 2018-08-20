/* meta
outputType app
*/
//#include usings.txt
#region App.Main
[module: DefaultCharSet(CharSet.Unicode)]
partial class App {
[STAThread] static void Main(string[] args) { new App()._Main(args); }
void _Main(string[] args) {
#endregion
Print("test");



}
}
