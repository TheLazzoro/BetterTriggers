{
  "Id": 50331815,
  "Comment": "",
  "IsScript": false,
  "RunOnMapInit": false,
  "Script": "",
  "Events": [],
  "LocalVariables": [],
  "Conditions": [],
  "Actions": [
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 4,
            "TriggerId": 50331661,
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
            "ParamType": 4,
            "TriggerId": 50331658,
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
            "ParamType": 4,
            "TriggerId": 50331650,
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
            "ParamType": 4,
            "TriggerId": 50331687,
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
            "ParamType": 2,
            "value": "CreateRemoveRemove"
          },
          {
            "ParamType": 2,
            "value": "PlayerNP"
          },
          {
            "ParamType": 5,
            "value": "Battlefield"
          }
        ],
        "value": "SetBlightRectBJ"
      }
    },
    {
      "ElementType": 9,
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 3,
            "VariableId": 100663298,
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
            "ParamType": 1,
            "parameters": [
              {
                "ParamType": 1,
                "parameters": [],
                "value": "GetEntireMapRect"
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
                            "parameters": [],
                            "value": "GetFilterUnit"
                          }
                        ],
                        "value": "GetUnitTypeId"
                      },
                      {
                        "ParamType": 2,
                        "value": "OperatorNotEqualENE"
                      },
                      {
                        "ParamType": 5,
                        "value": "h01B"
                      }
                    ],
                    "value": "OperatorCompareUnitCode"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 1,
                            "parameters": [],
                            "value": "GetFilterUnit"
                          }
                        ],
                        "value": "GetUnitTypeId"
                      },
                      {
                        "ParamType": 2,
                        "value": "OperatorNotEqualENE"
                      },
                      {
                        "ParamType": 5,
                        "value": "u000"
                      }
                    ],
                    "value": "OperatorCompareUnitCode"
                  }
                ],
                "value": "GetBooleanAnd"
              }
            ],
            "value": "GetUnitsInRectMatching"
          }
        ],
        "value": "SetVariable"
      }
    },
    {
      "ElementType": 4,
      "Actions": [
        {
          "isEnabled": true,
          "function": {
            "ParamType": 1,
            "parameters": [
              {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [
                      {
                        "ParamType": 1,
                        "parameters": [],
                        "value": "GetEnumUnit"
                      }
                    ],
                    "value": "GetHandleIdBJ"
                  },
                  {
                    "ParamType": 3,
                    "VariableId": 100663605,
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
                  }
                ],
                "value": "LoadTextTagHandleBJ"
              }
            ],
            "value": "DestroyTextTagBJ"
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
                "value": "GetEnumUnit"
              }
            ],
            "value": "RemoveUnit"
          }
        }
      ],
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 3,
            "VariableId": 100663298,
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
          }
        ],
        "value": "ForGroupMultiple"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 5,
            "value": "call DestroyGroup(udg_UnitGroup)"
          }
        ],
        "value": "CustomScriptCode"
      }
    },
    {
      "ElementType": 4,
      "Actions": [
        {
          "isEnabled": true,
          "function": {
            "ParamType": 1,
            "parameters": [
              {
                "ParamType": 1,
                "parameters": [],
                "value": "GetEnumUnit"
              }
            ],
            "value": "RemoveUnit"
          }
        }
      ],
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 3,
            "VariableId": 100663398,
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
          }
        ],
        "value": "ForGroupMultiple"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 3,
            "VariableId": 100663398,
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
          }
        ],
        "value": "GroupClear"
      }
    },
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 2,
            "value": "PauseUnpauseOptionUnpause"
          }
        ],
        "value": "PauseAllUnitsBJ"
      }
    },
    {
      "ElementType": 6,
      "Actions": [
        {
          "isEnabled": true,
          "function": {
            "ParamType": 1,
            "parameters": [
              {
                "ParamType": 3,
                "VariableId": 100663393,
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
            "value": "RemoveDestructable"
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
            "value": "30"
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
            "ParamType": 4,
            "TriggerId": 50331816,
            "value": null
          }
        ],
        "value": "ConditionalTriggerExecute"
      }
    },
    {
      "ElementType": 9,
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 3,
            "VariableId": 100663303,
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
          },
          {
            "ParamType": 5,
            "value": "0"
          }
        ],
        "value": "SetVariable"
      }
    },
    {
      "ElementType": 9,
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 3,
            "VariableId": 100663303,
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
          },
          {
            "ParamType": 5,
            "value": "0"
          }
        ],
        "value": "SetVariable"
      }
    },
    {
      "ElementType": 9,
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 3,
            "VariableId": 100663356,
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
          },
          {
            "ParamType": 5,
            "value": "0.00"
          }
        ],
        "value": "SetVariable"
      }
    },
    {
      "ElementType": 9,
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 3,
            "VariableId": 100663356,
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
          },
          {
            "ParamType": 5,
            "value": "0.00"
          }
        ],
        "value": "SetVariable"
      }
    },
    {
      "ElementType": 9,
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 3,
            "VariableId": 100663345,
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
            "ParamType": 5,
            "value": "0"
          }
        ],
        "value": "SetVariable"
      }
    },
    {
      "ElementType": 9,
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 3,
            "VariableId": 100663344,
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
            "ParamType": 5,
            "value": "0"
          }
        ],
        "value": "SetVariable"
      }
    },
    {
      "ElementType": 9,
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 3,
            "VariableId": 100663343,
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
            "ParamType": 5,
            "value": "0"
          }
        ],
        "value": "SetVariable"
      }
    },
    {
      "ElementType": 12,
      "Actions": [
        {
          "isEnabled": true,
          "function": {
            "ParamType": 1,
            "parameters": [
              {
                "ParamType": 1,
                "parameters": [],
                "value": "GetEnumItem"
              }
            ],
            "value": "RemoveItem"
          }
        }
      ],
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 5,
            "value": "Item Removal 1"
          }
        ],
        "value": "EnumItemsInRectBJMultiple"
      }
    },
    {
      "ElementType": 12,
      "Actions": [
        {
          "isEnabled": true,
          "function": {
            "ParamType": 1,
            "parameters": [
              {
                "ParamType": 1,
                "parameters": [],
                "value": "GetEnumItem"
              }
            ],
            "value": "RemoveItem"
          }
        }
      ],
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 5,
            "value": "Item Removal 2"
          }
        ],
        "value": "EnumItemsInRectBJMultiple"
      }
    },
    {
      "ElementType": 5,
      "Actions": [
        {
          "ElementType": 9,
          "isEnabled": true,
          "function": {
            "ParamType": 1,
            "parameters": [
              {
                "ParamType": 3,
                "VariableId": 100663482,
                "arrayIndexValues": [
                  {
                    "ParamType": 1,
                    "parameters": [
                      {
                        "ParamType": 1,
                        "parameters": [],
                        "value": "GetEnumPlayer"
                      }
                    ],
                    "value": "GetConvertedPlayerId"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  }
                ],
                "value": null
              },
              {
                "ParamType": 5,
                "value": "false"
              }
            ],
            "value": "SetVariable"
          }
        }
      ],
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
        "value": "ForForceMultiple"
      }
    },
    {
      "ElementType": 6,
      "Actions": [
        {
          "ElementType": 5,
          "Actions": [
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 3,
                    "VariableId": 100663357,
                    "arrayIndexValues": [
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 1,
                            "parameters": [],
                            "value": "GetEnumPlayer"
                          }
                        ],
                        "value": "GetConvertedPlayerId"
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
                    "value": "1.00"
                  }
                ],
                "value": "StartTimerBJ"
              }
            },
            {
              "ElementType": 9,
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 3,
                    "VariableId": 100663317,
                    "arrayIndexValues": [
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 1,
                            "parameters": [],
                            "value": "GetEnumPlayer"
                          }
                        ],
                        "value": "GetConvertedPlayerId"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": null
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  }
                ],
                "value": "SetVariable"
              }
            },
            {
              "ElementType": 9,
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 3,
                    "VariableId": 100663701,
                    "arrayIndexValues": [
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 1,
                            "parameters": [],
                            "value": "GetEnumPlayer"
                          }
                        ],
                        "value": "GetConvertedPlayerId"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": null
                  },
                  {
                    "ParamType": 5,
                    "value": "150"
                  }
                ],
                "value": "SetVariable"
              }
            },
            {
              "ElementType": 9,
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 3,
                    "VariableId": 100663358,
                    "arrayIndexValues": [
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 1,
                            "parameters": [],
                            "value": "GetEnumPlayer"
                          }
                        ],
                        "value": "GetConvertedPlayerId"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": null
                  },
                  {
                    "ParamType": 5,
                    "value": "false"
                  }
                ],
                "value": "SetVariable"
              }
            },
            {
              "ElementType": 9,
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 3,
                    "VariableId": 100663359,
                    "arrayIndexValues": [
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 1,
                            "parameters": [],
                            "value": "GetEnumPlayer"
                          }
                        ],
                        "value": "GetConvertedPlayerId"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": null
                  },
                  {
                    "ParamType": 5,
                    "value": "100"
                  }
                ],
                "value": "SetVariable"
              }
            },
            {
              "ElementType": 9,
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 3,
                    "VariableId": 100663360,
                    "arrayIndexValues": [
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 1,
                            "parameters": [],
                            "value": "GetEnumPlayer"
                          }
                        ],
                        "value": "GetConvertedPlayerId"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": null
                  },
                  {
                    "ParamType": 5,
                    "value": "100"
                  }
                ],
                "value": "SetVariable"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 2,
                    "value": "ShowHideHide"
                  },
                  {
                    "ParamType": 3,
                    "VariableId": 100663307,
                    "arrayIndexValues": [
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 1,
                            "parameters": [],
                            "value": "GetEnumPlayer"
                          }
                        ],
                        "value": "GetConvertedPlayerId"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": null
                  }
                ],
                "value": "MultiboardDisplayBJ"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 3,
                    "VariableId": 100663307,
                    "arrayIndexValues": [
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 1,
                            "parameters": [],
                            "value": "GetEnumPlayer"
                          }
                        ],
                        "value": "GetConvertedPlayerId"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": null
                  },
                  {
                    "ParamType": 5,
                    "value": "2"
                  },
                  {
                    "ParamType": 5,
                    "value": "1"
                  },
                  {
                    "ParamType": 5,
                    "value": "None"
                  }
                ],
                "value": "MultiboardSetItemValueBJ"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 3,
                    "VariableId": 100663307,
                    "arrayIndexValues": [
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 1,
                            "parameters": [],
                            "value": "GetEnumPlayer"
                          }
                        ],
                        "value": "GetConvertedPlayerId"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": null
                  },
                  {
                    "ParamType": 5,
                    "value": "2"
                  },
                  {
                    "ParamType": 5,
                    "value": "5"
                  },
                  {
                    "ParamType": 3,
                    "VariableId": 100663359,
                    "arrayIndexValues": [
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 1,
                            "parameters": [],
                            "value": "GetEnumPlayer"
                          }
                        ],
                        "value": "GetConvertedPlayerId"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": null
                  }
                ],
                "value": "MultiboardSetItemValueBJ"
              }
            },
            {
              "ElementType": 9,
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 3,
                    "VariableId": 100663681,
                    "arrayIndexValues": [
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 1,
                            "parameters": [],
                            "value": "GetEnumPlayer"
                          }
                        ],
                        "value": "GetConvertedPlayerId"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": null
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  }
                ],
                "value": "SetVariable"
              }
            },
            {
              "ElementType": 9,
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 3,
                    "VariableId": 100663481,
                    "arrayIndexValues": [
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 1,
                            "parameters": [],
                            "value": "GetEnumPlayer"
                          }
                        ],
                        "value": "GetConvertedPlayerId"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": null
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  }
                ],
                "value": "SetVariable"
              }
            },
            {
              "ElementType": 9,
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 3,
                    "VariableId": 100663352,
                    "arrayIndexValues": [
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 1,
                            "parameters": [],
                            "value": "GetEnumPlayer"
                          }
                        ],
                        "value": "GetConvertedPlayerId"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": null
                  },
                  {
                    "ParamType": 5,
                    "value": "0.00"
                  }
                ],
                "value": "SetVariable"
              }
            },
            {
              "ElementType": 9,
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 3,
                    "VariableId": 100663484,
                    "arrayIndexValues": [
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 1,
                            "parameters": [],
                            "value": "GetEnumPlayer"
                          }
                        ],
                        "value": "GetConvertedPlayerId"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": null
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  }
                ],
                "value": "SetVariable"
              }
            },
            {
              "ElementType": 9,
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 3,
                    "VariableId": 100663483,
                    "arrayIndexValues": [
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 1,
                            "parameters": [],
                            "value": "GetEnumPlayer"
                          }
                        ],
                        "value": "GetConvertedPlayerId"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": null
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  }
                ],
                "value": "SetVariable"
              }
            },
            {
              "ElementType": 9,
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 3,
                    "VariableId": 100663349,
                    "arrayIndexValues": [
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 1,
                            "parameters": [],
                            "value": "GetEnumPlayer"
                          }
                        ],
                        "value": "GetConvertedPlayerId"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": null
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  }
                ],
                "value": "SetVariable"
              }
            },
            {
              "ElementType": 9,
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 3,
                    "VariableId": 100663602,
                    "arrayIndexValues": [
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 1,
                            "parameters": [],
                            "value": "GetEnumPlayer"
                          }
                        ],
                        "value": "GetConvertedPlayerId"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": null
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  }
                ],
                "value": "SetVariable"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "HUMAN"
                  }
                ],
                "value": "CommentString"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rhme"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rhra"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rhhb"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rhar"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rhgb"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rhde"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rhan"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rhpt"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rhst"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rhla"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rhri"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rhfl"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rhss"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rhrt"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rhfc"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rhfs"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rhfs"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rhsb"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "R00J"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "H01F"
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
            },
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
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "H01J"
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
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "H01L"
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
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "ORC"
                  }
                ],
                "value": "CommentString"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rome"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rora"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Roar"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rwdm"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Robs"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rows"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Roen"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rovs"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rowd"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rost"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rotr"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rowt"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Robk"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Robf"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "R00K"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "O002"
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
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "O003"
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
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "O004"
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
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "O005"
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
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "UNDEAD"
                  }
                ],
                "value": "CommentString"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rume"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rura"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Ruar"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rugf"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Ruwb"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rune"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Ruba"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rufb"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rusl"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rucr"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rupc"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rusm"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rubu"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Ruex"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "R00L"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rusf"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "U005"
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
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "U006"
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
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "U007"
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
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "U008"
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
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "NIGHT ELF"
                  }
                ],
                "value": "CommentString"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Resm"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Resw"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rema"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rerh"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Resc"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Remg"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Reib"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Remk"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Redt"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Redc"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Resi"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Recb"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Repb"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rers"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Rehs"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "Reeb"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "R00M"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  },
                  {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "GetEnumPlayer"
                  }
                ],
                "value": "SetPlayerTechResearchedSwap"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "E00H"
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
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "E00I"
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
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "E00J"
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
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "E00K"
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
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "HEROES"
                  }
                ],
                "value": "CommentString"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "N012"
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
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "N013"
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
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "N014"
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
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "N015"
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
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "N016"
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
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "N017"
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
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "N018"
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
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "N019"
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
          ],
          "isEnabled": true,
          "function": {
            "ParamType": 1,
            "parameters": [
              {
                "ParamType": 3,
                "VariableId": 100663302,
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
            "value": "ForForceMultiple"
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
            "value": "2"
          }
        ],
        "value": "ForLoopAMultiple"
      }
    }
  ]
}