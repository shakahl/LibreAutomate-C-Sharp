 /ExcelMergeFiles
function $source_file Excel.Range&r_dest int&row_counter

Excel.Workbook wb._getfile(source_file) ;;open file in background
Excel.Worksheet ws=wb.Worksheets.Item(1) ;;get first worksheet
Excel.Range used_range=ws.UsedRange ;;get used range
Excel.Range row_src row_dest
int i

int remove_header_rows_count=5
int remove_footer_rows_count=13

for i 1+remove_header_rows_count used_range.Rows.Count+1-remove_footer_rows_count
	row_src=used_range.Rows.Item(i) ;;get source row
	row_dest=r_dest.Rows.Item(row_counter) ;;get destination row (initially empty)
	row_dest.Value=row_src.Value ;;copy
	row_counter+1
