{
  "Id": 50331802,
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
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 5,
            "value": "war3mapImported\\Intro3.mp3"
          }
        ],
        "value": "PlayThematicMusicBJ"
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
          }
        ],
        "value": "ClearTextMessagesBJ"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 5,
            "value": "call SetCameraField(CAMERA_FIELD_FARZ, 7000, 0)"
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
            "value": "CameraApply"
          },
          {
            "ParamType": 5,
            "value": "Cinematic 02 Team 1"
          },
          {
            "ParamType": 2,
            "value": "Player00"
          },
          {
            "ParamType": 5,
            "value": "9.50"
          },
          {
            "ParamType": 5,
            "value": "6.00"
          },
          {
            "ParamType": 5,
            "value": "1.00"
          },
          {
            "ParamType": 5,
            "value": "1.00"
          }
        ],
        "value": "CameraSetupApplyForPlayerSmooth"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 2,
            "value": "CameraApply"
          },
          {
            "ParamType": 5,
            "value": "Cinematic 02 Team 1"
          },
          {
            "ParamType": 2,
            "value": "Player02"
          },
          {
            "ParamType": 5,
            "value": "9.50"
          },
          {
            "ParamType": 5,
            "value": "6.00"
          },
          {
            "ParamType": 5,
            "value": "1.00"
          },
          {
            "ParamType": 5,
            "value": "1.00"
          }
        ],
        "value": "CameraSetupApplyForPlayerSmooth"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 2,
            "value": "CameraApply"
          },
          {
            "ParamType": 5,
            "value": "Cinematic 02 Team 1"
          },
          {
            "ParamType": 2,
            "value": "Player04"
          },
          {
            "ParamType": 5,
            "value": "9.50"
          },
          {
            "ParamType": 5,
            "value": "6.00"
          },
          {
            "ParamType": 5,
            "value": "1.00"
          },
          {
            "ParamType": 5,
            "value": "1.00"
          }
        ],
        "value": "CameraSetupApplyForPlayerSmooth"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 2,
            "value": "CameraApply"
          },
          {
            "ParamType": 5,
            "value": "Cinematic 02 Team 2"
          },
          {
            "ParamType": 2,
            "value": "Player01"
          },
          {
            "ParamType": 5,
            "value": "9.50"
          },
          {
            "ParamType": 5,
            "value": "6.00"
          },
          {
            "ParamType": 5,
            "value": "1.00"
          },
          {
            "ParamType": 5,
            "value": "1.00"
          }
        ],
        "value": "CameraSetupApplyForPlayerSmooth"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 2,
            "value": "CameraApply"
          },
          {
            "ParamType": 5,
            "value": "Cinematic 02 Team 2"
          },
          {
            "ParamType": 2,
            "value": "Player03"
          },
          {
            "ParamType": 5,
            "value": "9.50"
          },
          {
            "ParamType": 5,
            "value": "6.00"
          },
          {
            "ParamType": 5,
            "value": "1.00"
          },
          {
            "ParamType": 5,
            "value": "1.00"
          }
        ],
        "value": "CameraSetupApplyForPlayerSmooth"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 2,
            "value": "CameraApply"
          },
          {
            "ParamType": 5,
            "value": "Cinematic 02 Team 2"
          },
          {
            "ParamType": 2,
            "value": "Player05"
          },
          {
            "ParamType": 5,
            "value": "9.50"
          },
          {
            "ParamType": 5,
            "value": "6.00"
          },
          {
            "ParamType": 5,
            "value": "1.00"
          },
          {
            "ParamType": 5,
            "value": "1.00"
          }
        ],
        "value": "CameraSetupApplyForPlayerSmooth"
      }
    }
  ]
}