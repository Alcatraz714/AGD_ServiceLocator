using System.Collections.Generic;
using UnityEngine;
using ServiceLocator.Main;
using ServiceLocator.Player;
using ServiceLocator.Sound;

namespace ServiceLocator.Wave.Bloon
{
    public class BloonController
    {
        private BloonView bloonView;
        private BloonScriptableObject bloonScriptableObject;

        private PlayerService playerService_r;
        private WaveService waveService_r;
        private SoundService soundService_r;

        private const float waypointThreshold = 0.1f;
        private List<Vector3> waypoints;
        private int currentHealth;
        private int currentWaypointIndex;
        private BloonState currentState;

        public Vector3 Position => bloonView.transform.position;

        public BloonController(BloonView bloonPrefab, Transform bloonContainer, PlayerService playerService, WaveService waveService, SoundService soundService)
        {
            // DI
            this.playerService_r = playerService;
            this.waveService_r = waveService;
            this.soundService_r = soundService;

            bloonView = Object.Instantiate(bloonPrefab, bloonContainer);
            bloonView.Controller = this;
        }

        public void Init(BloonScriptableObject bloonScriptableObject)
        {
            this.bloonScriptableObject = bloonScriptableObject;
            InitializeVariables();
            SetState(BloonState.ACTIVE);
        }

        private void InitializeVariables()
        {
            bloonView.SetRenderer(bloonScriptableObject.Sprite);
            currentHealth = bloonScriptableObject.Health;
            waypoints = new List<Vector3>();
        }

        public void SetPosition(Vector3 spawnPosition)
        {
            bloonView.transform.position = spawnPosition;
            bloonView.gameObject.SetActive(true);
        }

        public void SetWayPoints(List<Vector3> waypointsToSet, int startingWaypointIndex)
        {
            waypoints = waypointsToSet;
            currentWaypointIndex = startingWaypointIndex;
        }

        public void TakeDamage(int damageToTake)
        {
            int reducedHealth = currentHealth - damageToTake;
            currentHealth = reducedHealth <= 0 ? 0 : reducedHealth;

            if (currentHealth <= 0 && currentState == BloonState.ACTIVE)
            {
                PopBloon();
                soundService_r.PlaySoundEffects(Sound.SoundType.BloonPop);
            }
        }

        public void FollowWayPoints()
        {
            if(HasReachedFinalWaypoint())
            {
                ResetBloon();
            }
            else
            {
                Vector3 direction = GetDirectionToMoveTowards();
                MoveBloon(direction);
                if (HasReachedNextWaypoint(direction.magnitude))
                    currentWaypointIndex++;
            }
        }

        private bool HasReachedFinalWaypoint() => currentWaypointIndex == waypoints.Count;

        private bool HasReachedNextWaypoint(float distance) => distance < waypointThreshold;

        private void ResetBloon()
        {
            waveService_r.RemoveBloon(this);
            playerService_r.TakeDamage(bloonScriptableObject.Damage);
            bloonView.gameObject.SetActive(false);
        }

        private Vector3 GetDirectionToMoveTowards() => waypoints[currentWaypointIndex] - bloonView.transform.position;

        private void MoveBloon(Vector3 moveDirection) => bloonView.transform.Translate(moveDirection.normalized * bloonScriptableObject.Speed * Time.deltaTime);

        public void SetOrderInLayer(int orderInLayer) => bloonView.SetSortingOrder(orderInLayer);

        private void PopBloon()
        {
            SetState(BloonState.POPPED);
            bloonView.PopBloon();
        }

        public void OnPopAnimationPlayed()
        {
            if (HasLayeredBloons())
                SpawnLayeredBloons();

            playerService_r.GetReward(bloonScriptableObject.Reward);
            waveService_r.RemoveBloon(this);
        }

        private bool HasLayeredBloons() => bloonScriptableObject.LayeredBloons.Count > 0;

        private void SpawnLayeredBloons() => waveService_r.SpawnBloons(bloonScriptableObject.LayeredBloons,
                                                                                          bloonView.transform.position,
                                                                                          currentWaypointIndex,
                                                                                          bloonScriptableObject.LayerBloonSpawnRate);

        public BloonType GetBloonType() => bloonScriptableObject.Type;

        private void SetState(BloonState state) => currentState = state;
    }

    public enum BloonState
    {
        ACTIVE,
        POPPED
    }
}