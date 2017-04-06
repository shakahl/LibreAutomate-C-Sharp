str s =
 {"sort":"1503068539","count_comment":"0","is_mobile":"Y","purchase_cnt":"3570","idx":"8904881","kind":1,"c_mark":"","icon":"","state":"1","title":"<span class="_L_" ><a href="javascript:;" onclick="viewContentsOpen('1', '8904881', '', '', '');"> <b><font color="#54940b"> (평점 7.30) <mark>런닝<\/mark> 맨.1987<\/font><\/b><\/a><\/span>","iconNew":"","size":"2.1<span>G<\/span>","category":"SF\/환타지","cash1":220,"cash2":"110","uploader":"<span class='uploader not'>부여백마강<\/span>","sell_option":"0","adult":"0","css_type":1,"new_sort":4},{"sort":"1441051470","count_comment":"1","is_mobile":"Y","purchase_cnt":"3570","idx":"8709661","kind":1,"c_mark":"","icon":"","state":"1","title":"<span class="_L_" ><a href="javascript:;" onclick="viewContentsOpen('1', '8709661', '', '', '');"> <mark>런닝<\/mark>맨 (Runningman, 2012) 신하균,이민호,김상호,.. <b style='font-size:11px;color:#b82626;'>(1)<\/b><\/a><\/span>","iconNew":" <b class='comNum' style='color:#b82626;'>(1)<\/b>","size":"2.9<span>G<\/span>","category":"한국영화","cash1":2000,"cash2":"1000","uploader":"<span class='uploader not'>옵로더더<\/span>","sell_option":"1","adult":"0","css_type":1,"new_sort":4}],

	str IDX=
	 "idx":"?"

	str TITLE=
	  <b><font color="?">?<\/font><\/b><\/a><\/span>","
 
	str PRICE=
	 "cash2":"?"

	str STORAGE=
	 "size":"?<\/span>"

	str USERID=
	 <span class='uploader not'>?<\/span>

	str SECTION=
	 "category":"?"
 
	ARRAY(str) GetIDXnum GetTitleInfo GetSizeInfo GetPriceInfo GetUserID GetSectionInfo; str SearchOutput PopupOut TitleOut
	if(!findrx(s IDX 0 4 GetIDXnum)) out "not found1";ret
	if(!findrx(s TITLE 0 4 GetTitleInfo)) out "not found2";ret
	if(!findrx(s PRICE 0 4 GetPriceInfo)) out "not found3";ret
	if(!findrx(s STORAGE 0 4 GetSizeInfo)) out "not found4";ret
	if(!findrx(s USERID 0 4 GetUserID)) out "not found5";ret
	if(!findrx(s SECTION 0 4 GetSectionInfo)) out "not found6";ret
	for(_i 0 GetIDXnum.len)
		SearchOutput.format("%s %s %s %s %s %s" GetIDXnum[0 _i] GetTitleInfo[0 _i] GetPriceInfo[0 _i] GetSizeInfo[0 _i] GetUserID[0 _i] GetSectionInfo[0 _i])
			str GetReseult= SearchOutput
			out GetReseult
