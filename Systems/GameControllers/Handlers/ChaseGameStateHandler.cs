using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChaseGameStateHandler : MonoCached // IState
{
    [SerializeField]
    private RoadHandler roadHandler;
    [SerializeField]
    private EnemyHandler enemyHandler;
    [SerializeField]
    private PlayerHandler playerHandler;
    [SerializeField]
    private InputHandler inputHandler;
    [SerializeField]
    private PathHandler pathHandler;
    [SerializeField]
    private EventHandler eventHandler;
    [SerializeField]
    private PlayerSessionHandler currentSessionHandler;
    [SerializeField]
    private ObjectPoolersHolder objectPoolersHolder;

    private void Start()
    {
        StartGame();
    }
    void StartGame()
    {
        customFixedUpdates.Add(this);
        customUpdates.Add(this);

        objectPoolersHolder.AwakeGeneralGameStatePoolers();
        inputHandler.SetUpControlsUI();
        

        #region InjectDependencies
        roadHandler.InjectPathHandler(pathHandler);
        pathHandler.InjectEnemyHandler(enemyHandler);
        enemyHandler.InjectPathHandler(pathHandler);
        #endregion
        #region WorldCreation

        playerHandler.CreatePlayer();
        //playerHandler.FindEnemyPlayer();
        //enemyHandler.AddEnemyPlayerToCurrentSession(playerHandler.player.GetComponent<Enemy>());
        playerHandler.CreateCamera();
        pathHandler.CreateGrid(new Vector3(0, 0, 0));

        #endregion
        #region StartGamePlay

        roadHandler.StartRoadHandle(playerHandler.player_rigidbody);
        enemyHandler.StartCheckingEnemiesPositions(playerHandler.player_rigidbody);
        enemyHandler.StartSpawnRandomEnemyEveryPeriod(playerHandler.player_rigidbody);
        //enemyHandler.StartSpawnAllEnemiesEveryPeriod(playerHandler.player_rigidbody);
        //enemyHandler.StartIncrementMaxEnemiesCount(5f);
        eventHandler.StartCheckDistance(playerHandler.player_transform.position, playerHandler.player_rigidbody);

        playerHandler.StartUpdateCamera();
        inputHandler.StartUpdateInputs();
        currentSessionHandler.StartHandleSession();
        #endregion
    }

}
