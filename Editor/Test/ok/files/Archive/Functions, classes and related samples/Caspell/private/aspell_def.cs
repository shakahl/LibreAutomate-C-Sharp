type AspellModuleInfo $name double'order_num $lib_dir !*dict_dirs !*dict_exts
type AspellDictInfo $name $code $jargon int'size $size_str AspellModuleInfo*module
type AspellErrorInfo AspellErrorInfo*isa $mesg int'num_parms $parms[3]
type AspellError $mesg AspellErrorInfo*err
type AspellKeyInfo $name type $def $desc !otherdata[16]
def AspellKeyInfoBool 2
def AspellKeyInfoInt 1
def AspellKeyInfoList 3
def AspellKeyInfoString 0
type AspellStringPair $first $second
type AspellToken int'offset int'len
type AspellTypeId int'num !str[4]
dll- "%aspell.dll%" aspell_config_assign !*ths !*other
dll- "%aspell.dll%" !*aspell_config_clone !*ths
dll- "%aspell.dll%" !*aspell_config_elements !*ths
dll- "%aspell.dll%" AspellError*aspell_config_error !*ths
dll- "%aspell.dll%" $aspell_config_error_message !*ths
dll- "%aspell.dll%" int'aspell_config_error_number !*ths
dll- "%aspell.dll%" $aspell_config_get_default !*ths $key
dll- "%aspell.dll%" int'aspell_config_have !*ths $key
dll- "%aspell.dll%" AspellKeyInfo*aspell_config_keyinfo !*ths $key
dll- "%aspell.dll%" !*aspell_config_possible_elements !*ths int'include_extra
dll- "%aspell.dll%" int'aspell_config_remove !*ths $key
dll- "%aspell.dll%" int'aspell_config_replace !*ths $key $value
dll- "%aspell.dll%" $aspell_config_retrieve !*ths $key
dll- "%aspell.dll%" int'aspell_config_retrieve_bool !*ths $key
dll- "%aspell.dll%" int'aspell_config_retrieve_int !*ths $key
dll- "%aspell.dll%" int'aspell_config_retrieve_list !*ths $key !*lst
dll- "%aspell.dll%" aspell_config_set_extra !*ths AspellKeyInfo*begin AspellKeyInfo*end
dll- "%aspell.dll%" aspell_dict_info_enumeration_assign !*ths !*other
dll- "%aspell.dll%" int'aspell_dict_info_enumeration_at_end !*ths
dll- "%aspell.dll%" !*aspell_dict_info_enumeration_clone !*ths
dll- "%aspell.dll%" AspellDictInfo*aspell_dict_info_enumeration_next !*ths
dll- "%aspell.dll%" !*aspell_dict_info_list_elements !*ths
dll- "%aspell.dll%" int'aspell_dict_info_list_empty !*ths
dll- "%aspell.dll%" int'aspell_dict_info_list_size !*ths
dll- "%aspell.dll%" AspellError*aspell_document_checker_error !*ths
dll- "%aspell.dll%" $aspell_document_checker_error_message !*ths
dll- "%aspell.dll%" int'aspell_document_checker_error_number !*ths
dll- "%aspell.dll%" !*aspell_document_checker_filter !*ths
dll- "%aspell.dll%" AspellToken'aspell_document_checker_next_misspelling !*ths
dll- "%aspell.dll%" aspell_document_checker_process !*ths $str int'size
dll- "%aspell.dll%" aspell_document_checker_reset !*ths
dll- "%aspell.dll%" AspellError*aspell_error !*ths
dll- "%aspell.dll%" int'aspell_error_is_a AspellError*ths AspellErrorInfo*e
dll- "%aspell.dll%" $aspell_error_message !*ths
dll- "%aspell.dll%" int'aspell_error_number !*ths
dll- "%aspell.dll%" AspellError*aspell_filter_error !*ths
dll- "%aspell.dll%" $aspell_filter_error_message !*ths
dll- "%aspell.dll%" int'aspell_filter_error_number !*ths
dll- "%aspell.dll%" aspell_key_info_enumeration_assign !*ths !*other
dll- "%aspell.dll%" int'aspell_key_info_enumeration_at_end !*ths
dll- "%aspell.dll%" !*aspell_key_info_enumeration_clone !*ths
dll- "%aspell.dll%" AspellKeyInfo*aspell_key_info_enumeration_next !*ths
dll- "%aspell.dll%" aspell_module_info_enumeration_assign !*ths !*other
dll- "%aspell.dll%" int'aspell_module_info_enumeration_at_end !*ths
dll- "%aspell.dll%" !*aspell_module_info_enumeration_clone !*ths
dll- "%aspell.dll%" AspellModuleInfo*aspell_module_info_enumeration_next !*ths
dll- "%aspell.dll%" !*aspell_module_info_list_elements !*ths
dll- "%aspell.dll%" int'aspell_module_info_list_empty !*ths
dll- "%aspell.dll%" int'aspell_module_info_list_size !*ths
dll- "%aspell.dll%" int'aspell_mutable_container_add !*ths $to_add
dll- "%aspell.dll%" aspell_mutable_container_clear !*ths
dll- "%aspell.dll%" int'aspell_mutable_container_remove !*ths $to_rem
dll- "%aspell.dll%" !*aspell_mutable_container_to_mutable_container !*ths
dll- "%aspell.dll%" int'aspell_speller_add_to_personal !*ths $word int'word_size
dll- "%aspell.dll%" int'aspell_speller_add_to_session !*ths $word int'word_size
dll- "%aspell.dll%" int'aspell_speller_check !*ths $word int'word_size
dll- "%aspell.dll%" int'aspell_speller_clear_session !*ths
dll- "%aspell.dll%" !*aspell_speller_config !*ths
dll- "%aspell.dll%" AspellError*aspell_speller_error !*ths
dll- "%aspell.dll%" $aspell_speller_error_message !*ths
dll- "%aspell.dll%" int'aspell_speller_error_number !*ths
dll- "%aspell.dll%" !*aspell_speller_main_word_list !*ths
dll- "%aspell.dll%" !*aspell_speller_personal_word_list !*ths
dll- "%aspell.dll%" int'aspell_speller_save_all_word_lists !*ths
dll- "%aspell.dll%" !*aspell_speller_session_word_list !*ths
dll- "%aspell.dll%" int'aspell_speller_store_replacement !*ths $mis int'mis_size $cor int'cor_size
dll- "%aspell.dll%" !*aspell_speller_suggest !*ths $word int'word_size
dll- "%aspell.dll%" aspell_string_enumeration_assign !*ths !*other
dll- "%aspell.dll%" int'aspell_string_enumeration_at_end !*ths
dll- "%aspell.dll%" !*aspell_string_enumeration_clone !*ths
dll- "%aspell.dll%" $aspell_string_enumeration_next !*ths
dll- "%aspell.dll%" int'aspell_string_list_add !*ths $to_add
dll- "%aspell.dll%" aspell_string_list_assign !*ths !*other
dll- "%aspell.dll%" aspell_string_list_clear !*ths
dll- "%aspell.dll%" !*aspell_string_list_clone !*ths
dll- "%aspell.dll%" !*aspell_string_list_elements !*ths
dll- "%aspell.dll%" int'aspell_string_list_empty !*ths
dll- "%aspell.dll%" int'aspell_string_list_remove !*ths $to_rem
dll- "%aspell.dll%" int'aspell_string_list_size !*ths
dll- "%aspell.dll%" !*aspell_string_list_to_mutable_container !*ths
dll- "%aspell.dll%" int'aspell_string_map_add !*ths $to_add
dll- "%aspell.dll%" aspell_string_map_assign !*ths !*other
dll- "%aspell.dll%" aspell_string_map_clear !*ths
dll- "%aspell.dll%" !*aspell_string_map_clone !*ths
dll- "%aspell.dll%" !*aspell_string_map_elements !*ths
dll- "%aspell.dll%" int'aspell_string_map_empty !*ths
dll- "%aspell.dll%" int'aspell_string_map_insert !*ths $key $value
dll- "%aspell.dll%" $aspell_string_map_lookup !*ths $key
dll- "%aspell.dll%" int'aspell_string_map_remove !*ths $to_rem
dll- "%aspell.dll%" int'aspell_string_map_replace !*ths $key $value
dll- "%aspell.dll%" int'aspell_string_map_size !*ths
dll- "%aspell.dll%" !*aspell_string_map_to_mutable_container !*ths
dll- "%aspell.dll%" aspell_string_pair_enumeration_assign !*ths !*other
dll- "%aspell.dll%" int'aspell_string_pair_enumeration_at_end !*ths
dll- "%aspell.dll%" !*aspell_string_pair_enumeration_clone !*ths
dll- "%aspell.dll%" AspellStringPair'aspell_string_pair_enumeration_next !*ths
dll- "%aspell.dll%" !*aspell_word_list_elements !*ths
dll- "%aspell.dll%" int'aspell_word_list_empty !*ths
dll- "%aspell.dll%" int'aspell_word_list_size !*ths
dll- "%aspell.dll%" delete_aspell_can_have_error !*ths
dll- "%aspell.dll%" delete_aspell_config !*ths
dll- "%aspell.dll%" delete_aspell_dict_info_enumeration !*ths
dll- "%aspell.dll%" delete_aspell_document_checker !*ths
dll- "%aspell.dll%" delete_aspell_filter !*ths
dll- "%aspell.dll%" delete_aspell_key_info_enumeration !*ths
dll- "%aspell.dll%" delete_aspell_module_info_enumeration !*ths
dll- "%aspell.dll%" delete_aspell_speller !*ths
dll- "%aspell.dll%" delete_aspell_string_enumeration !*ths
dll- "%aspell.dll%" delete_aspell_string_list !*ths
dll- "%aspell.dll%" delete_aspell_string_map !*ths
dll- "%aspell.dll%" delete_aspell_string_pair_enumeration !*ths
dll- "%aspell.dll%" !*get_aspell_dict_info_list !*config
dll- "%aspell.dll%" !*get_aspell_module_info_list !*config
dll- "%aspell.dll%" !*new_aspell_config
dll- "%aspell.dll%" !*new_aspell_document_checker !*speller
dll- "%aspell.dll%" !*new_aspell_speller !*config
dll- "%aspell.dll%" !*new_aspell_string_list
dll- "%aspell.dll%" !*new_aspell_string_map
dll- "%aspell.dll%" !*to_aspell_document_checker !*obj
dll- "%aspell.dll%" !*to_aspell_filter !*obj
dll- "%aspell.dll%" !*to_aspell_speller !*obj
