<?php

$file='images/qm.png';
$size=filesize("$file");
header('Content-Type: image/png');
header("Content-Length: $size");
header('Content-Disposition: attachment;filename="qm.png"');
$fp=fopen($file,'rb');
fpassthru($fp);
fclose($fp);

?>
