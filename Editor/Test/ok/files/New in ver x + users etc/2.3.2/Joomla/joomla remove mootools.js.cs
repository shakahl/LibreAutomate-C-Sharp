mootools.js is biggest file but almost unused.
Removing it makes the site to load slightly faster. Using slow Internet connection probably would save several seconds.
The site loads ok, however firebug shows 1 error - 1 missing function.
With smartoptimizer this is almost not useful because it compresses the file 75->18 KB.



To remove, in template before first ?> insert


//remove mootools.js and caption.js
$headerstuff=$this->getHeadData();
reset($headerstuff['scripts']);
foreach($headerstuff['scripts'] as $key=>$value){
//echo $key;
   if ("/media/system/js/mootools.js" == $key || "/media/system/js/caption.js" == $key)
      unset($headerstuff['scripts'][$key]);
}       
$this->setHeadData($headerstuff);

