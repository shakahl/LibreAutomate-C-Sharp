str s=
 <><b>bold</b> <i>italic</i> <u>underline</u>
 <c "0x00ff00">green color</c>
 <b><i><c "0xff">nested tags</c></i></b>
 <z "0xff">red background (QM 2.3.3)</z>
 <Z "0xff">red background full line(QM 2.3.3)</Z>
 <_> Text with <i>tags</i> that are <u>ignored</u>. </_>
 
 <c 0x00ff00>green color</c>
 <c 0xff>tags</c>
 <z 0xff>red background (QM 2.3.3)</z>
 <Z 0xff>red background full line(QM 2.3.3)</Z>
 
 <c 0xff8080>blue<c 0xff>red</c>blue</c>
 <c 0x8000>green<c 0xff8080>blue<c 0xff>red</c>blue</c>green</c>
out s
