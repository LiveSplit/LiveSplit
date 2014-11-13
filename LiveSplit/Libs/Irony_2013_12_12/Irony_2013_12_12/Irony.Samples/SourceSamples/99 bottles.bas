005 REM 99 Bottles of Beer in GW-BASIC 
010 REM by Stefan Scheler <sts[at]synflood[dot]de> - Ilmenau, Germany in June 2005
015 REM GW-BASIC (named after Greg Whitten, an early Microsoft employee and is 
020 REM known more affectionately as 'gee-whiz') was a dialect of BASIC developed
025 REM by Microsoft, originally for Compaq. It was the predecessor of QBasic. 
030 CLS
035 FOR I = 99 TO 1 STEP -1
040 MODE = 1: GOSUB 080
045 PRINT I; "bottle" + BOTTLES$ + " of beer on the wall,"; i; "bottle" + BOTTLES$ + " of beer."
050 MODE = 2: GOSUB 080
055 PRINT " Take one down and pass it around,"; i-1; "bottle" + BOTTLES$ + " of beer on the wall."
060 NEXT
065 PRINT " No more bottles of beer on the wall, no more bottles of beer."
070 PRINT " Go to the store and buy some more. 99 bottles of beer."
075 END

080 REM subroutine handles plural s
085 IF I = MODE THEN BOTTLES$ = "" ELSE BOTTLES$ = "s"
090 RETURN 
