using DragonLi.Core;

namespace Game
{
    public class ChessTileEvent : IQueueableEvent
    {
        protected ChessTile TargetTile { get; private set; }
        public ChessTileEvent(ChessTile tile)
        {
            TargetTile = tile;
        }
        
        public virtual void OnQueue() {}
        public virtual void OnExecute() {}
        public virtual void OnDequeue() {}
        public virtual void OnCancel() {}
        public virtual bool OnTick() { return true; }
        public virtual void OnFinish() {}
    }
}


