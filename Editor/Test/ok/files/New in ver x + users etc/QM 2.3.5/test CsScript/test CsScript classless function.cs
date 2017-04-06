/exe
 out

CsScript func.SetOptions("noFileCache=true")
func.AddCode("int Add(int a, int b) { return a+ b; }" 1)
out func.Call("Add" 10 5)

func.AddCode("using System.Windows.Forms;[] void MsgBox(object msg ) { MessageBox.Show(msg.ToString()); }" 1)
out func.Call("MsgBox" 100)

  this does not work
 str code=
  using System.Windows.Forms;
  int Add(int a, int b) { return a+ b; }
  void MsgBox(object msg ) { MessageBox.Show(msg.ToString()); }
 
 CsScript func.AddCode(code 1)
 out func.Call("Add" 10 5)
 out func.Call("MsgBox" 100)

 BEGIN PROJECT
 main_function  test CsScript classless function
 exe_file  $my qm$\test CsScript classless function.qmm
 flags  6
 guid  {B70A44E7-E2B6-4064-A01C-729002F34492}
 END PROJECT
