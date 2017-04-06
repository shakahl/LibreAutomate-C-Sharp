 variables
str Name="Oranges"
str Last_Name="Apples"
 format text
str s.format("Name: %s[]Last Name: %s[]" Name Last_Name)
 the following inserted by the dialog
_s=s; _s.escape(9)
run _s.from("mailto:santa@claus.fin?subject=hello" "&body=" _s)
