{
  "Id": 50331705,
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
            "value": "-test"
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
      "ElementType": 6,
      "Actions": [
        {
          "isEnabled": true,
          "function": {
            "ParamType": 1,
            "parameters": [
              {
                "ParamType": 1,
                "parameters": [],
                "value": "GetPlayersAll"
              },
              {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Player "
                  },
                  {
                    "ParamType": 1,
                    "parameters": [
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 1,
                            "parameters": [
                              {
                                "ParamType": 1,
                                "parameters": [
                                  {
                                    "ParamType": 1,
                                    "parameters": [],
                                    "value": "GetForLoopIndexA"
                                  }
                                ],
                                "value": "ConvertedPlayer"
                              }
                            ],
                            "value": "GetConvertedPlayerId"
                          }
                        ],
                        "value": "I2S"
                      },
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 5,
                            "value": ": "
                          },
                          {
                            "ParamType": 1,
                            "parameters": [
                              {
                                "ParamType": 3,
                                "VariableId": 100663334,
                                "arrayIndexValues": [
                                  {
                                    "ParamType": 1,
                                    "parameters": [],
                                    "value": "GetForLoopIndexA"
                                  },
                                  {
                                    "ParamType": 5,
                                    "value": "0"
                                  }
                                ],
                                "value": null
                              }
                            ],
                            "value": "I2S"
                          }
                        ],
                        "value": "OperatorString"
                      }
                    ],
                    "value": "OperatorString"
                  }
                ],
                "value": "OperatorString"
              }
            ],
            "value": "DisplayTextToForce"
          }
        }
      ],
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 5,
            "value": "1"
          },
          {
            "ParamType": 5,
            "value": "6"
          }
        ],
        "value": "ForLoopAMultiple"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 1,
            "parameters": [],
            "value": "GetPlayersAll"
          },
          {
            "ParamType": 1,
            "parameters": [
              {
                "ParamType": 5,
                "value": "Spawn Team 1: "
              },
              {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 3,
                    "VariableId": 100663309,
                    "arrayIndexValues": [
                      {
                        "ParamType": 5,
                        "value": "1"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": null
                  }
                ],
                "value": "I2S"
              }
            ],
            "value": "OperatorString"
          }
        ],
        "value": "DisplayTextToForce"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 1,
            "parameters": [],
            "value": "GetPlayersAll"
          },
          {
            "ParamType": 1,
            "parameters": [
              {
                "ParamType": 5,
                "value": "Spawn Team 2: "
              },
              {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 3,
                    "VariableId": 100663309,
                    "arrayIndexValues": [
                      {
                        "ParamType": 5,
                        "value": "2"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": null
                  }
                ],
                "value": "I2S"
              }
            ],
            "value": "OperatorString"
          }
        ],
        "value": "DisplayTextToForce"
      }
    }
  ]
}