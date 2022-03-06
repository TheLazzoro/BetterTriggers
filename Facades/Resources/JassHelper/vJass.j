globals

integer UntitledVariable
endglobals
globals

item UntitledVariable0
endglobals
function InitGlobals takes nothing returns nothing
set UntitledVariable=0
set UntitledVariable0=
endfunction


//! zinc

    library Bottles99
    {
        /* 99 Bottles of beer sample,
           prints the lyrics at: http://99-bottles-of-beer.net/lyrics.html
        */
        function onInit()
        {
            string bot = "99 bottles";
            integer i=99;
            while (i>=0)
            {
                BJDebugMsg(bot+" of beer on the wall, "+bot+" of beer");
                i=i-1;
                if      (i == 1) bot = "1 bottle";
                else if (i == 0) bot = "No more bottles";
                else            bot = I2S(i)+" bottles";
                //Lazyness = "No more" is always capitalized.

                if(i>=0)
                {
                    BJDebugMsg("Take one down and pass it around, "+bot+" of beer on the wall.\n");
                }
            }
            BJDebugMsg("Go to the store and buy some more, 99 bottles of beer on the wall.");
        }
    }

//! endzinc

call StopCameraForPlayerBJ()
call LockGuardPosition()
call StopCameraForPlayerBJ()
//*
//*  Unit Creation
//*
function CreateUnits takes nothing returns nothing
	local unit u
	local integer unitID
	local trigger t
	local real life
call BlzCreateUnitWithSkin(Player(0), 'hfoo', -1702.2902, 338.09357, 3.5110056, 'hfoo')
call BlzCreateUnitWithSkin(Player(0), 'Hamg', -1957.0186, 381.39386, 3.7505379, 'Hamg')
call BlzCreateUnitWithSkin(Player(0), 'hsor', -1719.3152, -146.31201, 2.152816, 'hsor')
endfunction
function main takes nothing returns nothing
endfunction
