{
  "Id": 50331727,
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
            "ParamType": 2,
            "value": "Player00"
          },
          {
            "ParamType": 5,
            "value": "-reset"
          },
          {
            "ParamType": 2,
            "value": "ChatMatchTypeExact"
          }
        ],
        "value": "TriggerRegisterPlayerChatEvent"
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
            "ParamType": 4,
            "TriggerId": 50331815,
            "value": null
          }
        ],
        "value": "ConditionalTriggerExecute"
      }
    }
  ]
}