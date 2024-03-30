{
  "Id": 50331819,
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
            "value": "0.70"
          }
        ],
        "value": "TriggerRegisterTimerEventPeriodic"
      }
    }
  ],
  "LocalVariables": [],
  "Conditions": [
    {
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 1,
            "parameters": [
              {
                "ParamType": 3,
                "VariableId": 100663390,
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
            "value": "CountUnitsInGroup"
          },
          {
            "ParamType": 2,
            "value": "OperatorGreater"
          },
          {
            "ParamType": 5,
            "value": "0"
          }
        ],
        "value": "OperatorCompareInteger"
      }
    }
  ],
  "Actions": [
    {
      "ElementType": 9,
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [
          {
            "ParamType": 3,
            "VariableId": 100663325,
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
                "ParamType": 3,
                "VariableId": 100663390,
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
            "value": "GroupPickRandomUnit"
          }
        ],
        "value": "SetVariable"
      }
    },
    {
      "ElementType": 1,
      "If": [
        {
          "isEnabled": true,
          "function": {
            "ParamType": 1,
            "parameters": [
              {
                "ParamType": 3,
                "VariableId": 100663463,
                "arrayIndexValues": [
                  {
                    "ParamType": 1,
                    "parameters": [
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 3,
                            "VariableId": 100663325,
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
                        "value": "GetOwningPlayer"
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
                "value": "OperatorEqualENE"
              },
              {
                "ParamType": 5,
                "value": "true"
              }
            ],
            "value": "OperatorCompareBoolean"
          }
        }
      ],
      "Then": [
        {
          "ElementType": 9,
          "isEnabled": true,
          "function": {
            "ParamType": 1,
            "parameters": [
              {
                "ParamType": 3,
                "VariableId": 100663297,
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
                    "ParamType": 3,
                    "VariableId": 100663325,
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
                "value": "GetUnitLoc"
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
                    "ParamType": 5,
                    "value": "500.00"
                  },
                  {
                    "ParamType": 3,
                    "VariableId": 100663297,
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
                        "parameters": [
                          {
                            "ParamType": 1,
                            "parameters": [
                              {
                                "ParamType": 1,
                                "parameters": [],
                                "value": "GetFilterUnit"
                              },
                              {
                                "ParamType": 1,
                                "parameters": [
                                  {
                                    "ParamType": 3,
                                    "VariableId": 100663325,
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
                                "value": "GetOwningPlayer"
                              }
                            ],
                            "value": "IsUnitEnemy"
                          },
                          {
                            "ParamType": 2,
                            "value": "OperatorEqualENE"
                          },
                          {
                            "ParamType": 5,
                            "value": "false"
                          }
                        ],
                        "value": "OperatorCompareBoolean"
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
                                  },
                                  {
                                    "ParamType": 5,
                                    "value": "Bply"
                                  }
                                ],
                                "value": "UnitHasBuffBJ"
                              },
                              {
                                "ParamType": 2,
                                "value": "OperatorEqualENE"
                              },
                              {
                                "ParamType": 5,
                                "value": "true"
                              }
                            ],
                            "value": "OperatorCompareBoolean"
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
                                      },
                                      {
                                        "ParamType": 5,
                                        "value": "BEer"
                                      }
                                    ],
                                    "value": "UnitHasBuffBJ"
                                  },
                                  {
                                    "ParamType": 2,
                                    "value": "OperatorEqualENE"
                                  },
                                  {
                                    "ParamType": 5,
                                    "value": "true"
                                  }
                                ],
                                "value": "OperatorCompareBoolean"
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
                                          },
                                          {
                                            "ParamType": 5,
                                            "value": "BHbn"
                                          }
                                        ],
                                        "value": "UnitHasBuffBJ"
                                      },
                                      {
                                        "ParamType": 2,
                                        "value": "OperatorEqualENE"
                                      },
                                      {
                                        "ParamType": 5,
                                        "value": "true"
                                      }
                                    ],
                                    "value": "OperatorCompareBoolean"
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
                                              },
                                              {
                                                "ParamType": 5,
                                                "value": "BOhx"
                                              }
                                            ],
                                            "value": "UnitHasBuffBJ"
                                          },
                                          {
                                            "ParamType": 2,
                                            "value": "OperatorEqualENE"
                                          },
                                          {
                                            "ParamType": 5,
                                            "value": "true"
                                          }
                                        ],
                                        "value": "OperatorCompareBoolean"
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
                                                  },
                                                  {
                                                    "ParamType": 5,
                                                    "value": "BNso"
                                                  }
                                                ],
                                                "value": "UnitHasBuffBJ"
                                              },
                                              {
                                                "ParamType": 2,
                                                "value": "OperatorEqualENE"
                                              },
                                              {
                                                "ParamType": 5,
                                                "value": "true"
                                              }
                                            ],
                                            "value": "OperatorCompareBoolean"
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
                                                  },
                                                  {
                                                    "ParamType": 5,
                                                    "value": "Bcyc"
                                                  }
                                                ],
                                                "value": "UnitHasBuffBJ"
                                              },
                                              {
                                                "ParamType": 2,
                                                "value": "OperatorEqualENE"
                                              },
                                              {
                                                "ParamType": 5,
                                                "value": "true"
                                              }
                                            ],
                                            "value": "OperatorCompareBoolean"
                                          }
                                        ],
                                        "value": "GetBooleanOr"
                                      }
                                    ],
                                    "value": "GetBooleanOr"
                                  }
                                ],
                                "value": "GetBooleanOr"
                              }
                            ],
                            "value": "GetBooleanOr"
                          }
                        ],
                        "value": "GetBooleanOr"
                      }
                    ],
                    "value": "GetBooleanAnd"
                  }
                ],
                "value": "GetUnitsInRangeOfLocMatching"
              }
            ],
            "value": "SetVariable"
          }
        },
        {
          "ElementType": 1,
          "If": [
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
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
                    "value": "CountUnitsInGroup"
                  },
                  {
                    "ParamType": 2,
                    "value": "OperatorGreater"
                  },
                  {
                    "ParamType": 5,
                    "value": "0"
                  }
                ],
                "value": "OperatorCompareInteger"
              }
            }
          ],
          "Then": [
            {
              "ElementType": 9,
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 3,
                    "VariableId": 100663328,
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
                    "value": "GroupPickRandomUnit"
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
                    "ParamType": 3,
                    "VariableId": 100663325,
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
                    "value": "UnitOrderPurge"
                  },
                  {
                    "ParamType": 3,
                    "VariableId": 100663328,
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
                "value": "IssueTargetOrder"
              }
            }
          ],
          "Else": [
            {
              "ElementType": 9,
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 3,
                    "VariableId": 100663324,
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
                        "ParamType": 5,
                        "value": "750.00"
                      },
                      {
                        "ParamType": 3,
                        "VariableId": 100663297,
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
                            "parameters": [
                              {
                                "ParamType": 1,
                                "parameters": [
                                  {
                                    "ParamType": 1,
                                    "parameters": [],
                                    "value": "GetFilterUnit"
                                  },
                                  {
                                    "ParamType": 1,
                                    "parameters": [
                                      {
                                        "ParamType": 3,
                                        "VariableId": 100663325,
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
                                    "value": "GetOwningPlayer"
                                  }
                                ],
                                "value": "IsUnitEnemy"
                              },
                              {
                                "ParamType": 2,
                                "value": "OperatorEqualENE"
                              },
                              {
                                "ParamType": 5,
                                "value": "true"
                              }
                            ],
                            "value": "OperatorCompareBoolean"
                          },
                          {
                            "ParamType": 1,
                            "parameters": [
                              {
                                "ParamType": 1,
                                "parameters": [
                                  {
                                    "ParamType": 2,
                                    "value": "BuffPolarityNegative"
                                  },
                                  {
                                    "ParamType": 2,
                                    "value": "BuffResistMagic"
                                  },
                                  {
                                    "ParamType": 1,
                                    "parameters": [],
                                    "value": "GetFilterUnit"
                                  },
                                  {
                                    "ParamType": 2,
                                    "value": "InclusionExclude"
                                  },
                                  {
                                    "ParamType": 2,
                                    "value": "InclusionExclude"
                                  }
                                ],
                                "value": "UnitCountBuffsExBJ"
                              },
                              {
                                "ParamType": 2,
                                "value": "OperatorLess"
                              },
                              {
                                "ParamType": 1,
                                "parameters": [
                                  {
                                    "ParamType": 2,
                                    "value": "BuffPolarityPositive"
                                  },
                                  {
                                    "ParamType": 2,
                                    "value": "BuffResistMagic"
                                  },
                                  {
                                    "ParamType": 1,
                                    "parameters": [],
                                    "value": "GetFilterUnit"
                                  },
                                  {
                                    "ParamType": 2,
                                    "value": "InclusionExclude"
                                  },
                                  {
                                    "ParamType": 2,
                                    "value": "InclusionExclude"
                                  }
                                ],
                                "value": "UnitCountBuffsExBJ"
                              }
                            ],
                            "value": "OperatorCompareInteger"
                          }
                        ],
                        "value": "GetBooleanAnd"
                      }
                    ],
                    "value": "GetUnitsInRangeOfLocMatching"
                  }
                ],
                "value": "SetVariable"
              }
            },
            {
              "ElementType": 1,
              "If": [
                {
                  "isEnabled": true,
                  "function": {
                    "ParamType": 1,
                    "parameters": [
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 3,
                            "VariableId": 100663324,
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
                        "value": "CountUnitsInGroup"
                      },
                      {
                        "ParamType": 2,
                        "value": "OperatorGreater"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": "OperatorCompareInteger"
                  }
                }
              ],
              "Then": [
                {
                  "ElementType": 9,
                  "isEnabled": true,
                  "function": {
                    "ParamType": 1,
                    "parameters": [
                      {
                        "ParamType": 3,
                        "VariableId": 100663328,
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
                            "ParamType": 3,
                            "VariableId": 100663324,
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
                        "value": "GroupPickRandomUnit"
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
                        "ParamType": 3,
                        "VariableId": 100663325,
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
                        "value": "UnitOrderPurge"
                      },
                      {
                        "ParamType": 3,
                        "VariableId": 100663328,
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
                    "value": "IssueTargetOrder"
                  }
                }
              ],
              "Else": [
                {
                  "ElementType": 9,
                  "isEnabled": true,
                  "function": {
                    "ParamType": 1,
                    "parameters": [
                      {
                        "ParamType": 3,
                        "VariableId": 100663365,
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
                            "ParamType": 5,
                            "value": "500.00"
                          },
                          {
                            "ParamType": 3,
                            "VariableId": 100663297,
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
                                "parameters": [
                                  {
                                    "ParamType": 1,
                                    "parameters": [
                                      {
                                        "ParamType": 1,
                                        "parameters": [],
                                        "value": "GetFilterUnit"
                                      },
                                      {
                                        "ParamType": 1,
                                        "parameters": [
                                          {
                                            "ParamType": 3,
                                            "VariableId": 100663325,
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
                                        "value": "GetOwningPlayer"
                                      }
                                    ],
                                    "value": "IsUnitEnemy"
                                  },
                                  {
                                    "ParamType": 2,
                                    "value": "OperatorEqualENE"
                                  },
                                  {
                                    "ParamType": 5,
                                    "value": "false"
                                  }
                                ],
                                "value": "OperatorCompareBoolean"
                              },
                              {
                                "ParamType": 1,
                                "parameters": [
                                  {
                                    "ParamType": 1,
                                    "parameters": [
                                      {
                                        "ParamType": 2,
                                        "value": "BuffPolarityNegative"
                                      },
                                      {
                                        "ParamType": 2,
                                        "value": "BuffResistMagic"
                                      },
                                      {
                                        "ParamType": 1,
                                        "parameters": [],
                                        "value": "GetFilterUnit"
                                      },
                                      {
                                        "ParamType": 2,
                                        "value": "InclusionExclude"
                                      },
                                      {
                                        "ParamType": 2,
                                        "value": "InclusionExclude"
                                      }
                                    ],
                                    "value": "UnitCountBuffsExBJ"
                                  },
                                  {
                                    "ParamType": 2,
                                    "value": "OperatorGreater"
                                  },
                                  {
                                    "ParamType": 1,
                                    "parameters": [
                                      {
                                        "ParamType": 2,
                                        "value": "BuffPolarityPositive"
                                      },
                                      {
                                        "ParamType": 2,
                                        "value": "BuffResistMagic"
                                      },
                                      {
                                        "ParamType": 1,
                                        "parameters": [],
                                        "value": "GetFilterUnit"
                                      },
                                      {
                                        "ParamType": 2,
                                        "value": "InclusionExclude"
                                      },
                                      {
                                        "ParamType": 2,
                                        "value": "InclusionExclude"
                                      }
                                    ],
                                    "value": "UnitCountBuffsExBJ"
                                  }
                                ],
                                "value": "OperatorCompareInteger"
                              }
                            ],
                            "value": "GetBooleanAnd"
                          }
                        ],
                        "value": "GetUnitsInRangeOfLocMatching"
                      }
                    ],
                    "value": "SetVariable"
                  }
                },
                {
                  "ElementType": 1,
                  "If": [
                    {
                      "isEnabled": true,
                      "function": {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 1,
                            "parameters": [
                              {
                                "ParamType": 3,
                                "VariableId": 100663365,
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
                            "value": "CountUnitsInGroup"
                          },
                          {
                            "ParamType": 2,
                            "value": "OperatorGreater"
                          },
                          {
                            "ParamType": 5,
                            "value": "0"
                          }
                        ],
                        "value": "OperatorCompareInteger"
                      }
                    }
                  ],
                  "Then": [
                    {
                      "ElementType": 9,
                      "isEnabled": true,
                      "function": {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 3,
                            "VariableId": 100663328,
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
                                "ParamType": 3,
                                "VariableId": 100663365,
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
                            "value": "GroupPickRandomUnit"
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
                            "ParamType": 3,
                            "VariableId": 100663325,
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
                            "value": "UnitOrderPurge"
                          },
                          {
                            "ParamType": 3,
                            "VariableId": 100663328,
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
                        "value": "IssueTargetOrder"
                      }
                    }
                  ],
                  "Else": [],
                  "isEnabled": true,
                  "function": {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "IfThenElseMultiple"
                  }
                },
                {
                  "isEnabled": true,
                  "function": {
                    "ParamType": 1,
                    "parameters": [
                      {
                        "ParamType": 5,
                        "value": "call DestroyGroup(udg_UnitGroup3)"
                      }
                    ],
                    "value": "CustomScriptCode"
                  }
                }
              ],
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [],
                "value": "IfThenElseMultiple"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "call DestroyGroup(udg_UnitGroup2)"
                  }
                ],
                "value": "CustomScriptCode"
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
                    "VariableId": 100663324,
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
                        "ParamType": 5,
                        "value": "650.00"
                      },
                      {
                        "ParamType": 3,
                        "VariableId": 100663297,
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
                            "parameters": [
                              {
                                "ParamType": 1,
                                "parameters": [
                                  {
                                    "ParamType": 1,
                                    "parameters": [],
                                    "value": "GetFilterUnit"
                                  },
                                  {
                                    "ParamType": 2,
                                    "value": "UnitTypeSummoned"
                                  }
                                ],
                                "value": "IsUnitType"
                              },
                              {
                                "ParamType": 2,
                                "value": "OperatorEqualENE"
                              },
                              {
                                "ParamType": 5,
                                "value": "true"
                              }
                            ],
                            "value": "OperatorCompareBoolean"
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
                                      },
                                      {
                                        "ParamType": 1,
                                        "parameters": [
                                          {
                                            "ParamType": 3,
                                            "VariableId": 100663325,
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
                                        "value": "GetOwningPlayer"
                                      }
                                    ],
                                    "value": "IsUnitEnemy"
                                  },
                                  {
                                    "ParamType": 2,
                                    "value": "OperatorEqualENE"
                                  },
                                  {
                                    "ParamType": 5,
                                    "value": "true"
                                  }
                                ],
                                "value": "OperatorCompareBoolean"
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
                                    "value": "IsUnitAliveBJ"
                                  },
                                  {
                                    "ParamType": 2,
                                    "value": "OperatorEqualENE"
                                  },
                                  {
                                    "ParamType": 5,
                                    "value": "true"
                                  }
                                ],
                                "value": "OperatorCompareBoolean"
                              }
                            ],
                            "value": "GetBooleanAnd"
                          }
                        ],
                        "value": "GetBooleanAnd"
                      }
                    ],
                    "value": "GetUnitsInRangeOfLocMatching"
                  }
                ],
                "value": "SetVariable"
              }
            },
            {
              "ElementType": 1,
              "If": [
                {
                  "isEnabled": true,
                  "function": {
                    "ParamType": 1,
                    "parameters": [
                      {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 3,
                            "VariableId": 100663324,
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
                        "value": "CountUnitsInGroup"
                      },
                      {
                        "ParamType": 2,
                        "value": "OperatorGreater"
                      },
                      {
                        "ParamType": 5,
                        "value": "0"
                      }
                    ],
                    "value": "OperatorCompareInteger"
                  }
                }
              ],
              "Then": [
                {
                  "ElementType": 9,
                  "isEnabled": true,
                  "function": {
                    "ParamType": 1,
                    "parameters": [
                      {
                        "ParamType": 3,
                        "VariableId": 100663328,
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
                            "ParamType": 3,
                            "VariableId": 100663324,
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
                        "value": "GroupPickRandomUnit"
                      }
                    ],
                    "value": "SetVariable"
                  }
                },
                {
                  "ElementType": 1,
                  "If": [
                    {
                      "isEnabled": true,
                      "function": {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 1,
                            "parameters": [
                              {
                                "ParamType": 3,
                                "VariableId": 100663328,
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
                            "value": "GetUnitTypeId"
                          },
                          {
                            "ParamType": 2,
                            "value": "OperatorNotEqualENE"
                          },
                          {
                            "ParamType": 5,
                            "value": "uplg"
                          }
                        ],
                        "value": "OperatorCompareUnitCode"
                      }
                    }
                  ],
                  "Then": [
                    {
                      "isEnabled": true,
                      "function": {
                        "ParamType": 1,
                        "parameters": [
                          {
                            "ParamType": 3,
                            "VariableId": 100663325,
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
                            "value": "UnitOrderPurge"
                          },
                          {
                            "ParamType": 3,
                            "VariableId": 100663328,
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
                        "value": "IssueTargetOrder"
                      }
                    }
                  ],
                  "Else": [],
                  "isEnabled": true,
                  "function": {
                    "ParamType": 1,
                    "parameters": [],
                    "value": "IfThenElseMultiple"
                  }
                }
              ],
              "Else": [],
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [],
                "value": "IfThenElseMultiple"
              }
            },
            {
              "isEnabled": true,
              "function": {
                "ParamType": 1,
                "parameters": [
                  {
                    "ParamType": 5,
                    "value": "call DestroyGroup(udg_UnitGroup2)"
                  }
                ],
                "value": "CustomScriptCode"
              }
            }
          ],
          "isEnabled": true,
          "function": {
            "ParamType": 1,
            "parameters": [],
            "value": "IfThenElseMultiple"
          }
        },
        {
          "isEnabled": true,
          "function": {
            "ParamType": 1,
            "parameters": [
              {
                "ParamType": 5,
                "value": "call RemoveLocation(udg_Point)"
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
                "ParamType": 5,
                "value": "call DestroyGroup(udg_UnitGroup)"
              }
            ],
            "value": "CustomScriptCode"
          }
        }
      ],
      "Else": [],
      "isEnabled": true,
      "function": {
        "ParamType": 1,
        "parameters": [],
        "value": "IfThenElseMultiple"
      }
    }
  ]
}