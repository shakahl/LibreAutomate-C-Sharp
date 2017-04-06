 /exe 1

 This macro gets text from a table in Internet Explorer, and populates Excel sheet.
 To test:
   Open a web page containing a table in Internet Explorer.
   Capture the table using the 'Find html element' dialog.
      Make sure that Tag field is TABLE. If it isn't, navigate to the table using < and > buttons.
      In this macro, replace the value of table_index to the value of the Index field of the dialog.
      Or, instead of index, use name or id, in double quotes. You can see them in the dialog, in the Html field or when you select name or id in the combo box.
      In this macro, change the value of colunm_count to the number of columns. Some columns may be not clearly visible, so you may have to use a bigger value.
 Run Excel.
 Run this macro. It should populate the sheet in Excel.
 This macro is set to run in separate process because on Vista QM cannot connect to Excel unless it runs as User.


VARIANT table_index=0 ;;change this
int colunm_count=5 ;;change this
int need_only_some_columns=0 ;;change this to 1 to see how it works
int first_cell_index=0 ;;change this to get data not from first cell

HtmlDoc d.InitFromInternetExplorer(win("Internet Explorer"))
ARRAY(str) a
d.GetTable(table_index a)

int i j n
n=a.len/colunm_count*colunm_count; if(n!=a.len) out "warning: the column count is incorrect or some cells span multiple columns"
ExcelSheet es
if(need_only_some_columns=0) ;;all columns
	es.Init ;;populate active sheet
	for i first_cell_index n colunm_count
		for j 0 colunm_count
			es.SetCell(a[i+j] 1+j i-first_cell_index/colunm_count+1); err
else ;;only columns 1, 2 and 3
	es.Init(2) ;;populate sheet 2
	for i first_cell_index n colunm_count
		es.SetCell(a[i] 1 i-first_cell_index/colunm_count+1)
		es.SetCell(a[i+1] 2 i-first_cell_index/colunm_count+1)
		es.SetCell(a[i+2] 3 i-first_cell_index/colunm_count+1)
	
