{
  "Id": 50331814,
  "Comment": "",
  "IsScript": false,
  "RunOnMapInit": false,
  "Script": "",
  "Events": [
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 5,
            "value": "2.00"
          }
        ],
        "value": "TriggerRegisterTimerEventSingle"
      }
    }
  ],
  "LocalVariables": [],
  "Conditions": [],
  "Actions": [
    {
      "isEnabled": false,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 5,
            "value": "SetupTestUI()"
          }
        ],
        "value": "CustomScriptCode"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 2,
            "value": "Player00"
          },
          {
            "ParamType": 2,
            "value": "PlayerStateGold"
          },
          {
            "ParamType": 5,
            "value": "250"
          }
        ],
        "value": "SetPlayerState"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 2,
            "value": "Player01"
          },
          {
            "ParamType": 2,
            "value": "PlayerStateGold"
          },
          {
            "ParamType": 5,
            "value": "250"
          }
        ],
        "value": "SetPlayerState"
      }
    },
    {
      "isEnabled": false,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 4,
            "TriggerId": 50331651,
            "value": null
          }
        ],
        "value": "EnableTrigger"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 3,
            "VariableId": 100663308,
            "arrayIndexValues": [
              {
                "ParamType": 5,
                "value": "0"
              },
              {
                "ParamType": 5,
                "value": "0"
              }
            ],
            "value": null
          },
          {
            "ParamType": 2,
            "value": "PeriodicOptionOneTime"
          },
          {
            "ParamType": 5,
            "value": "25.00"
          }
        ],
        "value": "StartTimerBJ"
      }
    }
  ]
}