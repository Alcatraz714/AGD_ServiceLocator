using UnityEngine;
using UnityEngine.UI;
using ServiceLocator.Main;
using ServiceLocator.Events;

namespace ServiceLocator.UI
{
    public class MapButton : MonoBehaviour
    {
        [SerializeField] private int MapId;
        private EventService eventService_r;

        private void Start() => GetComponent<Button>().onClick.AddListener(OnMapButtonClicked);
        public void Init(EventService eventService)
        {
            this.eventService_r = eventService;
        }

        private void OnMapButtonClicked() =>  eventService_r.OnMapSelected.InvokeEvent(MapId);
    }
}