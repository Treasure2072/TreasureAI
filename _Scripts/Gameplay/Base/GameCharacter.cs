using System.Collections.Generic;
using Data;
using DragonLi.Core;
using UnityEngine;

namespace Game
{
    public abstract class GameCharacter : MonoBehaviour
    {
        #region Properties

        [Header("Settings")] 
        [SerializeField] protected Transform model;
        [SerializeField] private List<GameObject> characterModels;
        
        [Header("Movement")]
        [SerializeField] protected ChessGameCharacterMovement movement;
        
        private GameObject CharacterModel { get; set; }
        private IChessGameCharacterAnimator CharacterAnimator { get; set; }
        
        #endregion

        #region API

        protected void SetCharacterModel(int index)
        {
            if (!characterModels.IsValidIndex(index))
            {
                this.LogErrorEditorOnly("Character model index is invalid.");
                return;
            }

            if (CharacterModel)
            {
                Destroy(CharacterModel);
                CharacterAnimator = null;
            }
            
            CharacterModel = Instantiate(characterModels[index], model);
            if (CharacterModel == null)
            {
                this.LogErrorEditorOnly("Failed to instantiate character model.");
                return;
            }
            
            CharacterModel.transform.SetParent(model);
            CharacterModel.transform.localPosition = Vector3.zero;
            CharacterModel.transform.localRotation = Quaternion.identity;
            CharacterModel.transform.localScale = Vector3.one;
            CharacterAnimator = CharacterModel.GetComponentInChildren<IChessGameCharacterAnimator>();
            if (CharacterAnimator == null)
            {
                this.LogErrorEditorOnly("Character model does not have a character animator.");
            }
        }

        public IChessGameCharacterAnimator GetCharacterAnimatorInterface()
        {
            return CharacterAnimator;
        }
        
        public int GetStandIndex()
        {
            return PlayerSandbox.Instance.ChessBoardHandler.StandIndex;
        }

        public ChessGameCharacterMovement GetMovement()
        {
            return movement;
        }

        #endregion
    }
}


