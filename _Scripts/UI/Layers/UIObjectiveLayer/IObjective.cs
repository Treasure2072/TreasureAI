using UnityEngine;

namespace Game
{
    public interface IObjective
    {
        public Transform GetTransform();

        public void SetContent();

        public void RefreshCollectStatus();

        public void SetClaimNums();
    }
}