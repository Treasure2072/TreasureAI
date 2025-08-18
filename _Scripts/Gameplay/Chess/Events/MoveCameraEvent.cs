using DragonLi.Core;
using DragonLi.Frame;
using UnityEngine;

namespace Game
{
    public class MoveCameraEvent : ChessTileEvent
    {

        #region Properties

        private Transform Target { get; set; }
        private float MoveTimeFinish { get; set; }
        
        #endregion
        
        #region ChessTileEvent

        public MoveCameraEvent(ChessTile tile, Transform target, float moveTime = 0.8f) : base(tile)
        {
            Target = target;
            MoveTimeFinish = Time.unscaledTime +  moveTime;
        }

        public override void OnExecute()
        {
            base.OnExecute();

            var cameraTrans = World.GetMainCamera();
            var camera = cameraTrans.GetComponent<DragonLiCameraTopdown>();
            camera.SetOverrideTarget(Target);
        }

        public override bool OnTick()
        {
            return MoveTimeFinish <= Time.unscaledTime;
        }

        #endregion
    }

}