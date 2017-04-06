out

str- t_form_data ;;on OK will be populated with form data

str controls = "3"
str ax3SHD
ax3SHD.getmacro("sample html form")
if(!ShowDialog("HTML_dialog_proc" &HTML_dialog_proc &controls)) ret

out t_form_data
