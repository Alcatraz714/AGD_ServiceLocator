using System.Collections;
using System.Collections.Generic;
using ServiceLocator.Player;
using ServiceLocator.Sound;
using ServiceLocator.UI;
using ServiceLocator.Utilities;
using UnityEngine;

public class GameService : GenericMonoSingleton<GameService>
{
    public PlayerService playerService {get; private set;}
    public SoundService soundService {get; private set;}

    [SerializeField] private UIService uIService;
    public UIService UIService => uIService;

    // Services for Service Locator
    [SerializeField] private PlayerScriptableObject playerScriptableObject;
    [SerializeField] private SoundScriptableObject soundScriptableObject;
    [SerializeField] private AudioSource audioEffects;
    [SerializeField] private AudioSource backgroundMusic;

    private void Start() 
    {
        //Init services
        playerService = new PlayerService(playerScriptableObject);// initial load for SO player service
        soundService = new SoundService(soundScriptableObject, audioEffects, backgroundMusic);
    }

    private void Update() 
    {
        playerService.Update();
    }
}
