globals
    boolean array isVisibleAbilityMenu
    boolean array isVisibleNeutralAbilityMenu
endglobals

library LibRequired initializer init
    private function init takes nothing returns nothing
    endfunction
        
    function GetActivePlayers takes nothing returns integer
        local integer n = 0
        local integer i = 1
        local integer endIndex = 6
        loop
            exitwhen i > endIndex
            if (udg_ActivePlayers[i] == true) then
                set n = n + 1
            endif
            set i = i + 1
        endloop

        return n
    endfunction

    function GetShortPlayerName takes player p returns string
        local string pn = GetPlayerName(p)
        local integer i = 0
        loop
            exitwhen i >= StringLength(pn)
            if SubString(pn, i, i + 1) == "#" then
                return SubString(pn, 0, i)
            endif
            set i = i + 1
        endloop
        return pn
    endfunction

    function UnitMoveLoc takes unit u returns nothing
        local player v_player = GetOwningPlayer(u)
        local location point = GetUnitLoc(u)
        local location point2 = null
        local real angle = 180
        local real real1
        local real real2
        local real real3
        local real real4
        local location loc

        if (IsPlayerInForce(v_player, udg_Teams[1]) == true) then
            set point2 = GetRectCenter(gg_rct_Fortress_Team_2)
        else
            set point2 = GetRectCenter(gg_rct_Fortress_Team_1)
        endif
        set real1 = DistanceBetweenPoints(point, point2)
        set real2 = AngleBetweenPoints(point, point2)
        set real3 = real2 - angle
        set real4 = real1 * CosBJ(real3)
        set loc = PolarProjectionBJ(point, real4, 180)

        call IssuePointOrderLoc(u, "attack", loc)
        call RemoveLocation(point)
        call RemoveLocation(point2)
        call RemoveLocation(loc)
    endfunction

endlibrary