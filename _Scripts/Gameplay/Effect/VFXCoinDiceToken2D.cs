using System;
using DragonLi.Core;
using UnityEngine;

namespace Game
{
    public class VFXCoinDiceToken2D : DummyPoolObject
    {
        #region Fields

        [Header("References")]
        [SerializeField] private ParticleSystem particleCoin;
        [SerializeField] private ParticleSystem particleDice;
        [SerializeField] private ParticleSystem particleDiamond;

        [Header("Settings")]
        [SerializeField] private int maxParticleCoin = 200;
        [SerializeField] private int maxParticleDice = 30;
        [SerializeField] private int maxParticleDiamond = 20;
        
        #endregion

        #region Property

        private ParticleSystem[] Particles { get; set; }

        #endregion

        #region Unity

        private void Awake()
        {
            Particles = GetComponentsInChildren<ParticleSystem>(includeInactive: true);
        }

        #endregion

        #region Function

        private void PlayerOther()
        {
            foreach (var particle in Particles)
            {
                if(particle == particleCoin || particle == particleDice || particle == particleDiamond) continue;
                particle.Play();
            }
        }

        private void PlayCoin(int particleNumber)
        {
            particleCoin.gameObject.SetActive(particleNumber > 0);
            if (particleNumber <= 0)
            {
                particleCoin.Stop();
                return;
            }
            if(particleNumber > maxParticleCoin) particleNumber = maxParticleCoin;
            var burst = particleCoin.emission.GetBurst(0);
            burst.count = particleNumber;
            particleCoin.emission.SetBurst(0, burst);
            particleCoin.Play();
        }

        private void PlayDice(int particleNumber)
        {
            particleDice.gameObject.SetActive(particleNumber > 0);
            if (particleNumber <= 0)
            {
                particleDice.Stop();
                return;
            }
            if(particleNumber > maxParticleDice) particleNumber = maxParticleDice;
            var burst = particleDice.emission.GetBurst(0);
            burst.count = particleNumber;
            particleDice.emission.SetBurst(0, burst);
            particleDice.Play();
        }

        private void PlayDiamond(int particleNumber)
        {
            particleDiamond.gameObject.SetActive(particleNumber > 0);
            if (particleNumber <= 0)
            {
                particleDiamond.Stop();
                return;
            }
            if(particleNumber > maxParticleDiamond) particleNumber = maxParticleDiamond;
            var burst = particleDiamond.emission.GetBurst(0);
            burst.count = particleNumber;
            particleDiamond.emission.SetBurst(0, burst);
            particleDiamond.Play();
        }

        #endregion

        #region API

        public void Play(int coinNum, int diceNum, int diamondNum)
        {
            PlayCoin(coinNum);
            PlayDice(diceNum);
            PlayDiamond(diamondNum);

            if (coinNum > 0 || diceNum > 0 || diamondNum > 0)
            {
                PlayerOther();
            }
        }
            
        #endregion
    }
}