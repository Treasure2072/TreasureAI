using _Scripts.UI.Common.Grids;
using UnityEngine;

namespace Game
{
    public class LotteryDrawSlot : GridBase
    {
        #region Property

        private GameObject Obj { get; set; }

        #endregion
        
        #region GridBase

        protected override void OnRecycle()
        {
            base.OnRecycle();
            if (Obj != null)
            {
                Destroy(Obj);
                Obj = null;
            }
        }

        public override void SetGrid(params object[] args)
        {
            base.SetGrid(args);
            var type = args[0] is ELotteryDrawType ? (ELotteryDrawType)args[0] : ELotteryDrawType.Blue;

            var data = UILotteryDrawLayer.GetLayer().GetLotteryDraw(type);
            Obj = Instantiate(data.prefab, transform, false);
            Obj.transform.localPosition = Vector3.zero;
            Obj.transform.localScale = Vector3.one;
            Obj.transform.localRotation = Quaternion.identity;
        }

        #endregion
    }
}