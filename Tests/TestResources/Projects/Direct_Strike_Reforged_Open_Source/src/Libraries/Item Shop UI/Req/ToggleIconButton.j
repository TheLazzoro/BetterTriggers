library ToggleIconButton initializer Init requires Power2, SimpleTooltip
// function CreateToggleIconButton takes framehandle parent, integer valueOn, string text, string textureOn, integer mode, string textureOff, string textOff returns integer
// function CreateToggleIconButtonSimple takes framehandle parent, integer valueOn, string text, string textureOn returns integer
// function ToggleIconButtonSetValue takes integer object, player p, boolean enable returns nothing
// function ToggleIconButtonAddAction takes integer object, code action returns nothing
// function ToggleIconButtonGetValue takes integer object, player p returns integer
    globals
        public real DefaultSizeX = 0.024
        public real DefaultSizeY = 0.024
        public integer MODE_DEFAULT = 0 // the visual is local only.
        public integer MODE_SHARED = 1 // is the same for all players.
        public integer MODE_LOCAL = -1 // Visual and Action are for the clicking player only

        public integer Counter = 0
        public integer array ValueOn
        public integer array Value
        public framehandle array Button
        public framehandle array Icon
        public framehandle array IconPushed
        public framehandle array ToolTip
        public integer array Mode
        public string array Texture
        public string array TextureOff
        public string array Text
        public string array TextOff
        public trigger array Action

        integer ToggleIconButton_Index
        player ToggleIconButton_Player
        boolean ToggleIconButton_Enabled

        private trigger Trigger = CreateTrigger()
        public sound Sound
    endglobals
    private function GetDisabledIcon takes string icon returns string
        //ReplaceableTextures\CommandButtons\BTNHeroPaladin.tga -> ReplaceableTextures\CommandButtonsDisabled\DISBTNHeroPaladin.tga
        if SubString(icon, 34, 35) != "\\" then
            return icon
        endif //this string has not enough chars return it
        //string.len(icon) < 34 then return icon end //this string has not enough chars return it
        return SubString(icon, 0, 34) + "Disabled\\DIS" + SubString(icon, 35, StringLength(icon))
    endfunction
    function ToggleIconButtonGetKey takes integer object, player p returns integer
        if Mode[object] == MODE_SHARED then
            return 0
        else
            return GetPower2Value(GetPlayerId(p) + 1)
        endif
    endfunction

    function ToggleIconButtonSetValue takes integer object, player p, boolean enable returns nothing
        local integer key = ToggleIconButtonGetKey(object, p)
        if enable and BlzBitAnd(Value[object], key) == 0 then
            set Value[object] = Value[object] + key
        elseif not enable and BlzBitAnd(Value[object], key) > 0 then
            set Value[object] = Value[object] - key
        endif
        
        // update visual
        if Mode[object] == MODE_SHARED or GetLocalPlayer() == p then
            if not enable then
                call BlzFrameSetTexture(Icon[object], TextureOff[object], 0, false)
                call BlzFrameSetTexture(IconPushed[object], TextureOff[object], 0, false)
                call BlzFrameSetText(ToolTip[object], TextOff[object])
            else
                call BlzFrameSetTexture(Icon[object], Texture[object], 0, false)
                call BlzFrameSetTexture(IconPushed[object], Texture[object], 0, false)
                call BlzFrameSetText(ToolTip[object], Text[object])
            endif
        endif
    endfunction
    

    function ToggleIconButtonGetValue takes integer object, player p returns integer
        if BlzBitAnd(Value[object], ToggleIconButtonGetKey(object, p)) > 0 then
            return ValueOn[object]
        else
            return 0
        endif
    endfunction

    

    function ToggleIconButtonDefaultSize takes real x, real y returns nothing
        set DefaultSizeX = x
        set DefaultSizeY = y
    endfunction


    
    function CreateToggleIconButton takes framehandle parent, integer valueOn, string text, string textureOn, integer mode, string textureOff, string textOff returns integer
        set Counter = Counter + 1        
        set Button[Counter] = BlzCreateFrame("TasItemShopCatButton", parent, 0, 0)
        set Icon[Counter] = BlzGetFrameByName("TasItemShopCatButtonBackdrop", 0)
        set IconPushed[Counter] = BlzGetFrameByName("TasItemShopCatButtonBackdropPushed", 0)
        call BlzFrameSetText(Button[Counter], I2S(Counter))
        call BlzFrameSetSize(Button[Counter], DefaultSizeX, DefaultSizeY)
        call BlzFrameSetTexture(Icon[Counter], textureOff, 0, false)
        call BlzFrameSetTexture(IconPushed[Counter], textureOff, 0, false)
        
        call BlzTriggerRegisterFrameEvent(Trigger, Button[Counter], FRAMEEVENT_CONTROL_CLICK)

        set ToolTip[Counter] = CreateSimpleTooltip(Button[Counter], textOff)
        set Mode[Counter] = mode
        set Value[Counter] = 0
        set ValueOn[Counter] = valueOn
        set Texture[Counter] = textureOn
        set TextureOff[Counter] = textureOff
        set Text[Counter] = text
        set TextOff[Counter] = textOff
        set Action[Counter] = CreateTrigger()
        return Counter
    endfunction
    function ToggleIconButtonAddAction takes integer object, code action returns nothing
        call TriggerAddCondition(Action[object], Filter(action))
    endfunction
    function CreateToggleIconButtonSimple takes framehandle parent, integer valueOn, string text, string textureOn returns integer
        return CreateToggleIconButton(parent, valueOn, text, textureOn, MODE_DEFAULT, GetDisabledIcon(textureOn), text)
    endfunction

    private function TriggerAction takes nothing returns nothing
        local framehandle frame = BlzGetTriggerFrame()
        local integer object = S2I(BlzFrameGetText(frame))
        local player p = GetTriggerPlayer()
        local integer key = ToggleIconButtonGetKey(object, p)
        //StartSoundForPlayerBJ(player, ToggleIconButton.Sound)
        
        call ToggleIconButtonSetValue(object, p, BlzBitAnd(Value[object], key) == 0)
        if (Mode[object] != MODE_LOCAL or GetLocalPlayer() == p) then
            set ToggleIconButton_Index = object
            set ToggleIconButton_Player = p
            set ToggleIconButton_Enabled = ToggleIconButtonGetValue(object, p) == ValueOn[object]
            call TriggerEvaluate(Action[object])
        endif
        // remove focus
        call BlzFrameSetEnable(frame, false)
        call BlzFrameSetEnable(frame, true)
    endfunction
    private function Init takes nothing returns nothing
        set Sound = CreateSound("Sound\\Interface\\MouseClick1.wav", false, false, false, 10, 10, "")
        call SetSoundParamsFromLabel(Sound, "InterfaceClick")
        call SetSoundDuration(Sound, 239)
        call BlzLoadTOCFile("war3mapImported\\Templates.toc")
        call TriggerAddAction(Trigger, function TriggerAction)        
    endfunction
endlibrary