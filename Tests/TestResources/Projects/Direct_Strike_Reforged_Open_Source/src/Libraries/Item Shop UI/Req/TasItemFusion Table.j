library TasItemFusion initializer Init requires TasItemCost, Table
// function TasItemFusionAdd takes integer result, integer mat returns nothing
//  function TasItemFusionAdd2 takes integer result, integer a, integer b returns nothing
//  function TasItemFusionAdd3 takes integer result, integer a, integer b, integer c returns nothing
//  function TasItemFusionAdd4 takes integer result, integer a, integer b, integer c, integer d returns nothing
//  function TasItemFusionAdd5 takes integer result, integer a, integer b, integer c, integer d, integer e returns nothing
//  function TasItemFusionAdd6 takes integer result, integer a, integer b, integer c, integer d, integer e, integer f returns nothing
// function TasItemFusionGetUseableItems takes player p , group units, boolean checkOwner returns nothing
// function TasItemFusionGetUseableMaterial takes player p, integer result, boolean reset returns nothing
// function TasItemFusionCalc player p, integer result returns nothing
// function TasItemFusionGetMissingMaterial takes player p, integer result, boolean reset returns nothing
globals
        public integer Count = 0
        public hashtable Hash = InitHashtable()

        public integer array ItemCodes
        public integer ItemCodesCount = 0

        public integer FusionCount = 0
        public integer array FusionResult
        public HashTable UsedIn  // allows to find Fusions from a Mat
        public HashTable BuiltWay  // find Fusions from the result
        public Table array PlayerItems  //contains all useable
        public Table array PlayerMaterial  //contains material used
        public Table array PlayerMissing  //contains material used
        public Table array PlayerMissingUsed  //contains material used
      
        private boolean CheckOwner
        private player Owner
        private integer OwnerIndex

        // Costs
        integer TasItemFusionGold
        integer TasItemFusionLumber
    endglobals


    function TasItemFusionAdd takes integer result, integer mat returns nothing
        local integer materialCount
        local integer builtWayCount
        local integer count
        
        set FusionCount = FusionCount + 1
        set FusionResult[FusionCount] = result

        set count = BuiltWay[result][0] + 1
        set BuiltWay[result][0] = count
        set BuiltWay[result][count] = mat
        
        if not UsedIn[mat].boolean.has(result) then
            set UsedIn[mat].boolean[result] = true
            set count = UsedIn[mat][0] + 1
            set UsedIn[mat][0] = count
            set UsedIn[mat][count] = result
            call TasItemCaclCost(mat)
        endif        
        
        call TasItemCaclCost(result)        
    endfunction

    function TasItemFusionAdd2 takes integer result, integer a, integer b returns nothing
        call TasItemFusionAdd(result, a)
        call TasItemFusionAdd(result, b)
    endfunction

    function TasItemFusionAdd3 takes integer result, integer a, integer b, integer c returns nothing
        call TasItemFusionAdd(result, a)
        call TasItemFusionAdd(result, b)
        call TasItemFusionAdd(result, c)
    endfunction

    function TasItemFusionAdd4 takes integer result, integer a, integer b, integer c, integer d returns nothing
        call TasItemFusionAdd(result, a)
        call TasItemFusionAdd(result, b)
        call TasItemFusionAdd(result, c)
        call TasItemFusionAdd(result, d)
    endfunction

    function TasItemFusionAdd5 takes integer result, integer a, integer b, integer c, integer d, integer e returns nothing
        call TasItemFusionAdd(result, a)
        call TasItemFusionAdd(result, b)
        call TasItemFusionAdd(result, c)
        call TasItemFusionAdd(result, d)
        call TasItemFusionAdd(result, e)
    endfunction

    function TasItemFusionAdd6 takes integer result, integer a, integer b, integer c, integer d, integer e, integer f returns nothing
        call TasItemFusionAdd(result, a)
        call TasItemFusionAdd(result, b)
        call TasItemFusionAdd(result, c)
        call TasItemFusionAdd(result, d)
        call TasItemFusionAdd(result, e)
        call TasItemFusionAdd(result, f)
    endfunction
 

    function TasItemFusionGetUseableItemsEnum takes nothing returns nothing
        local integer index = 0
        local item i
        local unit u = GetEnumUnit()
        local integer count
        local integer itemCode
        loop
            set i = UnitItemInSlot(u, index)
            if i != null and (not CheckOwner or (GetItemPlayer(i) == Owner or GetItemPlayer(i) == Player(PLAYER_NEUTRAL_PASSIVE))) then
                set count = PlayerItems[OwnerIndex][0] + 1
                set PlayerItems[OwnerIndex][0] = count
                set PlayerItems[OwnerIndex].item[count] = i
                set itemCode = GetItemTypeId(i)
                set PlayerItems[OwnerIndex][itemCode] = PlayerItems[OwnerIndex][itemCode] + 1
            endif
            set index = index + 1
            exitwhen index >= bj_MAX_INVENTORY
        endloop

        set u = null
        set i = null
    endfunction
    function TasItemFusionGetUseableItems takes player p , group units, boolean checkOwner returns nothing
        // give the units which inventory is useable
        local integer playerIndex = GetPlayerId(p)       
        call PlayerItems[playerIndex].flush()
        
        set OwnerIndex = playerIndex
        set Owner = p
        set CheckOwner = checkOwner
        call ForGroup(units, function TasItemFusionGetUseableItemsEnum)
    endfunction

    // returns a list of material that can be used for result.
    function TasItemFusionGetUseableMaterial takes player p, integer result, boolean reset, boolean quick returns nothing
        local integer playerIndex = GetPlayerId(p)
        local item i
        local boolean canBeFound
        local integer loopA
        local integer loopB
        local integer itemCode
        local integer count
        if reset then
            call PlayerMaterial[playerIndex].flush()
            //set UpdateCounter = UpdateCounter + 1
            //call BlzFrameSetText(UpdateCounterText, I2S(UpdateCounter))
        endif

        set loopA = BuiltWay[result][0]
        loop
            exitwhen loopA < 1
            set itemCode = BuiltWay[result][loopA]
            // have more total then yet found
            set canBeFound = (PlayerItems[playerIndex].integer[itemCode] > PlayerMaterial[playerIndex].integer[itemCode])
            
            if canBeFound then
                if quick then
                    set count = PlayerMaterial[playerIndex][0] + 1
                    set PlayerMaterial[playerIndex][0] = count
                    set PlayerMaterial[playerIndex][count] = itemCode
                    set PlayerMaterial[playerIndex][itemCode] = PlayerMaterial[playerIndex][itemCode] + 1
                else
                    set loopB = PlayerItems[playerIndex][0]
                    loop
                        exitwhen loopB < 1
                        set i = PlayerItems[playerIndex].item[loopB]
                        if GetItemTypeId(i) == itemCode and not PlayerMaterial[playerIndex].boolean[GetHandleId(i)] then
                            set PlayerMaterial[playerIndex].boolean[GetHandleId(i)] = true
                            set PlayerMaterial[playerIndex][itemCode] = PlayerMaterial[playerIndex][itemCode] + 1
                            set count = PlayerMaterial[playerIndex][0] + 1
                            set PlayerMaterial[playerIndex][0] = count
                            set PlayerMaterial[playerIndex].item[count] = i
                            exitwhen true
                        endif
                        set loopB = loopB - 1
                    endloop
                endif
            elseif BuiltWay[itemCode][0] > 0 then
                call TasItemFusionGetUseableMaterial(p, itemCode, false, quick)
            endif

            set loopA = loopA - 1
        endloop
        set i = null
    endfunction


// returns the total gold cost and the used material from useAble
    function TasItemFusionCalc takes player p, integer result, boolean quick returns nothing
        // find all useable fusion material
        local integer playerIndex = GetPlayerId(p)
        local integer gold = TasItemGetCostGold(result)
        local integer lumber = TasItemGetCostLumber(result)
        local integer gold2
        local integer lumber2
        local integer count
        call TasItemFusionGetUseableMaterial(p, result, true, quick)
        // reduce total gold cost by the useables
        set count = PlayerMaterial[playerIndex][0]
        loop
            exitwhen count < 1
            if quick then
                set gold2 = TasItemGetCostGold(PlayerMaterial[playerIndex][count])
                set lumber2 = TasItemGetCostLumber(PlayerMaterial[playerIndex][count])
            else
                set gold2 = TasItemGetCostGold(GetItemTypeId(PlayerMaterial[playerIndex].item[count]))
                set lumber2 = TasItemGetCostLumber(GetItemTypeId(PlayerMaterial[playerIndex].item[count]))
            endif
            set gold = gold - gold2
            set lumber = lumber - lumber2
            set count = count - 1
        endloop
        // "return values"
        set TasItemFusionGold = gold
        set TasItemFusionLumber = lumber
    endfunction


    // returns a table of the material missing
    // call it that way TasItemFusionGetMissingMaterial(useAble, result)
    function TasItemFusionGetMissingMaterial takes player p, integer result, boolean reset returns nothing
        local item i
        local integer playerIndex = GetPlayerId(p)
        local boolean found
        local integer loopA
        local integer loopB
        local integer itemCode
        local integer count
        if reset then
            call PlayerMissing[playerIndex].flush()
            call PlayerMissingUsed[playerIndex].flush()
        endif
        set loopA = BuiltWay[result][0]
        loop
            exitwhen loopA < 1
            set found = false
            set itemCode = BuiltWay[result][loopA]
            set loopB = PlayerItems[playerIndex][0]
            
            loop
                exitwhen loopB < 1
                set i = PlayerItems[playerIndex].item[loopB]
                if GetItemTypeId(i) == itemCode and not PlayerMissingUsed[playerIndex].boolean[GetHandleId(i)] then
                    set PlayerMissingUsed[playerIndex].boolean[GetHandleId(i)] = true
                    set count = PlayerMissingUsed[playerIndex][0] + 1
                    set PlayerMissingUsed[playerIndex][0] = count
                    set PlayerMissingUsed[playerIndex].item[count] = i
                    set found = true
                    exitwhen true
                endif
                set loopB = loopB - 1
            endloop
            if not found and not PlayerMissing[playerIndex].boolean[itemCode] then
                set count = PlayerMissing[playerIndex][0] + 1
                set PlayerMissing[playerIndex][0] = count
                set PlayerMissing[playerIndex][count] = itemCode
                set PlayerMissing[playerIndex].boolean[itemCode] = true
                if BuiltWay[itemCode][0] > 0 then
                    call TasItemFusionGetMissingMaterial(p, itemCode, false)
                endif
            endif

            set loopA = loopA - 1
        endloop
        
    endfunction

    private function Init takes nothing returns nothing
        local integer loopA = 0
        set UsedIn = HashTable.create() // allows to find Fusions from a Mat
        set BuiltWay = HashTable.create() // find Fusions from the result
        loop
            set PlayerItems[loopA] = Table.create()
            set PlayerMaterial[loopA] = Table.create()  //contains material used
            set PlayerMissing[loopA] = Table.create()
            set PlayerMissingUsed[loopA] = Table.create()
            set loopA = loopA + 1
            exitwhen loopA == bj_MAX_PLAYER_SLOTS
            // body
        endloop
        
        
    endfunction

endlibrary