 str code=
  shell_exec('notepad');
  shell_exec('calc');
 PhpExec code

 PhpAddCode "function Add($a, $b) { return $a+$b; }"
 int sum=PhpFunc("Add" 1 2)
 out sum

