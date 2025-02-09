using System.Collections.Generic;
using UnityEngine;
using ServiceLocator.Wave.Bloon;
using System.Threading.Tasks;
using ServiceLocator.Main;
using ServiceLocator.Events;
using ServiceLocator.UI;
using ServiceLocator.Map;
using ServiceLocator.Sound;
using ServiceLocator.Player;

namespace ServiceLocator.Wave
{
    public class WaveService
    {
        private WaveScriptableObject waveScriptableObject;
        private BloonPool bloonPool;

        private EventService eventService_r;
        private UIService uIService_r;
        private MapService mapService_r;
        private SoundService soundService_r;
        private PlayerService playerService_r;

        private int currentWaveId;
        private List<WaveData> waveDatas;
        private List<BloonController> activeBloons;

        public WaveService(WaveScriptableObject waveScriptableObject)
        {
            this.waveScriptableObject = waveScriptableObject;
            //InitializeBloons();
            //SubscribeToEvents();
        }

        public void Init(EventService eventService, UIService uIService, MapService mapService, SoundService soundService, PlayerService playerService)
        {
            this.eventService_r = eventService;
            this.uIService_r = uIService;
            this.mapService_r = mapService;
            this.soundService_r = soundService;
            this.playerService_r = playerService;
            InitializeBloons();
            SubscribeToEvents();
        }

        private void InitializeBloons()
        {
            bloonPool = new BloonPool(waveScriptableObject, playerService_r, this, soundService_r);
            activeBloons = new List<BloonController>();
        }

        private void SubscribeToEvents() => eventService_r.OnMapSelected.AddListener(LoadWaveDataForMap);

        private void LoadWaveDataForMap(int mapId)
        {
            currentWaveId = 0;
            waveDatas = waveScriptableObject.WaveConfigurations.Find(config => config.MapID == mapId).WaveDatas;
            uIService_r.UpdateWaveProgressUI(currentWaveId, waveDatas.Count);
        }

        public void StarNextWave()
        {
            currentWaveId++;
            var bloonsToSpawn = GetBloonsForCurrentWave();
            var spawnPosition = mapService_r.GetBloonSpawnPositionForCurrentMap();
            SpawnBloons(bloonsToSpawn, spawnPosition, 0, waveScriptableObject.SpawnRate);
        }

        public async void SpawnBloons(List<BloonType> bloonsToSpawn, Vector3 spawnPosition, int startingWaypointIndex, float spawnRate)
        {
            foreach(BloonType bloonType in bloonsToSpawn)
            {
                BloonController bloon = bloonPool.GetBloon(bloonType);
                bloon.SetPosition(spawnPosition);
                bloon.SetWayPoints(mapService_r.GetWayPointsForCurrentMap(), startingWaypointIndex);

                AddBloon(bloon);
                await Task.Delay(Mathf.RoundToInt(spawnRate * 1000));
            }
        }

        private void AddBloon(BloonController bloonToAdd)
        {
            activeBloons.Add(bloonToAdd);
            bloonToAdd.SetOrderInLayer(-activeBloons.Count);
        }

        public void RemoveBloon(BloonController bloon)
        {
            bloonPool.ReturnItem(bloon);
            activeBloons.Remove(bloon);
            if (HasCurrentWaveEnded())
            {
                soundService_r.PlaySoundEffects(Sound.SoundType.WaveComplete);
                uIService_r.UpdateWaveProgressUI(currentWaveId, waveDatas.Count);

                if(IsLevelWon())
                    uIService_r.UpdateGameEndUI(true);
                else
                    uIService_r.SetNextWaveButton(true);
            }
        }

        private List<BloonType> GetBloonsForCurrentWave() => waveDatas.Find(waveData => waveData.WaveID == currentWaveId).ListOfBloons;

        private bool HasCurrentWaveEnded() => activeBloons.Count == 0;

        private bool IsLevelWon() => currentWaveId >= waveDatas.Count;
    }
}