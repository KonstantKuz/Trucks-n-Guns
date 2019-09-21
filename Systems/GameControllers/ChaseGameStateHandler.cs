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

        objectPoolersHolder.AwakePoolers();

        inputHandler.FindControlsUI();

        #region WorldCreation

        playerHandler.CreatePlayer(inputHandler);
        //playerHandler.FindEnemyPlayer();
        //enemyHandler.AddEnemyPlayerToCurrentSession(playerHandler.player.GetComponent<Enemy>());
        playerHandler.CreateCamera();
        inputHandler.FindCamera();
        pathHandler.CreateGrid(new Vector3(0, 0, 0));
        roadHandler.CreateRoadGrid();
        #endregion

        #region InjectDependencies
        roadHandler.InjectPathHandler(pathHandler);
        pathHandler.InjectEnemyHandler(enemyHandler);
        enemyHandler.InjectNeededPooler(objectPoolersHolder.EnemyPooler);
        roadHandler.InjectNeededPooler(objectPoolersHolder.RoadPooler);
        //eventHandler.InjectNeededPooler(objectPoolersHolder.EventPooler);
        #endregion

        #region StartGamePlay

        roadHandler.StartPlayerPositionCheckForMakeRoadComplex(playerHandler.player_rigidbody);
        enemyHandler.StartCheckingEnemiesPositions(playerHandler.player_rigidbody);
        enemyHandler.StartSpawnRandomEnemyEveryPeriod(playerHandler.player_rigidbody);
        //enemyHandler.StartIncrementMaxEnemiesCount(5f);
        roadHandler.StartIncrementRoadTroubleCount();
        //eventHandler.StartCheckDistance(playerHandler.playerStartPosition, playerHandler.player_rigidbody);
        #endregion
    }

    public override void OnFixedTick()
    {
        playerHandler.UpdateCamera();
    }
    public override void OnTick()
    {
        inputHandler.UpdateInputs();
        if (playerHandler.player_transform.gameObject.activeInHierarchy == false)
        {
            enemyHandler.GameOver();
            //playerHandler.player_transform.gameObject.SetActive(true);
        }
    }
   
}
