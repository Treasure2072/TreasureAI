
using System.Collections.Generic;
using Data;
using DragonLi.Core;
using UnityEngine;

namespace Game
{
    public class MatchThreeInstance : Singleton<MatchThreeInstance>
    {
        private MiniGameSettings _settings;

        public MiniGameSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = Resources.Load<MiniGameSettings>(nameof(MiniGameSettings));
                }

                return _settings;
            }
            set
            {
                if (_settings) return;
                _settings = value;
            }
        }
        
        private Dictionary<string, GameObject> MatchesElementDesign { get; set; } = new();
        
        private Dictionary<string, GameObject> EffectDesign { get; set; } = new();
        
        private void AddRegistryInRouter()
        {
            MatchesElementDesign.Clear();
            EffectDesign.Clear();
            foreach (var element in Settings.elements)
            {
                var succeed = MatchesElementDesign.TryAdd(element.name, element.gameObject);
                if (!succeed) this.LogEditorOnly(element.name + " is already registered");
                
                EffectDesign.Add(element.name, element.effect);
            }
        }

        public void Initialized()
        {
            AddRegistryInRouter();
        }
        
        public HashSet<string> GetMatchesType()
        {
            var matches = new HashSet<string>();
            foreach (var kv in MatchesElementDesign) matches.Add(kv.Key);
            return matches;
        }
        
        public GameObject GetMatchThreeElement(string elementName)
        {
            return MatchesElementDesign.GetValueOrDefault(elementName);
        }

        public GameObject GetMatchThreeEffect(string elementName)
        {
            return EffectDesign.GetValueOrDefault(elementName);
        }

        public IReadOnlyCollection<GameObject> GetMatchThreeElements()
        {
            return MatchesElementDesign.Values;
        }
    }
}