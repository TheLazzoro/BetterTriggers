library SimpleTooltip
    function CreateSimpleTooltip takes framehandle frame, string text returns framehandle
        // this FRAME is important when the Box is outside of 4:3 it can be limited to 4:3.
        local framehandle toolTipParent = BlzCreateFrameByType("FRAME", "", frame, "", 0)
        local framehandle toolTipBox = BlzCreateFrame("EscMenuControlBackdropTemplate", toolTipParent, 0, 0)
        local framehandle toolTip = BlzCreateFrame("TasButtonTextTemplate", toolTipBox, 0, 0)
        call BlzFrameSetPoint(toolTip, FRAMEPOINT_BOTTOM, frame, FRAMEPOINT_TOP, 0, 0.008)
        call BlzFrameSetPoint(toolTipBox, FRAMEPOINT_TOPLEFT, toolTip, FRAMEPOINT_TOPLEFT, -0.008, 0.008)
        call BlzFrameSetPoint(toolTipBox, FRAMEPOINT_BOTTOMRIGHT, toolTip, FRAMEPOINT_BOTTOMRIGHT, 0.008, -0.008)
        call BlzFrameSetText(toolTip, text)
        call BlzFrameSetTooltip(frame, toolTipParent)
        return toolTip
    endfunction
endlibrary
 