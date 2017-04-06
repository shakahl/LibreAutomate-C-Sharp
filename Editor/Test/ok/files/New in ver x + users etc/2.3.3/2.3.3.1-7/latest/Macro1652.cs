str s=
 </div>
 <div class="dbutton">
 <button type="submit" class="big" onclick="download_file('somefiletoget'); return false;"><span><b><ins class="bigger">go!!</ins></b></span></button>
 </div>
 </div>
 </div>
 <div class="dbutton">
 <button type="submit" class="big" onclick="download_file('otherfiletoget'); return false;"><span><b><ins class="bigger">go!!</ins></b></span></button>
 </div>
 </div>

str rx=
 <button\s+type="submit"\s+class="big"\s+onclick="download_file\('(.+?)'\);
ARRAY(str) a; int i
if(!findrx(s rx 0 1|4 a 1)) end "not found"
for i 0 a.len
	out a[1 i]
