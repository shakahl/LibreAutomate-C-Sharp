; This function gets some attribute.
; Input:
;       s: string of atributes with the format: X,Y,Z where X,Y and Z are numbers (int)
;       Atr: The atribute you want to return:
;            0: Number of versions
;            1: Versioning time before delete (days)
;            2: Deleting time if original file doesn't exist any more before delete (days)

function# ~s #Atr
ret val(getTok(s Atr -1 ","))
err
	ret -1