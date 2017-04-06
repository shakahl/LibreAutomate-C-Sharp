 /exe
PF
CsScript x.Init
PN
x.AddCode("")
PN
rep 1
	x.Call("Test.Main")
	PN
PO

 BEGIN PROJECT
 main_function  Scripting C#, run Main function3
 exe_file  $my qm$\Scripting C#, run Main function3.qmm
 flags  6
 guid  {3144537A-4A9C-4CE9-8CE7-4BA79D51C3D3}
 END PROJECT

#ret
//C# code
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Reflection;

public class Test
{
//[MethodImpl(MethodImplOptions.NoOptimization)]
public static void Main()
{
string s="";
InputBox(null, "te", ref s);
}

//[MethodImpl(MethodImplOptions.NoOptimization)]
public static DialogResult InputBox(string title, string promptText, ref string value)
{
//if(title==null) return DialogResult.Cancel;            ///
  Form form = new Form();
  Label label = new Label();
  TextBox textBox = new TextBox();
  Button buttonOk = new Button();
  Button buttonCancel = new Button();

  form.Text = title;
  label.Text = promptText;
  textBox.Text = value;

  buttonOk.Text = "OK";
  buttonCancel.Text = "Cancel";
  buttonOk.DialogResult = DialogResult.OK;
  buttonCancel.DialogResult = DialogResult.Cancel;

  label.SetBounds(9, 20, 372, 13);
  textBox.SetBounds(12, 36, 372, 20);
  buttonOk.SetBounds(228, 72, 75, 23);
  buttonCancel.SetBounds(309, 72, 75, 23);

  label.AutoSize = true;
  textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
  buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
  buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

  form.ClientSize = new Size(396, 107);
  form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
  form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
  form.FormBorderStyle = FormBorderStyle.FixedDialog;
  form.StartPosition = FormStartPosition.CenterScreen;
  form.MinimizeBox = false;
  form.MaximizeBox = false;
  form.AcceptButton = buttonOk;
  form.CancelButton = buttonCancel;

    AppDomain currentDomain = AppDomain.CurrentDomain;
    Assembly[] assems = currentDomain.GetAssemblies();
    Console.WriteLine("List of assemblies loaded in current appdomain:");
    foreach (Assembly assem in assems){
      Console.WriteLine(assem.ToString());
    }

if(title==null) return DialogResult.Cancel; 
  DialogResult dialogResult = form.ShowDialog();
  value = textBox.Text;
  return dialogResult;
}
}
