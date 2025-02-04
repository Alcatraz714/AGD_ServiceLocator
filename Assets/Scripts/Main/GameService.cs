using System.Collections;
using System.Collections.Generic;
using ServiceLocator.Player;
using ServiceLocator.Utilities;
using UnityEngine;

public class GameService : GenericMonoSingleton<GameService>
{
    public PlayerService playerService {get; private set;}

    // Services for Service Locator
    [SerializeField] public PlayerScriptableObject playerScriptableObject;

    private void Start() 
    {
        //Init services
        playerService = new PlayerService(playerScriptableObject);// initial load for SO player service   
    }

    private void Update() 
    {
        playerService.Update();
        
    }
}
