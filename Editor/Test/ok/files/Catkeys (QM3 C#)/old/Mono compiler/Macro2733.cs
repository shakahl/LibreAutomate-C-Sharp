using System.Runtime.InteropServices;
public class Test {
 [DllImport("user32.dll")]
 public static extern int MessageBoxA(IntPtr hWnd, String text, String caption, int options);
 public static void Main() { Console.WriteLine(11); }
 public int Add(int a, int b) { return a+b; }
 }
Test.Main();
