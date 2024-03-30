library String initializer Init
//String functions v1.04
//made by MaskedPoptart
//--------------------IMPORTANT FUNCTIONS------------------------
  
    function FindIndexFrom takes string mainString, string stringToFind, integer startingIndex returns integer
        local integer msLength = StringLength(mainString)
        local integer sfLength = StringLength(stringToFind)
        local integer i = startingIndex
        if(sfLength > msLength or i < 0)then
            return -1
        endif
        loop
            exitwhen i > msLength - sfLength
            if(SubString(mainString, i, i+sfLength) == stringToFind)then
                return i
            endif
            set i = i + 1
        endloop
        return -1
    endfunction

    function FindIndex takes string mainString, string stringToFind returns integer
        return FindIndexFrom(mainString, stringToFind, 0)
    endfunction
  
    function FindLastIndexFrom takes string mainString, string stringToFind, integer startingIndex returns integer
        local integer msLength = StringLength(mainString)
        local integer sfLength = StringLength(stringToFind)
        local integer i = msLength-sfLength
        if(startingIndex < i)then
            set i = startingIndex
        endif
        if(sfLength > msLength)then
            return -1
        endif
        loop
            exitwhen i < 0
            if(SubString(mainString, i, i+sfLength) == stringToFind)then
                return i
            endif
            set i = i - 1
        endloop
        return -1
    endfunction

    function FindLastIndex takes string mainString, string stringToFind returns integer
        return FindLastIndexFrom(mainString, stringToFind, 2147483647)
    endfunction
  
//-----------------------COLOR FUNCTIONS ------------------------
    globals
        private playercolor array PLAYER_COLORS
        private string array PLAYER_COLOR_STRINGS
        private constant string HEX_CHARS = "0123456789abcdef"
        private string COLOR_ENDING = "|r"
    endglobals

    private function Init takes nothing returns nothing
        local integer i = 0
        loop
            exitwhen i >= 12
            set PLAYER_COLORS[i] = ConvertPlayerColor(i)
            set i = i + 1
        endloop
        set PLAYER_COLOR_STRINGS[0] = "|cffff0303"
        set PLAYER_COLOR_STRINGS[1] = "|cff0042ff"
        set PLAYER_COLOR_STRINGS[2] = "|cff1ce6b9"
        set PLAYER_COLOR_STRINGS[3] = "|cff540081"
        set PLAYER_COLOR_STRINGS[4] = "|cfffffc01"
        set PLAYER_COLOR_STRINGS[5] = "|cfffe8a0e"
        set PLAYER_COLOR_STRINGS[6] = "|cff20c000"
        set PLAYER_COLOR_STRINGS[7] = "|cffe55bb0"
        set PLAYER_COLOR_STRINGS[8] = "|cff959697"
        set PLAYER_COLOR_STRINGS[9] = "|cff7ebff1"
        set PLAYER_COLOR_STRINGS[10] = "|cff106246"
        set PLAYER_COLOR_STRINGS[11] = "|cff4e2a04"
        set PLAYER_COLOR_STRINGS[12] = "|cff272727"
        set PLAYER_COLOR_STRINGS[13] = "|cff272727"
        set PLAYER_COLOR_STRINGS[14] = "|cff272727"
    endfunction

    function PlayerColor2ColorString takes playercolor pc returns string
        local integer i = 0
        loop
            exitwhen i >= 12
            if(PLAYER_COLORS[i] == pc)then
                return PLAYER_COLOR_STRINGS[i]
            endif
            set i = i + 1
        endloop
        return PLAYER_COLOR_STRINGS[12]
    endfunction

    function GetPlayerColorString takes player p returns string
        return PlayerColor2ColorString(GetPlayerColor(p))
    endfunction

    function GetPlayerNameColored takes player p returns string
        return GetPlayerColorString(p)+GetPlayerName(p)+COLOR_ENDING
    endfunction


    //please use responsibly
    function RemoveColorCode takes string mainString returns string
        local integer msLength = StringLength(mainString)
        if(msLength<12)then
            return mainString
        endif
        return SubString(mainString, 10, msLength-2)
    endfunction

    function IBase2S takes integer base10Num, integer newBase returns string
        local integer placeNum //number at current place
        local string newBaseString = ""
        loop
            exitwhen base10Num == 0
            set placeNum = ModuloInteger(base10Num, newBase)
            set newBaseString = SubString(HEX_CHARS, placeNum, placeNum+1) + newBaseString
            set base10Num = base10Num / newBase
        endloop
        if(newBaseString == "")then
            return "0"
        endif
        return newBaseString
    endfunction

    function SBase2I takes string oldBaseString, integer oldBase returns integer
        local integer base10Num = 0
        local integer placeNum //number at current place
        local integer placeIndex = 0 //index of current place. 0 = one's place, etc.
        local integer i = StringLength(oldBaseString)-1
        loop
            exitwhen i < 0
            set placeNum = FindLastIndexFrom(HEX_CHARS, SubString(oldBaseString, i, i+1), oldBase-1)
            set base10Num = base10Num + placeNum*R2I(Pow(oldBase, placeIndex))
            set placeIndex = placeIndex + 1
            set i = i - 1
        endloop
        return base10Num
    endfunction

    function ConvertRGBToColorString takes integer red, integer green, integer blue returns string
        local string RR
        local string GG
        local string BB
        if(red>255)then
            set red = 255
        endif
        if(green>255)then
            set green = 255
        endif
        if(blue>255)then
            set blue = 255
        endif
        set RR = IBase2S(red, 16)
        set GG = IBase2S(green, 16)
        set BB = IBase2S(blue, 16)
        if(StringLength(RR)<2)then
            set RR = "0"+RR
        endif
        if(StringLength(GG)<2)then
            set GG = "0"+GG
        endif
        if(StringLength(BB)<2)then
            set BB = "0"+BB
        endif
        return "|cff"+RR+GG+BB
    endfunction

    function GetColoredString takes string str, integer r, integer g, integer b returns string
        return ConvertRGBToColorString(r,g,b)+str+COLOR_ENDING
    endfunction


//----------------------CHAT EVENT FUNCTIONS------------------------------

    function RemoveString takes string mainString, string toRemove returns string
        local integer i = 0
        local string currentString
        local integer msLength = StringLength(mainString)
        local integer trLength = StringLength(toRemove)
        if(trLength > msLength)then
            return mainString
        endif
        loop
            exitwhen i+trLength > msLength
            set currentString = SubString(mainString, i, i+trLength)
            if(currentString == toRemove)then
                if(i+trLength <= msLength)then
                    set mainString = SubString(mainString, 0, i)+SubString(mainString, i+trLength, msLength)
                else
                    set mainString = SubString(mainString, 0, i)
                endif
                set i = i - trLength
            endif
            set i = i + 1
        endloop
        return mainString
    endfunction

    function NumOccurances takes string mainString, string stringToFind returns integer
        local integer count = 0
        local integer i = 0
        local integer msLength = StringLength(mainString)
        local integer sfLength = StringLength(stringToFind)
        loop
            exitwhen (i+sfLength) > msLength
            if(SubString(mainString, i, i+sfLength) == stringToFind)then
                set count = count + 1
            endif
            set i = i + 1
        endloop
        return count
    endfunction

    function S2B takes string word returns boolean
        if(word == "true")then
            return true
        endif
        return false
    endfunction 

    function S2Player takes string word returns player
        return Player(S2I(SubString(word, 1, StringLength(word))))
    endfunction
  
    globals
        private integer MIN_RAW_CODE = ' ' //32
        private string RAW_CHARS = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~"
    endglobals
    function S2RawCode takes string str returns integer
        local integer rawCode = 0
        local integer placeNum //number at current place
        local integer placeIndex = 0 //index of current place. 0 = one's place, etc.
        local integer i = StringLength(str)-1
        loop
            exitwhen i < 0
            set placeNum = MIN_RAW_CODE + FindIndex(RAW_CHARS, SubString(str, i, i+1))
            //the char at index 0 of RAW_CHARS has ASCII value 32, so we need to offset each FindIndex by 32.
            set rawCode = rawCode + placeNum*R2I(Pow(256., placeIndex))
            set placeIndex = placeIndex + 1
            set i = i - 1
        endloop
        return rawCode
    endfunction

//-----------------------DEBUG FUNCTIONS-------------------------

    function B2S takes boolean bool returns string
        if(bool)then
            return "true"
        endif
        return "false"
    endfunction

    function Player2S takes player p returns string
        return "Player("+I2S(GetPlayerId(p))+")"
    endfunction

    function Unit2S takes unit u returns string
        return GetUnitName(u)+ "_"+I2S(GetHandleId(u)-0x100000)
    endfunction

    function RawCode2S takes integer rawCode returns string
        local integer placeNum //number at current place
        local string str = ""
        if(rawCode < MIN_RAW_CODE)then
            return str
        endif
        loop
            exitwhen rawCode == 0
            set placeNum = ModuloInteger(rawCode, 256) - MIN_RAW_CODE
            set str = SubString(RAW_CHARS, placeNum, placeNum+1) + str
            set rawCode = rawCode / 256
        endloop
        return str
    endfunction
endlibrary