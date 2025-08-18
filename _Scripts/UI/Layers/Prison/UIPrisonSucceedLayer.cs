using DragonLi.UI;
using UnityEngine.Assertions;

namespace Game
{
    public class UIPrisonSucceedLayer : UILayer
    {
        #region API

        public static UIPrisonSucceedLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIPrisonSucceedLayer>("UIPrisonSucceedLayer");
            Assert.IsNotNull(layer);
            return layer;
        }

        #endregion
    }
}
