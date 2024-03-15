library ToggleIconButtonGroup initializer init_function requires ToggleIconButton 
// function ToggleIconButtonGroupGetValue takes integer groupObject, player p returns integer
// function ToggleIconButtonGroupClear takes integer groupObject, player p returns nothing
// function ToggleIconButtonGroupAddButton takes integer groupObject, integer buttonObject returns nothing
// function ToggleIconButtonGroupClearButton takes integer groupObject, framehandle parent, string iconPath returns framehandle
// function ToggleIconButtonGroupClearButtonSimple takes integer groupObject returns framehandle
// function CreateToggleIconButtonGroup takes code action returns integer
    globals
        // limitation all ToggleIconButton of a group have to take indexes next to each other
        public integer array IndexStart // first ToggleIconButton Index in a group
        public integer array IndexEnd // Last ToggleIconButton Inde in a group
        public integer array groupIndex // this is for the buttons not for the group
        public framehandle array ClearButton
        public framehandle array ClearButtonIcon
        public framehandle array ClearButtonIconPushed        

        public integer Count = 0

        // args for the Actions
        integer ToggleIconButtonGroup_Index
        integer ToggleIconButtonGroup_Value

        public trigger array Action


        private trigger Trigger = CreateTrigger()
        private trigger RightClickTrigger = CreateTrigger()
        private trigger ClearTrigger = CreateTrigger()
    endglobals

    
    function ToggleIconButtonGroupGetValue takes integer groupObject, player p returns integer
        local integer returnValue = 0
        local integer loopA = IndexStart[groupObject]
        loop
            exitwhen loopA > IndexEnd[groupObject]
            set returnValue = returnValue + ToggleIconButtonGetValue(loopA, p)
            set loopA = loopA + 1
        endloop
        return returnValue
    endfunction

    function ToggleIconButtonGroupClear takes integer groupObject, player p returns nothing
        local integer loopA = IndexStart[groupObject]
        loop
            exitwhen loopA > IndexEnd[groupObject]
            call ToggleIconButtonSetValue(loopA, p, false)
            set loopA = loopA + 1
        endloop
    endfunction
    
    private function GroupTriggerAction takes nothing returns nothing
        local integer groupObject = groupIndex[ToggleIconButton_Index]
        set ToggleIconButtonGroup_Index = groupObject
        set ToggleIconButtonGroup_Value = ToggleIconButtonGroupGetValue(groupObject, ToggleIconButton_Player)
        call TriggerEvaluate(Action[groupObject])
    endfunction


    function ToggleIconButtonGroupAddButton takes integer groupObject, integer buttonObject returns nothing
        if IndexStart[groupObject] == 0 then
            set IndexStart[groupObject] = buttonObject
        endif
        set IndexEnd[groupObject] = buttonObject
        call ToggleIconButtonAddAction(buttonObject, function GroupTriggerAction)
        set groupIndex[buttonObject] = groupObject
        call BlzTriggerRegisterFrameEvent(RightClickTrigger, ToggleIconButton_Button[buttonObject], FRAMEEVENT_MOUSE_UP)
    endfunction

    function ToggleIconButtonGroupClearButton takes integer groupObject, framehandle parent, string iconPath returns framehandle
        local framehandle but = BlzCreateFrame("TasItemShopCatButton", parent, 0, 0)
        local framehandle icon = BlzGetFrameByName("TasItemShopCatButtonBackdrop", 0)
        local framehandle iconPushed = BlzGetFrameByName("TasItemShopCatButtonBackdropPushed", 0)
        // only one clearButton
        if ClearButton[groupObject] == null then
            set but = BlzCreateFrame("TasItemShopCatButton", parent, 0, 0)
            set icon = BlzGetFrameByName("TasItemShopCatButtonBackdrop", 0)
            set iconPushed = BlzGetFrameByName("TasItemShopCatButtonBackdropPushed", 0)
            call BlzFrameSetSize(but, ToggleIconButton_DefaultSizeX, ToggleIconButton_DefaultSizeY)
            call BlzFrameSetTexture(icon, iconPath, 0, false)
            call BlzFrameSetTexture(iconPushed, iconPath, 0, false)
            call CreateSimpleTooltip(but, "Clear")
            call BlzFrameSetText(but, I2S(groupObject))
            set ClearButton[groupObject] = but
            set ClearButtonIcon[groupObject] = icon
            set ClearButtonIconPushed[groupObject] = iconPushed
            
            call BlzTriggerRegisterFrameEvent(ClearTrigger, but, FRAMEEVENT_CONTROL_CLICK)
            return but
        else
            return ClearButton[groupObject]
        endif
    endfunction

    function ToggleIconButtonGroupClearButtonSimple takes integer groupObject returns framehandle
        return ToggleIconButtonGroupClearButton(groupObject, BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "ReplaceableTextures\\CommandButtons\\BTNCancel")
    endfunction

    function CreateToggleIconButtonGroup takes code action returns integer
        set Count = Count + 1
        set Action[Count] = CreateTrigger()
        call TriggerAddCondition(Action[Count], Filter(action))
        return Count
    endfunction

    private function RightClickTriggerAction takes nothing returns nothing
        local player p = GetTriggerPlayer()
        local framehandle frame = BlzGetTriggerFrame()
        local integer buttonObject
        local integer groupObject
        if IsRightClick(p) then
            set buttonObject = S2I(BlzFrameGetText(frame))
            set groupObject = groupIndex[buttonObject]
            call StartSoundForPlayerBJ(p, ToggleIconButton_Sound)
            call ToggleIconButtonGroupClear(groupObject, p)
            call ToggleIconButtonSetValue(buttonObject, p, true)
            set ToggleIconButton_Index = buttonObject
            set ToggleIconButtonGroup_Index = groupObject
            set ToggleIconButton_Player = p
            set ToggleIconButtonGroup_Value = ToggleIconButtonGroupGetValue(groupObject, p)
            call TriggerEvaluate(Action[groupObject])
        endif
    endfunction

    private function ClearTriggerAction takes nothing returns nothing
        local framehandle but = BlzGetTriggerFrame()
        local integer groupObject = S2I(BlzFrameGetText(but))
        local player p = GetTriggerPlayer()
        call ToggleIconButtonGroupClear(groupObject, p)
        // remove focus
        call BlzFrameSetEnable(but, false)
        call BlzFrameSetEnable(but, true)

        set ToggleIconButton_Index = 0
        set ToggleIconButtonGroup_Index = groupObject
        set ToggleIconButton_Player = p
        set ToggleIconButtonGroup_Value = ToggleIconButtonGroupGetValue(groupObject, p)
        call TriggerEvaluate(Action[groupObject])
    endfunction

    private function init_function takes nothing returns nothing
        set Trigger = CreateTrigger()
        call TriggerAddAction(Trigger, function GroupTriggerAction)
        call TriggerAddAction(ClearTrigger, function ClearTriggerAction)
        call TriggerAddAction(RightClickTrigger, function RightClickTriggerAction)        
    endfunction
endlibrary
