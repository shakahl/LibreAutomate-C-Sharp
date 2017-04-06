 /CtoQM
function $src_file $dest_file $include_dir $preproc_def flags $dll_dn_file $dll_an_file $pch_file $dll_list $ref_list $type_map

int t1=perf

str s sff.getpath(dest_file) sfn.getfilename(dest_file)
m_dest.from(sff sfn)
m_sInclDir=include_dir
m_outIncl=flags&1
m_compact=flags&4!0
m_needclasses=flags&64!0
m_delayload=flags&128!0
if(len(dll_dn_file)) s.getfile(dll_dn_file); m_mfdn.AddList(s "") ;;out m_mfdn.Count
if(len(dll_an_file)) s.getfile(dll_an_file); m_mfan.AddList(s "") ;;out m_mfan.Count
if(len(dll_list)) AddDlls(dll_list)
if(len(pch_file)) m_pch=1

 Add default constants, etc
AddDef(preproc_def flags&32)
AddTypes(type_map flags&32)
if(m_pch) AddPCH(pch_file)

 Preprocess (include, exclude, exctract constants)
H_file(src_file)
 out m_s
if(flags&2) m_s.setfile(_s.from(m_dest "_preprocessed.txt")); ret

 Exctract everything else
Tidy
Convert ;;main
if(flags&8) CreatePCH

 Make corrections, prepare to save
FuncA
Typedefs
AppendMap(m_mc 1) ;;must be before, because adds to m_mg and other maps
MissingIID
MacrosToFunctions
 if(m_compact) RemoveFuncW

AppendMap(m_mf 3)
AppendMap(m_mt 4)
AppendMap(m_mi 5)
AppendMap(m_mg 2)
AppendMap(m_mtd 6)
if(!m_compact) AppendMap(m_mo 7)

UnknownTypes
RemovePCH

 Save
Save(dest_file ref_list)

 Results (including pch)
int t2=perf
if(flags&16=0)
	 out "time %.3f s" t2-t1/1000000.0
	
	out " dll %i, type %i, interface %i, def %i, guid %i, typedef %i, callback %i, added %i" m_mf.Count m_mt.Count m_mi.Count m_mc.Count m_mg.Count m_mtd.Count m_mfcb.Count m_mall.Count
	
	 OutMap(m_mf)
	 OutMap(m_mt)
	 OutMap(m_mi)
	 OutMap(m_mc)
	 OutMap(m_mg)
	 OutMap(m_mtd)
	 OutMap(m_mfcb)
