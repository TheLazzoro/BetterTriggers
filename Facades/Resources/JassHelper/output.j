globals
//globals from Bottles99:
constant boolean LIBRARY_Bottles99=true
//endglobals from Bottles99

trigger l__library_init

//JASSHelper struct globals:

endglobals


//library Bottles99:

    function Bottles99__onInit takes nothing returns nothing
        local string bot="99 bottles"
        local integer i=99
        loop
        exitwhen ( i < 0 )
            call BJDebugMsg(bot + " of beer on the wall, " + bot + " of beer")
            set i=i - 1
            if ( i == 1 ) then
                set bot="1 bottle"
            elseif ( i == 0 ) then
                set bot="No more bottles"
            else //Lazyness = "No more" is always capitalized.
                set bot=I2S(i) + " bottles"
            endif
            if ( i >= 0 ) then
                call BJDebugMsg("Take one down and pass it around, " + bot + " of beer on the wall.\n")
            endif
        endloop
        call BJDebugMsg("Go to the store and buy some more, 99 bottles of beer on the wall.")
    endfunction

//library Bottles99 ends
function InitGlobals takes nothing returns nothing
endfunction

//*
//*  Unit Creation
//*
function CreateUnits takes nothing returns nothing
 local unit u
 local integer unitID
 local trigger t
 local real life
call BlzCreateUnitWithSkin(Player(0), 'hfoo', - 1702.2902, 338.09357, 3.5110056, 'hfoo')
call BlzCreateUnitWithSkin(Player(0), 'Hamg', - 1957.0186, 381.39386, 3.7505379, 'Hamg')
call BlzCreateUnitWithSkin(Player(0), 'hsor', - 1719.3152, - 146.31201, 2.152816, 'hsor')
endfunction
function main takes nothing returns nothing

call ExecuteFunc("Bottles99__onInit")

endfunction



//Struct method generated initializers/callers:

