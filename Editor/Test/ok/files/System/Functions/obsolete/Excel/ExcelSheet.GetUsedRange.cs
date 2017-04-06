function [int&rowCount] [int&columnCount] [int&firstRow] [int&firstColumn]

 Gets info about the used range (part of worksheet containing non-empty cells).

 rowCount, columnCount - variables that receive number of rows and columns. Can be 0 if don't need.
   Does not include empty rows/columns at the beginning.
 firstRow, firstColumn - variables that receive 1-based index of the first used row/column. Can be 0 if don't need.

 Added in: QM 2.3.2.
 Errors: Excel errors


WS(1)

GetRangeInfo("" firstColumn firstRow columnCount rowCount)

err+ E
