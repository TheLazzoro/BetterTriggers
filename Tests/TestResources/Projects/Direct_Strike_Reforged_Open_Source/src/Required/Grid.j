globals 
    image array gridEffect[6][96]
    framehandle btnGrid
    boolean array isGridOn
endglobals

function ShowHideGrid takes nothing returns nothing
    local integer p = GetConvertedPlayerId(GetTriggerPlayer())
    local integer i = 1
    local integer endIndex = 96
    local integer val

    call BlzFrameSetEnable(BlzGetTriggerFrame(), false)
    call BlzFrameSetEnable(BlzGetTriggerFrame(), true)

    if isGridOn[p] then
        set isGridOn[p] = false
        set val = 0
    else
        set isGridOn[p] = true
        set val = 255
    endif

    loop
        exitwhen i > endIndex
        if(GetLocalPlayer() == GetTriggerPlayer()) then
            call SetImageColor( gridEffect[p][i], 255, 255, 255, val)
        endif
        set i = i + 1
    endloop
endfunction

function SetupGrid takes nothing returns nothing
    local integer p = 1
    local integer endIndex = 6
    local integer i = 1
    local integer x = 1
    local integer endIndex1 = 96
    local integer y = 1
    local real px
    local real py
    local trigger trig

    loop
        exitwhen p > endIndex
        set px = GetRectCenterX(udg_BuildRegion[p])
        set py = GetRectCenterY(udg_BuildRegion[p])
        set x = 1
        set y = 1
        set i = 1

        loop
            exitwhen i > endIndex1

            if(ModuloInteger(x, 9) == 0) then
                set x = 1
                set y = y + 1
            endif

            // CreateImage(string file, real sizeX, real sizeY, real sizeZ, real posX, real posY, real posZ, real originX, real originY, real originZ, integer imageType)
            set gridEffect[p][i] = CreateImage("Gridplane.dds",128,128,0,0,0,0, 1,1,1,1)
            call SetImageRenderAlways( gridEffect[p][i], true )
            call SetImageColor( gridEffect[p][i], 255, 255, 255, 0)
            call SetImagePosition(gridEffect[p][i],(px - 128*5) + (x * 128),(py - 128*7) + (y * 128),0)

            set x = x + 1
            set i = i + 1
        endloop

        set p = p + 1
    endloop

    set btnGrid = BlzCreateFrame("ScriptDialogButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
    call BlzFrameSetSize(btnGrid, 0.06, 0.03)
    call BlzFrameSetAbsPoint(btnGrid, FRAMEPOINT_CENTER, 0.28, 0.14)
    call BlzFrameSetText(btnGrid, "Grid")
    call BlzFrameSetVisible(btnGrid, false)

    // Button trigger
    set trig = CreateTrigger()
    call BlzTriggerRegisterFrameEvent(trig, btnGrid, FRAMEEVENT_CONTROL_CLICK)
    call TriggerAddAction(trig, function ShowHideGrid)

endfunction