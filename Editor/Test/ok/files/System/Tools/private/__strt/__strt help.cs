 Used with QM code insertion dialogs.
 Based on str. Usually used for dialog variables instead of str, but can be used anywhere.
 Prepares the user-entered or default value to be used in code, depending on field type (text, numeric/expression, variable).
 For text fields use S. It adds escape sequences and "", etc.
 For numeric/expression fields use N. It encloses in () if need, sets default, etc.
 For variable fields use VD. It validates name or sets default, adds declaration if need, etc.
 VN simply validates name or sets default.
