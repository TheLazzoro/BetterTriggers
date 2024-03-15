{
  "Id": 50332034,
  "Comment": "",
  "IsScript": false,
  "RunOnMapInit": false,
  "Script": "",
  "Events": [
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [],
        "value": "MapInitializationEvent"
      }
    }
  ],
  "LocalVariables": [],
  "Conditions": [],
  "Actions": [
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 5,
            "value": "H01I"
          },
          {
            "ParamType": 2,
            "value": "AvailabilityOptionAvailable"
          },
          {
            "ParamType": 1,
            "parameters": [],
            "value": "GetEnumPlayer"
          }
        ],
        "value": "SetPlayerUnitAvailableBJ"
      }
    }
  ]
}