using static War3Api.Common;

namespace War3Map.FZero.Source
{
  internal class Program
  {
    private static void Main()
    {
      var p = Player( 0 );
      if ( GetPlayerSlotState( p ) == PLAYER_SLOT_STATE_PLAYING )
      {
        TriggerAddCondition( CreateTrigger(), Condition( () => false ) );
      }
    }
  }
}
