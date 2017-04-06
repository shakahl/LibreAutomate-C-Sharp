 RegisterNetComComponent "$qm$\QmNet.dll" 2 ;;register
 RegisterNetComComponent "$qm$\QmNet.dll" 1 ;;unregister
__ComActivator ca.Activate("QmNet.dll,2")
 __ComActivator ca.Activate("QmNet\QmNet.manifest")

#if 0

typelib QmNet "$qm$\QmNet.tlb"

#else

 str code.getfile("Q:\Downloads\cs-script\Samples\hello.cs")
 out code
str code=
 using System.Windows.Forms;
 MessageBox.Show( "Hello World!");

str sf.expandpath("$qm$\hello.cs")
int+ ___csscript_bug_fixed; if(!___csscript_bug_fixed) ___csscript_bug_fixed=SetEnvVar("CSSCRIPT_DIR" "" 1) ;;if cs-script suite installed, error when trying to compile code from memory (ok from file): "Compiler executable file csc.exe cannot be found". Workaround: remove this env var from registry or in this process before using cs-script library.

Q &q
IDispatch d._create("QmNet.CsScript")
Q &qq
out d.Test(5)
 out d.ExecFile(sf "/?")
 out d.ExecFile("Q:\Downloads\cs-script\Samples\hello_auto.cs" "")
 out d.Eval("Test() { return 5; }")
 out d.ExecFile(code "")
Q &qqq
outq
