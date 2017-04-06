function [int&rowCount] [int&columnCount] [int&firstRow] [int&firstColumn]

 Gets info about the selected range.

 rowCount, columnCount - variables that receive number of rows and columns. Can be 0 if don't need. If the function returns both 1, a single cell is selected.
 firstRow, firstColumn - variables that receive 1-based first row and column index. Can be 0 if don't need.

 REMARKS
 Works in the active sheet, even if this variable represents other sheet.

 Added in: QM 2.3.2.
 Errors: Excel errors


WS(1)

GetRangeInfo("<sel>" firstColumn firstRow columnCount rowCount)

err+ E
