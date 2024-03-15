library TasItemCost initializer Init

    //function TasItemCaclCost takes integer itemCode returns nothing
    //function TasItemGetCostGold takes integer itemCode returns integer
    //function TasItemGetCostLumber takes integer itemCode returns integer
        globals
            private unit shop
            // defines position of a hidden shop to get item costs
            // the size is used to clear up bought items
            private real shopRectSize = 1000
            private real shopRectX = 180
            private real shopRectY = -340
            private rect shopRect
            private player shopOwner = Player(bj_PLAYER_NEUTRAL_EXTRA)
            private integer array Test
            private integer TestCount = 0
            private integer HASH_GOLD = StringHash("GOLD")
            private integer HASH_LUMBER = StringHash("LUMBER")
            private integer HASH_CHARGE = StringHash("CHARGE")
            hashtable TasItemHash = InitHashtable()
        endglobals
        private function ClearItem takes nothing returns nothing
            //call BJDebugMsg("Enum " + GetItemName(GetEnumItem()))
            call RemoveItem(GetEnumItem())
        endfunction
        
        private function ClearItemStart takes nothing returns nothing
            //call BJDebugMsg("ClearItemStart")
            call EnumItemsInRect(shopRect, null, function ClearItem)
        endfunction
        private function TasItemCalcDestroy takes nothing returns nothing
            call ClearItemStart()
            call ShowUnit(shop, true)
            call RemoveUnit(shop)
            call RemoveRect(shopRect)
        endfunction
        private function Start takes nothing returns nothing
            local integer itemCode = Test[1]
            local integer gold
            local integer lumber
            local item i
            call AddItemToStock(shop, itemCode, 1, 1)
            call SetPlayerState(shopOwner, PLAYER_STATE_RESOURCE_GOLD, 99999999)
            call SetPlayerState(shopOwner, PLAYER_STATE_RESOURCE_LUMBER, 99999999)
            set gold = GetPlayerState(shopOwner, PLAYER_STATE_RESOURCE_GOLD)
            set lumber = GetPlayerState(shopOwner, PLAYER_STATE_RESOURCE_LUMBER)
            call IssueNeutralImmediateOrderById(shopOwner, shop, itemCode)
            
            call SaveInteger(TasItemHash, itemCode, HASH_GOLD, gold - GetPlayerState(shopOwner, PLAYER_STATE_RESOURCE_GOLD))
            call SaveInteger(TasItemHash, itemCode, HASH_LUMBER, lumber - GetPlayerState(shopOwner, PLAYER_STATE_RESOURCE_LUMBER))
            set i = CreateItem(itemCode,0,0)
            call SaveInteger(TasItemHash, itemCode, HASH_CHARGE, GetItemCharges(i))
            call RemoveItem(i)
            set i = null
            call RemoveItemFromStock(shop, itemCode)
            // testing order does not matter much, simple reindex
            set Test[1] = Test[TestCount]
            set TestCount = TestCount - 1
            
            call ClearItemStart()
            if TestCount > 0 then
                call Start()
            else
                //call TimerStart(t, 1, false, function ClearItemStart)	
            endif
            
        endfunction
    
        function TasItemCaclCost takes integer itemCode returns nothing
            local item i
            // if there is already data for that itemcode, skip it
            if not HaveSavedInteger(TasItemHash, itemCode, HASH_GOLD) then
                    // is this a valid itemCode? Create it, if that fails skip testing it
                set i = CreateItem(itemCode, 0, 0)
                if GetHandleId(i) > 0 then
                    call RemoveItem(i)
                    set TestCount = TestCount + 1
                    set Test[TestCount] = itemCode
                endif
                if TestCount > 0 then
                    call Start() 
                endif
                set i = null
            endif
        endfunction
            
        function TasItemGetCostGold takes integer itemCode returns integer
            call TasItemCaclCost(itemCode)
            return LoadInteger(TasItemHash, itemCode, HASH_GOLD)
        endfunction
        function TasItemGetCostLumber takes integer itemCode returns integer
            call TasItemCaclCost(itemCode)
            return LoadInteger(TasItemHash, itemCode, HASH_LUMBER)
        endfunction
        function TasItemGetCharges takes integer itemCode returns integer
            call TasItemCaclCost(itemCode)
            return LoadInteger(TasItemHash, itemCode, HASH_CHARGE)
        endfunction
        private function Init takes nothing returns nothing
            set shopRect = Rect(0, 0, shopRectSize, shopRectSize)
            set shop = CreateUnit(shopOwner, 'nmrk', shopRectX, shopRectY, 0)
            call SetUnitX(shop, shopRectX)
            call SetUnitY(shop, shopRectY)
            call MoveRectTo(shopRect, shopRectX, shopRectY)
            call UnitAddAbility(shop, 'AInv')
            call IssueNeutralTargetOrder(shopOwner, shop, "smart", shop)
            call ShowUnit(shop, false)
        endfunction
    endlibrary
    