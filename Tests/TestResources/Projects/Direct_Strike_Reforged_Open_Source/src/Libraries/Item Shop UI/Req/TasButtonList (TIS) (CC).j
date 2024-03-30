native GetUnitGoldCost takes integer rawCode returns integer
native GetUnitWoodCost takes integer rawCode returns integer

library TasButtonList initializer Init uses String, IsRightClick, optional FrameLoader, optional SimpleTooltip
// TasButtonList10b (CC) by Tasyen

//function CreateTasButtonList10 takes string buttonName, integer cols, integer rows, framehandle parent, code buttonAction, code rightClickAction, code updateAction, code searchAction, code filterAction, code asyncAction, code asyncRigthAction, real colGap, real rowGap returns integer
//function CreateTasButtonListEx takes string buttonName, integer cols, integer rows, framehandle parent, code buttonAction, code rightClickAction, code updateAction, code searchAction, code filterAction returns integer
 //create a new List
 //parent is the container of this Frame it will attach itself to its TOP.
 //the given functions are called over Triggers
 //buttonAction is the function that executes when an option is clicked.
 //when your data are unit-RawCodes then you can skip updateAction & searchAction.
 //updateAction runs for each Button and is used to set the diplayed content.

 //searchAction is a function that returns true if the current data matches the searchText.
 //filterAction is meant to be used when one wants an addtional non text based filtering, with returning true allowing data or false rejecting it.
 //searchAction , udateAction & filterAction are async this functions should not do anything that alters the game state/flow.

//function CreateTasButtonList takes integer buttonCount, framehandle parent, code buttonAction, code updateAction, code searchAction, code filterAction returns integer
  // wrapper for CreateTasButtonListEx, 1 col, buttonCount rows.
//function CreateTasButtonListV2 takes integer rowCount, framehandle parent, code buttonAction, code updateAction, code searchAction, code filterAction returns integer
  //  2 Buttons each Row, takes more Height then the other Versions
//function CreateTasButtonListV3 takes integer rowCount, framehandle parent, code buttonAction, code updateAction, code searchAction, code filterAction returns integer
  //  3 Buttons each Row, only Icon, and Costs

//function TasButtonListClearDataEx takes integer listIndex, integer playerIndex returns nothing
//function TasButtonListClearData takes integer listIndex returns nothing
  //  remove all data
//function TasButtonListRemoveDataEx takes integer listIndex, integer data, integer playerIndex returns nothing
//function TasButtonListRemoveData takes integer listIndex, integer data returns nothing
  //  search for data and remove it
//function TasButtonListAddDataEx takes integer listIndex, integer data, integer playerIndex returns nothing
//function TasButtonListAddData takes integer listIndex, integer data returns nothing
  //  add data for one Button
//function TasButtonListCopyDataEx takes integer writeObject, integer readObject, integer playerIndex returns nothing
//function TasButtonListCopyData takes integer writeObject, integer readObject returns nothing
  //  writeObject uses the same data as readObject and calls UpdateButtonList.
//function UpdateTasButtonList takes integer listIndex returns nothing
  //  update the displayed Content should be done after Data was added or removed was used.
//function TasButtonListSearch takes integer listIndex, string text returns nothing
    // The buttonList will search it's data for the given text, if nil is given as text it will search for what the user currently has in its box.
    // This will also update the buttonList

globals
    //args for custom user actions
    integer TasButtonListData = 0
    string TasButtonListText = ""
    boolean TasButtonListIsSearching = false
    integer TasButtonListIndex = 0
    framehandle TasButtonListFrame = null

    // System 
    public hashtable Hash = InitHashtable()
    private integer Counter = 0 //amount of Lists created, each index is one List
    private trigger SyncTrigger = CreateTrigger()
    private trigger ButtonTrigger = CreateTrigger()    
    private trigger SearchTrigger = CreateTrigger()
    private trigger ButtonScrollTrigger = CreateTrigger()
    private trigger SliderTrigger = CreateTrigger()
    private trigger SyncRightTrigger = CreateTrigger()
    private trigger ButtonRightTrigger = CreateTrigger()
    private sound RightClickSound

    // ButtonLists
    framehandle array TasButtonListSlider
    framehandle array TasButtonListSliderText
    framehandle array TasButtonListParent
    framehandle array TasButtonListInputFrame
    framehandle array TasButtonListSyncFrame
    framehandle array TasButtonListSyncFrameRight
    integer array TasButtonListButtonCount
    integer array TasButtonListCreateContext
    string array TasButtonListButtonName    
    integer array TasButtonListStepSize
    trigger array TasButtonListButtonAction
    trigger array TasButtonListRightAction
    trigger array TasButtonListUpdateAction
    trigger array TasButtonListSearchAction
    trigger array TasButtonListFilterAction
    trigger array TasButtonListAsyncAction
    trigger array TasButtonListAsyncRightAction
    
    integer array TasButtonListViewPoint
    location array TasButtonListDataList
    location array TasButtonListDataListFiltered

    real TasButtonListGapCol = 0.0
    real TasButtonListGapRow = 0.0
    public integer CreateContextCounter = 0

endglobals


// update the shown content
function UpdateTasButtonList takes integer listIndex returns nothing
    local integer dataHash = GetHandleId(LoadLocationHandle(Hash, GetHandleId(TasButtonListDataList[listIndex]), GetPlayerId(GetLocalPlayer())))
    local integer filteredDataHash = GetHandleId(TasButtonListDataListFiltered[listIndex])
    local integer dataFilteredCount = LoadInteger(Hash, filteredDataHash, 0)
    
    local integer i = 1
    set TasButtonListIndex = listIndex
    call BlzFrameSetVisible(TasButtonListSlider[listIndex], dataFilteredCount > TasButtonListButtonCount[listIndex])
    loop
        exitwhen i > TasButtonListButtonCount[listIndex]
        set TasButtonListFrame = BlzGetFrameByName(TasButtonListButtonName[listIndex], TasButtonListCreateContext[listIndex] +  i)
        
        if dataFilteredCount >= i  then
            set TasButtonListData = LoadInteger(Hash, dataHash, LoadInteger(Hash, filteredDataHash, i + TasButtonListViewPoint[listIndex]))
            call TriggerEvaluate(TasButtonListUpdateAction[listIndex])
            call BlzFrameSetVisible(TasButtonListFrame, true)
        else
            call BlzFrameSetVisible(TasButtonListFrame, false)
        endif
        set i = i + 1
    endloop

endfunction

function TasButtonListSearch takes integer listIndex, string text returns nothing
    local integer filteredDataHash = GetHandleId(TasButtonListDataListFiltered[listIndex])
    local integer dataHash = GetHandleId(LoadLocationHandle(Hash, GetHandleId(TasButtonListDataList[listIndex]), GetPlayerId(GetLocalPlayer())))
    local integer filteredDataCount

    local integer i
    local integer iEnd
    if text == null or text == "" then
        set text = BlzFrameGetText(TasButtonListInputFrame[listIndex])
    endif
    if GetLocalPlayer() == GetTriggerPlayer() then
        set TasButtonListText = text
        set TasButtonListIndex = listIndex
        call FlushChildHashtable(Hash, filteredDataHash)
        set filteredDataCount = 0
        if text != "" then
            set TasButtonListIsSearching = true
            set iEnd = LoadInteger(Hash, dataHash, 0)
            set i = 1
            loop
                exitwhen i > iEnd
                set TasButtonListData = LoadInteger(Hash, dataHash, i)
                if TriggerEvaluate(TasButtonListSearchAction[listIndex]) and TriggerEvaluate(TasButtonListFilterAction[listIndex]) then
                    set filteredDataCount = filteredDataCount + 1
                    call SaveInteger(Hash, filteredDataHash, filteredDataCount, i)
                endif
                set i = i + 1
            endloop
            call SaveInteger(Hash, filteredDataHash, 0, filteredDataCount)
        else
            set TasButtonListIsSearching = false
            set iEnd = LoadInteger(Hash, dataHash, 0)
            set i = 1
            loop
                exitwhen i > iEnd
                set TasButtonListData = LoadInteger(Hash, dataHash, i)
                if TriggerEvaluate(TasButtonListFilterAction[listIndex]) then
                    set filteredDataCount = filteredDataCount + 1
                    call SaveInteger(Hash, filteredDataHash, filteredDataCount, i)
                endif
                set i = i + 1
            endloop
            call SaveInteger(Hash, filteredDataHash, 0, filteredDataCount)
        endif
        

        //update Slider, with that also update
        call BlzFrameSetMinMaxValue(TasButtonListSlider[listIndex], TasButtonListButtonCount[listIndex], filteredDataCount)
        call BlzFrameSetValue(TasButtonListSlider[listIndex], 999999)
    endif
endfunction

function TasButtonListTriggerActionSync takes nothing returns nothing
    local integer listIndex = LoadInteger(Hash, GetHandleId(BlzGetTriggerFrame()), 0)
    local integer dataIndex = R2I(BlzGetTriggerFrameValue() + 0.5)

    set TasButtonListData = LoadInteger(Hash, GetHandleId(LoadLocationHandle(Hash, GetHandleId(TasButtonListDataList[listIndex]), GetPlayerId(GetTriggerPlayer()))), dataIndex)
    set TasButtonListIndex = listIndex
    call TriggerExecute(TasButtonListButtonAction[listIndex])

    call UpdateTasButtonList(listIndex)
endfunction

function TasButtonListTriggerActionButton takes nothing returns nothing
    local framehandle frame = BlzGetTriggerFrame()
    local integer buttonIndex = LoadInteger(Hash, GetHandleId(frame), 1)
    local integer listIndex = LoadInteger(Hash, GetHandleId(frame), 0)
    local integer dataIndex = LoadInteger(Hash, GetHandleId(TasButtonListDataListFiltered[listIndex]), buttonIndex + TasButtonListViewPoint[listIndex])
    local integer data = LoadInteger(Hash, GetHandleId(LoadLocationHandle(Hash, GetHandleId(TasButtonListDataList[listIndex]), GetPlayerId(GetTriggerPlayer()))), dataIndex)
    call BlzFrameSetEnable(frame, false)
    call BlzFrameSetEnable(frame, true)
    set TasButtonListData = data
    set TasButtonListIndex = listIndex
    set TasButtonListFrame = frame
    if GetLocalPlayer() == GetTriggerPlayer() then
       call TriggerEvaluate(TasButtonListAsyncAction[listIndex])
       call BlzFrameSetValue(TasButtonListSyncFrame[listIndex], dataIndex)
    endif
    set frame = null
endfunction

function TasButtonListTriggerActionSearch takes nothing returns nothing
    call TasButtonListSearch(LoadInteger(Hash, GetHandleId(BlzGetTriggerFrame()), 0), null)
endfunction

// scrolling while pointing on Buttons
function TasButtonListTriggerActionButtonScroll takes nothing returns nothing
    local integer listIndex = LoadInteger(Hash, GetHandleId(BlzGetTriggerFrame()), 0)
    local framehandle frame = TasButtonListSlider[listIndex]

    if GetLocalPlayer() == GetTriggerPlayer() then
        if BlzGetTriggerFrameValue() > 0 then
            call BlzFrameSetValue(frame, BlzFrameGetValue(frame) + TasButtonListStepSize[listIndex])
        else
            call BlzFrameSetValue(frame, BlzFrameGetValue(frame) - TasButtonListStepSize[listIndex])
        endif
    endif
    set frame = null
endfunction

// scrolling while pointing on slider aswell as calling
function TasButtonListTriggerActionSlider takes nothing returns nothing
    local integer listIndex = LoadInteger(Hash, GetHandleId(BlzGetTriggerFrame()), 0)
    local integer filteredDataHash = GetHandleId(TasButtonListDataListFiltered[listIndex])
    local integer dataFilteredCount = LoadInteger(Hash, filteredDataHash, 0)

    local framehandle frame = BlzGetTriggerFrame()
    if GetLocalPlayer() == GetTriggerPlayer() then
        if BlzGetTriggerFrameEvent() == FRAMEEVENT_MOUSE_WHEEL then
            if BlzGetTriggerFrameValue() > 0 then
                call BlzFrameSetValue(frame, BlzFrameGetValue(frame) + TasButtonListStepSize[listIndex])
            else
                call BlzFrameSetValue(frame, BlzFrameGetValue(frame) - TasButtonListStepSize[listIndex])
            endif
        else
            // when there is enough data use viewPoint. the Viewpoint is reduced from the data to make top being top.
            if dataFilteredCount > TasButtonListButtonCount[listIndex] then
                set TasButtonListViewPoint[listIndex] = dataFilteredCount - R2I(BlzGetTriggerFrameValue())
            else
                set TasButtonListViewPoint[listIndex] = 0
            endif

            if TasButtonListSliderText[listIndex] != null  then
                call BlzFrameSetText(TasButtonListSliderText[listIndex], I2S(R2I(0.5 + dataFilteredCount - BlzFrameGetValue(frame))) + "/" + I2S(R2I(0.5 + dataFilteredCount - TasButtonListButtonCount[listIndex])))
            endif
            call UpdateTasButtonList(listIndex)
        endif
    endif
    set frame = null
endfunction

// runs once for each button shown
function UpdateTasButtonListDefaultObject takes nothing returns nothing
//TasButtonListFrame
//TasButtonListData
//TasButtonListIndex
    local integer frameHandle = GetHandleId(TasButtonListFrame)
    local integer data = TasButtonListData
    local integer buttonIndex = LoadInteger(Hash, frameHandle, 1)
    local integer listIndex = LoadInteger(Hash, frameHandle, 0)
    local integer lumber
    local integer gold
    local integer context = TasButtonListCreateContext[listIndex] + buttonIndex

    call BlzFrameSetTexture(BlzGetFrameByName("TasButtonIcon", context),  BlzGetAbilityIcon(data), 0, false)
    call BlzFrameSetText(BlzGetFrameByName("TasButtonText", context), GetObjectName(data))

    call BlzFrameSetTexture(BlzGetFrameByName("TasButtonListTooltipIcon", context), BlzGetAbilityIcon(data), 0, false)
    call BlzFrameSetText(BlzGetFrameByName("TasButtonListTooltipName", context), GetObjectName(data))
    call BlzFrameSetText(BlzGetFrameByName("TasButtonListTooltipText", context), BlzGetAbilityExtendedTooltip(data, 0))

    if not IsUnitIdType(data, UNIT_TYPE_HERO) then
        // GetUnitWoodCost GetUnitGoldCost CRASH with heroes
        set lumber = GetUnitWoodCost(data)
        set gold = GetUnitGoldCost(data)
        if GetPlayerState(GetLocalPlayer(), PLAYER_STATE_RESOURCE_GOLD) >= gold then
            call BlzFrameSetText(BlzGetFrameByName("TasButtonTextGold", context), I2S(GetUnitGoldCost(data)))
        else
            call BlzFrameSetText(BlzGetFrameByName("TasButtonTextGold", context), "|cffff2010" + I2S(GetUnitGoldCost(data)))
        endif
        
        if GetPlayerState(GetLocalPlayer(), PLAYER_STATE_RESOURCE_LUMBER) >= lumber then
           call BlzFrameSetText(BlzGetFrameByName("TasButtonTextLumber", context), I2S(GetUnitWoodCost(data)))
        else
           call BlzFrameSetText(BlzGetFrameByName("TasButtonTextLumber", context), "|cffff2010" + I2S(GetUnitWoodCost(data)))
        endif
    else
        call BlzFrameSetText(BlzGetFrameByName("TasButtonTextLumber", context), "0")
        call BlzFrameSetText(BlzGetFrameByName("TasButtonTextGold", context), "0")
    endif
endfunction

function SearchTasButtonListDefaultObject takes nothing returns boolean
//TasButtonListText
//TasButtonListData
//TasButtonListIndex
    return FindIndex(GetObjectName(TasButtonListData), TasButtonListText) >= 0
endfunction

function InitTasButtonListObject8c takes framehandle parent, code buttonAction, code rightClickAction, code updateAction, code searchAction, code filterAction, code asyncAction, code asyncRigthAction returns integer
    local framehandle frame
    local integer playerIndex = 0
    set Counter = Counter + 1
    // the locations are created to have an unique slot in the hash which are used as something like a Lua table.
    set TasButtonListDataList[Counter] = Location(0, 0) //
    // each player also got an own list
    loop
        call SaveLocationHandle(Hash, GetHandleId(TasButtonListDataList[Counter]), playerIndex, Location(0,0))
        set playerIndex = playerIndex + 1
        exitwhen playerIndex == bj_MAX_PLAYER_SLOTS
    endloop
    set TasButtonListDataListFiltered[Counter] = Location(0, 0) //
    set TasButtonListParent[Counter] = parent
    set TasButtonListViewPoint[Counter] = 0

    set TasButtonListButtonAction[Counter] = CreateTrigger() //call this inside the SyncAction after a button is clicked
    set TasButtonListUpdateAction[Counter] = CreateTrigger() //function defining how to display stuff (async)
    set TasButtonListFilterAction[Counter] = CreateTrigger() //function to return the searched Text (async)
    set TasButtonListSearchAction[Counter] = CreateTrigger()
    set TasButtonListRightAction[Counter] = CreateTrigger()
    set TasButtonListAsyncAction[Counter] = CreateTrigger()
    set TasButtonListAsyncRightAction[Counter] = CreateTrigger()
    
    call TriggerAddAction(TasButtonListButtonAction[Counter], buttonAction)
    if rightClickAction != null then
        call TriggerAddAction(TasButtonListRightAction[Counter], rightClickAction)
    endif
    
    // update is a condition with it can be run with TriggerEvaluate in localPlayer code. TriggerExecute would desync
    if updateAction == null then
        call TriggerAddCondition(TasButtonListUpdateAction[Counter], Filter(function UpdateTasButtonListDefaultObject))
    else
        call TriggerAddCondition(TasButtonListUpdateAction[Counter], Filter(updateAction))
    endif

    if searchAction == null then
        call TriggerAddCondition(TasButtonListSearchAction[Counter], Filter(function SearchTasButtonListDefaultObject))
    else
        call TriggerAddCondition(TasButtonListSearchAction[Counter], Filter(searchAction))
    endif
    if filterAction != null then
        call TriggerAddCondition(TasButtonListFilterAction[Counter], Filter(filterAction))
    endif

    if asyncAction != null then
        call TriggerAddCondition(TasButtonListAsyncAction[Counter], Filter(asyncAction))
    endif

    if asyncRigthAction != null then
        call TriggerAddCondition(TasButtonListAsyncRightAction[Counter], Filter(asyncRigthAction))
    endif
    
    set frame = BlzCreateFrameByType("SLIDER", "", parent, "", 0)
    set TasButtonListSyncFrame[Counter] = frame
    call BlzFrameSetMinMaxValue(frame, 0, 9999999)
    call BlzFrameSetStepSize(frame, 1.0)
    call BlzTriggerRegisterFrameEvent(SyncTrigger, frame, FRAMEEVENT_SLIDER_VALUE_CHANGED)
    call BlzFrameSetVisible(frame, false)
    call SaveInteger(Hash, GetHandleId(frame), 0, Counter)

    set frame = BlzCreateFrameByType("SLIDER", "", parent, "", 0)
    set TasButtonListSyncFrameRight[Counter] = frame
    call BlzFrameSetMinMaxValue(frame, 0, 9999999)
    call BlzFrameSetStepSize(frame, 1.0)
    call BlzTriggerRegisterFrameEvent(SyncRightTrigger, frame, FRAMEEVENT_SLIDER_VALUE_CHANGED)
    call BlzFrameSetVisible(frame, false)
    call SaveInteger(Hash, GetHandleId(frame), 0, Counter)

    set frame = BlzCreateFrame("TasEditBox", parent, 0, 0)
    set TasButtonListInputFrame[Counter] = frame
    call BlzTriggerRegisterFrameEvent(SearchTrigger, frame, FRAMEEVENT_EDITBOX_TEXT_CHANGED)
    call BlzFrameSetPoint(frame, FRAMEPOINT_TOPRIGHT, parent, FRAMEPOINT_TOPRIGHT, 0, 0)
    call SaveInteger(Hash, GetHandleId(frame), 0, Counter)

    set frame = null
    return Counter
endfunction

// this should have beend called InitTasButtonListObject8
function InitTasButtonListObjectEx takes framehandle parent, code buttonAction, code rightClickAction, code updateAction, code searchAction, code filterAction returns integer
    return InitTasButtonListObject8c(parent, buttonAction, rightClickAction, updateAction, searchAction, filterAction, null, null)
endfunction

function InitTasButtonListObject takes framehandle parent, code buttonAction, code updateAction, code searchAction, code filterAction returns integer
    return InitTasButtonListObjectEx(parent, buttonAction, null, updateAction, searchAction, filterAction)
endfunction

function InitTasButtonListSlider10a takes integer listIndex, integer stepSize, integer rowCount, real colGap, real rowGap returns nothing
    local framehandle frame = BlzCreateFrameByType("SLIDER", "FrameListSlider", TasButtonListParent[listIndex], "QuestMainListScrollBar", 0)
    local framehandle buttonFrame = BlzGetFrameByName(TasButtonListButtonName[listIndex], TasButtonListCreateContext[listIndex] + stepSize)
    set TasButtonListSlider[listIndex] = frame
    call SaveInteger(Hash, GetHandleId(frame), 0, listIndex) // the slider nows the TasButtonListobject
    set TasButtonListStepSize[listIndex] = stepSize
    
    call BlzFrameSetStepSize(frame, stepSize)
    call BlzFrameClearAllPoints(frame)
    call BlzFrameSetVisible(frame, true)
    call BlzFrameSetMinMaxValue(frame, 0, 0)
    call BlzFrameSetPoint(frame, FRAMEPOINT_TOPLEFT, buttonFrame, FRAMEPOINT_TOPRIGHT, 0, 0)
    call BlzFrameSetSize(frame, 0.012, BlzFrameGetHeight(buttonFrame) * rowCount + rowGap * (rowCount - 1))
    call BlzTriggerRegisterFrameEvent(SliderTrigger, frame , FRAMEEVENT_SLIDER_VALUE_CHANGED)
    call BlzTriggerRegisterFrameEvent(SliderTrigger, frame , FRAMEEVENT_MOUSE_WHEEL)

    static if LIBRARY_SimpleTooltip then
        set TasButtonListSliderText[listIndex] = CreateSimpleTooltip(TasButtonListSlider[listIndex], "1000/1000")
        call BlzFrameClearAllPoints(TasButtonListSliderText[listIndex])
        call BlzFrameSetPoint(TasButtonListSliderText[listIndex], FRAMEPOINT_BOTTOMRIGHT, TasButtonListSlider[listIndex], FRAMEPOINT_TOPLEFT, 0, 0)
    endif
    
endfunction

function InitTasButtonListSlider takes integer listIndex, integer stepSize, integer rowCount returns nothing
    call InitTasButtonListSlider10a(listIndex, stepSize, rowCount, 0, 0)
endfunction

//Demo Creators
function CreateTasButtonTooltip takes framehandle frameButton, framehandle parent, integer createContext returns nothing
    local framehandle frameParent = BlzCreateFrame("TasButtonListTooltipBoxFrame", frameButton, 0, createContext)
    local framehandle frame = BlzGetFrameByName("TasButtonListTooltipBox", createContext)
    local framehandle frameText = BlzGetFrameByName("TasButtonListTooltipText", createContext)
    call BlzGetFrameByName("TasButtonListTooltipIcon", createContext)
    call BlzGetFrameByName("TasButtonListTooltipName", createContext)
    call BlzGetFrameByName("TasButtonListTooltipSeperator", createContext)
    call BlzFrameSetTooltip(frameButton, frameParent)

    call BlzFrameSetPoint(frameText, FRAMEPOINT_TOPRIGHT, parent, FRAMEPOINT_TOPLEFT, -0.001, -0.052)
    call BlzFrameSetPoint(frame, FRAMEPOINT_TOPLEFT, BlzGetFrameByName("TasButtonListTooltipIcon", createContext), FRAMEPOINT_TOPLEFT, -0.005, 0.005)
    call BlzFrameSetPoint(frame, FRAMEPOINT_BOTTOMRIGHT, frameText, FRAMEPOINT_BOTTOMRIGHT, 0.005, -0.005)
endfunction

function CreateTasButtonList10 takes string buttonName, integer cols, integer rows, framehandle parent, code buttonAction, code rightClickAction, code updateAction, code searchAction, code filterAction, code asyncAction, code asyncRigthAction, real colGap, real rowGap returns integer
    local integer buttonCount = rows*cols
    local integer listIndex = InitTasButtonListObject8c(parent, buttonAction, rightClickAction, updateAction, searchAction, filterAction, asyncAction, asyncRigthAction)
    local integer i = 1
    local framehandle frame
    local integer frameHandle 

    local integer rowRemain = cols
    set TasButtonListButtonName[listIndex] = buttonName
    set TasButtonListCreateContext[listIndex] = CreateContextCounter
    set TasButtonListButtonCount[listIndex] = buttonCount
    loop
        exitwhen i > buttonCount
        set CreateContextCounter = CreateContextCounter + 1    
        set frame = BlzCreateFrame(buttonName, parent, 0, CreateContextCounter)
        set frameHandle = GetHandleId(frame)
        if frameHandle == 0 then
            call BJDebugMsg("TasButtonList - Error - can't create Button:" + buttonName)
            //return 0
        endif
        call SaveInteger(Hash, frameHandle, 1, i)
        call SaveInteger(Hash, frameHandle, 0, listIndex)
        
        call BlzFrameSetText(frame, I2S(i))
        // for some reason in a lan test with same Pc the BlzFrameGetText was not set for the second User.
        // restarting warcraft 3 fixed it.
        //call BJDebugMsg("TasButtonList " + I2S(i) + " "+ BlzFrameGetText(frame))
        //call SaveInteger(Hash, frameHandle, 2, CreateContextCounter)
        call BlzTriggerRegisterFrameEvent(ButtonTrigger, frame, FRAMEEVENT_CONTROL_CLICK)
        call BlzTriggerRegisterFrameEvent(ButtonRightTrigger, frame, FRAMEEVENT_MOUSE_UP)
        call BlzTriggerRegisterFrameEvent(ButtonScrollTrigger, frame, FRAMEEVENT_MOUSE_WHEEL)
        //give these handleIds to no desync when calling them in a async manner
        call BlzGetFrameByName("TasButtonIcon", CreateContextCounter)
        call BlzGetFrameByName("TasButtonText", CreateContextCounter)
        call BlzGetFrameByName("TasButtonIconGold", CreateContextCounter)
        call BlzGetFrameByName("TasButtonTextGold", CreateContextCounter)
        call BlzGetFrameByName("TasButtonIconLumber", CreateContextCounter)
        call BlzGetFrameByName("TasButtonTextLumber", CreateContextCounter)

        call CreateTasButtonTooltip(frame, parent, CreateContextCounter)

        if i > 1 then 
            if rowRemain == 0 then
                call BlzFrameSetPoint(frame, FRAMEPOINT_TOP, BlzGetFrameByName(buttonName, CreateContextCounter - cols), FRAMEPOINT_BOTTOM, 0, -rowGap)
                set rowRemain = cols
            else
                //call BlzFrameSetPoint(frame, FRAMEPOINT_RIGHT, BlzGetFrameByName(buttonName, CreateContextCounter - 1), FRAMEPOINT_LEFT, -colGap, 0)
                call BlzFrameSetPoint(frame, FRAMEPOINT_LEFT, BlzGetFrameByName(buttonName, CreateContextCounter - 1), FRAMEPOINT_RIGHT, colGap, 0)
            endif
        else
            call BlzFrameSetPoint(frame, FRAMEPOINT_TOPRIGHT, TasButtonListInputFrame[listIndex], FRAMEPOINT_BOTTOMRIGHT, -BlzFrameGetWidth(frame)*cols - colGap*(cols-1), 0)
            //call BlzFrameSetPoint(frame, FRAMEPOINT_TOPRIGHT, TasButtonListInputFrame[listIndex], FRAMEPOINT_BOTTOMRIGHT, 0, 0)
        endif
        set rowRemain = rowRemain - 1


        set i = i + 1
    endloop
    call InitTasButtonListSlider10a(listIndex, cols, rows, colGap, rowGap)
    set frame = null
    return listIndex
endfunction

function CreateTasButtonList8c takes string buttonName, integer cols, integer rows, framehandle parent, code buttonAction, code rightClickAction, code updateAction, code searchAction, code filterAction, code asyncAction, code asyncRigthAction returns integer
    return CreateTasButtonList10(buttonName, cols, rows, parent, buttonAction, rightClickAction, updateAction, searchAction, filterAction, asyncAction, asyncRigthAction, TasButtonListGapCol, TasButtonListGapRow)
endfunction


function CreateTasButtonListEx takes string buttonName, integer cols, integer rows, framehandle parent, code buttonAction, code rightClickAction, code updateAction, code searchAction, code filterAction returns integer
    return CreateTasButtonList8c(buttonName, cols, rows, parent, buttonAction, rightClickAction, updateAction, searchAction, filterAction, null, null)
endfunction

function CreateTasButtonList takes integer buttonCount, framehandle parent, code buttonAction, code updateAction, code searchAction, code filterAction returns integer
    return CreateTasButtonListEx("TasButton", 1, buttonCount, parent, buttonAction, null, updateAction, searchAction, filterAction)
endfunction

function CreateTasButtonListV2 takes integer rowCount, framehandle parent, code buttonAction, code updateAction, code searchAction, code filterAction returns integer
    return CreateTasButtonListEx("TasButtonSmall", 2, rowCount, parent, buttonAction, null, updateAction, searchAction, filterAction)
endfunction

function CreateTasButtonListV3 takes integer rowCount, framehandle parent, code buttonAction, code updateAction, code searchAction, code filterAction returns integer
    return CreateTasButtonListEx("TasButtonGrid", 3, rowCount, parent, buttonAction, null, updateAction, searchAction, filterAction)
endfunction

function TasButtonListAddDataEx takes integer listIndex, integer data, integer playerIndex returns nothing
    local integer listHandle = GetHandleId(LoadLocationHandle(Hash, GetHandleId(TasButtonListDataList[listIndex]), playerIndex))
    local integer index = LoadInteger(Hash, listHandle, 0) + 1
    call SaveInteger(Hash, listHandle, 0, index)
    call SaveInteger(Hash, listHandle, index, data)
    if GetLocalPlayer() == Player(playerIndex) then
        // add to current filtered for local player only
        set listHandle = GetHandleId(TasButtonListDataListFiltered[listIndex])
        call SaveInteger(Hash, listHandle, 0, index)
        call SaveInteger(Hash, listHandle, index, index)
        call BlzFrameSetMinMaxValue(TasButtonListSlider[listIndex], TasButtonListButtonCount[listIndex], index)
    endif
endfunction

function TasButtonListAddData takes integer listIndex, integer data returns nothing
    local integer playerIndex = 0
    loop
        call TasButtonListAddDataEx(listIndex, data, playerIndex)
        set playerIndex = playerIndex + 1
        exitwhen playerIndex == bj_MAX_PLAYER_SLOTS
    endloop
endfunction

function TasButtonListCopyDataEx takes integer writeObject, integer readObject, integer playerIndex returns nothing
    local integer i = LoadInteger(Hash, GetHandleId(TasButtonListDataListFiltered[readObject]), 0)
    local integer listHandleRead = GetHandleId(LoadLocationHandle(Hash, GetHandleId(TasButtonListDataList[readObject]), playerIndex))
    local integer listHandleWrite = GetHandleId(LoadLocationHandle(Hash, GetHandleId(TasButtonListDataList[writeObject]), playerIndex))
    call FlushChildHashtable(Hash, listHandleWrite)
    call RemoveLocation(TasButtonListDataList[writeObject])

    loop
        exitwhen i < 0
        call SaveInteger(Hash, listHandleWrite, i, LoadInteger(Hash, listHandleRead, i))
        set i = i -1
    endloop
    if GetLocalPlayer() == Player(playerIndex) then
        call BlzFrameSetMinMaxValue(TasButtonListSlider[writeObject], TasButtonListButtonCount[writeObject], LoadInteger(Hash, listHandleRead, 0))
        call UpdateTasButtonList(writeObject)
    endif
endfunction

function TasButtonListCopyData takes integer writeObject, integer readObject returns nothing
    local integer playerIndex = 0
    loop
        call TasButtonListCopyDataEx(writeObject, readObject, playerIndex)
        set playerIndex = playerIndex + 1
        exitwhen playerIndex == bj_MAX_PLAYER_SLOTS
    endloop
endfunction

function TasButtonListRemoveDataEx takes integer listIndex, integer data, integer playerIndex returns nothing
    local integer listHandle = GetHandleId(LoadLocationHandle(Hash, GetHandleId(TasButtonListDataList[listIndex]), playerIndex))
    local integer i = LoadInteger(Hash, listHandle, 0)
    local integer max = LoadInteger(Hash, listHandle, 0)
    local integer temp
    loop
        exitwhen i <= 0
        if LoadInteger(Hash, listHandle, 0) == data then
            call SaveInteger(Hash, listHandle, i, LoadInteger(Hash, listHandle, max))
            call SaveInteger(Hash, listHandle, 0, max - 1)
            call RemoveSavedInteger(Hash, listHandle, max)
            exitwhen true
        endif
        set i = i - 1
    endloop
    if GetLocalPlayer() == Player(playerIndex) then
        call BlzFrameSetMinMaxValue(TasButtonListSlider[listIndex], TasButtonListButtonCount[listIndex], LoadInteger(Hash, listHandle, 0))
    endif
endfunction

function TasButtonListRemoveData takes integer listIndex, integer data returns nothing
    local integer playerIndex = 0
    loop
        call TasButtonListRemoveDataEx(listIndex, data, playerIndex)
        set playerIndex = playerIndex + 1
        exitwhen playerIndex == bj_MAX_PLAYER_SLOTS
    endloop
endfunction

function TasButtonListClearDataEx takes integer listIndex, integer playerIndex returns nothing
    call FlushChildHashtable(Hash, GetHandleId(LoadLocationHandle(Hash, GetHandleId(TasButtonListDataList[listIndex]), playerIndex)))
    if GetLocalPlayer() == Player(playerIndex) then
        call FlushChildHashtable(Hash, GetHandleId(TasButtonListDataListFiltered[listIndex]))
        call BlzFrameSetMinMaxValue(TasButtonListSlider[listIndex], 0, 0)
    endif    
endfunction

function TasButtonListClearData takes integer listIndex returns nothing
    local integer playerIndex = 0
    loop
        call TasButtonListClearDataEx(listIndex, playerIndex)
        set playerIndex = playerIndex + 1
        exitwhen playerIndex == bj_MAX_PLAYER_SLOTS
    endloop
endfunction

    private function SyncRightTriggerAction takes nothing returns nothing  
        local integer listIndex = LoadInteger(Hash, GetHandleId(BlzGetTriggerFrame()), 0)
        local integer dataIndex = R2I(BlzGetTriggerFrameValue() + 0.5)
        set TasButtonListData = LoadInteger(Hash, GetHandleId(LoadLocationHandle(Hash, GetHandleId(TasButtonListDataList[listIndex]), GetPlayerId(GetTriggerPlayer()))), dataIndex)
        set TasButtonListIndex = listIndex
        call TriggerExecute(TasButtonListRightAction[listIndex])

        call UpdateTasButtonList(listIndex)
    endfunction
    private function ButtonRightClickTriggerAction takes nothing returns nothing
    local framehandle frame = BlzGetTriggerFrame()
    local integer buttonIndex = LoadInteger(Hash, GetHandleId(frame), 1)
    local integer listIndex = LoadInteger(Hash, GetHandleId(frame), 0)
    local integer dataIndex = LoadInteger(Hash, GetHandleId(TasButtonListDataListFiltered[listIndex]), buttonIndex + TasButtonListViewPoint[listIndex])
    local integer data = LoadInteger(Hash, GetHandleId(LoadLocationHandle(Hash, GetHandleId(TasButtonListDataList[listIndex]), GetPlayerId(GetTriggerPlayer()))), dataIndex)

    set TasButtonListData = data
    set TasButtonListIndex = listIndex
    set TasButtonListFrame = frame

        if IsRightClick(GetTriggerPlayer()) and GetLocalPlayer() == GetTriggerPlayer() then            
            call TriggerEvaluate(TasButtonListAsyncRightAction[listIndex])
            call StartSound(RightClickSound)
            call BlzFrameSetValue(TasButtonListSyncFrameRight[listIndex], dataIndex)
        endif
    endfunction

 private function LoadToc takes nothing returns nothing
    // this function should be Repeated when the game is loaded
    call BlzLoadTOCFile("war3mapimported\\TasButtonList.toc")
    set CreateContextCounter = 0
 endfunction
 private function Init takes nothing returns nothing    
    local integer loopA = 0
    
    call LoadToc()
    static if LIBRARY_FrameLoader then
        call FrameLoaderAdd(function LoadToc)
    endif

    call TriggerAddAction(SyncTrigger, function TasButtonListTriggerActionSync)
    call TriggerAddAction(ButtonTrigger, function TasButtonListTriggerActionButton)
    call TriggerAddAction(SearchTrigger, function TasButtonListTriggerActionSearch)
    call TriggerAddAction(ButtonScrollTrigger, function TasButtonListTriggerActionButtonScroll)
    call TriggerAddAction(SliderTrigger, function TasButtonListTriggerActionSlider)
    call TriggerAddAction(SyncRightTrigger, function SyncRightTriggerAction)
    call TriggerAddAction(ButtonRightTrigger, function ButtonRightClickTriggerAction)

    set RightClickSound = CreateSound("Sound\\Interface\\MouseClick1.wav", false, false, false, 10, 10, "")
    call SetSoundParamsFromLabel(RightClickSound, "InterfaceClick")
    call SetSoundDuration(RightClickSound, 239)
    
endfunction

endlibrary