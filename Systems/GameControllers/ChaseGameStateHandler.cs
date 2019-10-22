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
    private ObjectPoolersHolder objectPoolersHolder;


    private void OnEnable()
    {
        StartGame();
    }
    void StartGame()
    {
        allFixedTicks.Add(this);
        allTicks.Add(this);

        objectPoolersHolder.AwakeGeneralGameStatePoolers();

        inputHandler.FindControlsUI();
        

        #region InjectDependencies
        roadHandler.InjectPathHandler(pathHandler);
        pathHandler.InjectEnemyHandler(enemyHandler);
        enemyHandler.InjectPathHandler(pathHandler);
        #endregion
        #region WorldCreation

        playerHandler.CreatePlayer(inputHandler);
        //playerHandler.FindEnemyPlayer();
        //enemyHandler.AddEnemyPlayerToCurrentSession(playerHandler.player.GetComponent<Enemy>());
        playerHandler.CreateCamera();
        inputHandler.FindCamera();
        roadHandler.CreateRoadGrid();
        pathHandler.CreateGrid(new Vector3(0, 0, 0));

        #endregion
        #region StartGamePlay

        roadHandler.StartRoadHandle(playerHandler.player_rigidbody);
        enemyHandler.StartCheckingEnemiesPositions(playerHandler.player_rigidbody);
        enemyHandler.StartSpawnRandomEnemyEveryPeriod(playerHandler.player_rigidbody);
        //enemyHandler.StartSpawnAllEnemiesEveryPeriod(playerHandler.player_rigidbody);
        //enemyHandler.StartIncrementMaxEnemiesCount(5f);
        roadHandler.StartIncrementRoadTroubleCount();
        eventHandler.StartCheckDistance(playerHandler.playerStartPosition, playerHandler.player_rigidbody);

        playerHandler.StartUpdateCamera();
        inputHandler.StartUpdateInputs();
        #endregion
    }

}
