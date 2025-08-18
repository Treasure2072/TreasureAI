using UnityEngine.Events;

namespace Game
{
    public interface ITutorialStep
    {
        /// <summary>
        /// 开始步骤
        /// </summary>
        /// <param name="onFinishSpeak"></param>
        void StartStep(UnityAction onFinishSpeak = null);
        
        /// <summary>
        /// 打断步骤
        /// </summary>
        void InterruptStep();
        
        /// <summary>
        /// 结束步骤
        /// </summary>
        void FinishStep();
        
        /// <summary>
        /// 是否能跳过
        /// </summary>
        bool Skippable { get; }
    }
}