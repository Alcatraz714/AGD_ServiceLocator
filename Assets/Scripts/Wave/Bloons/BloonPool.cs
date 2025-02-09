using System.Collections.Generic;
using UnityEngine;
using ServiceLocator.Utilities;
using ServiceLocator.Player;
using ServiceLocator.Sound;

namespace ServiceLocator.Wave.Bloon
{
    public class BloonPool : GenericObjectPool<BloonController>
    {
        private BloonView bloonPrefab;
        private List<BloonScriptableObject> bloonScriptableObjects;
        private Transform bloonContainer;

        private PlayerService playerService_r;
        private WaveService waveService_r;
        private SoundService soundService_r;

        public BloonPool(WaveScriptableObject waveScriptableObject, PlayerService playerService, WaveService waveService, SoundService soundService)
        {
            // DI
            this.playerService_r = playerService;
            this.waveService_r = waveService;
            this.soundService_r = soundService;

            this.bloonPrefab = waveScriptableObject.BloonPrefab;
            this.bloonScriptableObjects = waveScriptableObject.BloonScriptableObjects;
            bloonContainer = new GameObject("Bloon Container").transform;
        }

        public BloonController GetBloon(BloonType bloonType)
        {
            BloonController bloon = GetItem();
            BloonScriptableObject scriptableObjectToUse = bloonScriptableObjects.Find(so => so.Type == bloonType);
            bloon.Init(scriptableObjectToUse);
            return bloon;
        }

        protected override BloonController CreateItem() => new BloonController(bloonPrefab, bloonContainer, playerService_r, waveService_r, soundService_r);
    }
}