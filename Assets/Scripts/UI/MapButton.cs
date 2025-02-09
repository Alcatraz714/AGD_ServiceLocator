using UnityEngine;
using UnityEngine.UI;
using ServiceLocator.Main;

namespace ServiceLocator.UI
{
    public class MapButton : MonoBehaviour
    {
        [SerializeField] private int MapId;

        private void Start() => GetComponent<Button>().onClick.AddListener(OnMapButtonClicked);

        private void OnMapButtonClicked() =>  GameService.Instance.EventService.OnMapSelected.InvokeEvent(MapId);
    }
}