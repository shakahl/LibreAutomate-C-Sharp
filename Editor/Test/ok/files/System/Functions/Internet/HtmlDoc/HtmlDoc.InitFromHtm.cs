function Htm&el

 Initializes the variable from a html element.
 Error if something fails, eg el is not initialized.

 REMARKS
 Read about html elements and Htm variables in <help>htm</help> help.
 You can capture element in Internet Explorer using the 'Find html element' dialog.


_s=el.el.outerHTML
InitFromText(_s)
err+ end _error
