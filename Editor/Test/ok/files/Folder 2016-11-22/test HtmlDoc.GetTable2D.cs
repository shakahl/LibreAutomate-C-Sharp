out
str html=
 <!DOCTYPE html>
 <html>
 <head>
 <style>
 table, th, td {
     border: 1px solid black;
 }
 </style>
 </head>
 <body>
 
 <table>
   <tr>
     <th colspan=2>colspan 2</th>
     <th>Savings</th>
   </tr>
   <tr>
     <td>January</td>
     <td rowspan="2">rowspan 2</td>
     <td>2</td>
   </tr>
   <tr>
     <td>February</td>
     <td>3</td>
   </tr>
   <tr>
     <td>March</td>
     <td>4</td>
     <td>5</td>
   </tr>
   <tr>
     <td colspan="2" rowspan="2">colspan+rowspan</td>
     <td><b>6</b></td>
   </tr>
   <tr>
     <td>7</td>
   </tr>
 </table>
 
 </body>
 </html>


HtmlDoc d.InitFromText(html)
ARRAY(str) a
 ARRAY(MSHTML.IHTMLElement) a2
d.GetTable2D(0 a)
 d.GetTable2D(d.GetHtmlElement("TABLE" 0) a)
int r c
for r 0 a.len
	out F"---- row {r} ----"
	for c 0 a.len(1)
		out a[c r]
