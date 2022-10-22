function BlzQueueImmediateOrderByIdBT takes unit whichUnit, string order returns nothing
    call BlzQueueImmediateOrderById(whichUnit, OrderId(order))
endfunction

function BlzQueuePointOrderByIdBT takes string order, unit whichUnit, location loc returns nothing
    call BlzQueuePointOrderById(whichUnit, OrderId(order), GetLocationX(loc), GetLocationY(loc))
endfunction

function BlzQueueTargetOrderByIdBT takes unit whichUnit, string order, widget targetWidget returns nothing
    call BlzQueueTargetOrderById(whichUnit, OrderId(order), targetWidget)
endfunction