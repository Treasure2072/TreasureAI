using DragonLi.Core;

namespace Game
{
    public class GameInstance : Singleton<GameInstance>
    {
        #region Properties - Handler

        public HostingHandler HostingHandler { get; private set; }
        
        public bool EnteredMainScene { get; set; }
        
        public bool IsChessBoardScene { get; set; }
        
        public int MoveCounter { get; set; }
        
        public int MoveReceived { get; set; }

        #endregion

        #region API

        public void Initialize()
        {
            HostingHandler = new HostingHandler();
        }

        #endregion
    }
}