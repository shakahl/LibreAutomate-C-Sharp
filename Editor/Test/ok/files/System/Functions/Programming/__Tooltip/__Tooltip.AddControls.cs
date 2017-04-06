function $controlsCSV

 Adds tooltips for multiple controls.
 Same as <help>__Tooltip.AddControl</help>, but don't need to call for each control.

 controlsCSV - CSV containing control ids and their tooltip text.
   CSV line format: "id, text".


ICsv c._create
c.FromString(controlsCSV)
for _i 0 c.RowCount
	AddControl(c.CellInt(_i 0) c.Cell(_i 1))

err+ end _error.description 8
