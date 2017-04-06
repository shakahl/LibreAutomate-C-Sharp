ARRAY(str) MGMT.create(6 1) ;;6 columns, 1 row
out MGMT.ndim ;;2
out MGMT.len(1) ;;6 columns in dimension 1
out MGMT.len(2) ;;1 row in dimension 2

MGMT[0 0]="test" ;;first column, first row
out MGMT[0 0]
out MGMT[0 1] ;;error invalid index because there is only 1 row (index 0)
