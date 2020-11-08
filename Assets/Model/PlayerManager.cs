using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;
using System.Linq;

public class PlayerManager {
    private DataService Db = (GameModel.Story != null) ? GameModel.Story.DB : new DataService("ZUp.db"); // Not good because this has already happened in StoryManager???
    private Player _currentPlayer;
    private int _logInAttempts = 0;
    public bool LoggedIn = false;

    public Player CurrentPlayer
    {
        get
        {
            return _currentPlayer;
        }
    }
    public int LoginAttempts
    {
        get
        {
            return _logInAttempts;
        }
    }

    /*
     * Going to Register when ?
     * LogIn fails, ...?
     * Show Register when there are no players?
     */
    public bool PlayerExists(string pUserName)
    {
        return
                (Db.Connection.Table<Player>().Where<Player>(
                          x => x.Name == pUserName
                     ).ToList<Player>().Count > 0);
    }
    public bool RegisterPlayer(string pUserName, string pPassword)
    {
        bool result = false;
        /*
         * Need to check if the player already exists before we register
         * Then Log this player in 
         * if it fails then we don't have a registration
         */
        if ( ! PlayerExists(pUserName)    )
        {

            Player newPlayer = new Player
            {
                Name = pUserName,
                Password = pPassword,
                CurrentScene = GameModel.Story.FirstScene.Id
            };

            Db.Connection.Insert(newPlayer);

            result = LogIn(pUserName, pPassword);
        }
        
        return result;
    }



    /*
    * Log in
    * Set ups currentPlayer if it is sucessfull too
    */
    public bool LogIn(string pUserName, string pPassword)
    {
        List<Player> lcPlayers = Db.Connection.Table<Player>().Where<Player>(
                            x => x.Name == pUserName && x.Password == pPassword
                       ).ToList<Player>();
        
        bool result = lcPlayers.Count > 0 ;
        if (!result)
        {
            _logInAttempts++;

            _currentPlayer = null; // CurrentPlayer
        }
        else
        {
            _logInAttempts = 0;
            _currentPlayer = lcPlayers.First<Player>(); // CurrentPlayer

        }

        LoggedIn = result;

        return result;
    }
    
    public Scene CurrentScene()
    {
        Scene result;

        result = Db.Connection.Table<Scene>().Where(x =>
             x.Id == _currentPlayer.CurrentScene
            ).First<Scene>();

        return result;
    }


    void MoveTo(string pDirection)
    {
        // get the scene where SceneDirection label is pDirection
        int lcCurrentSceneId = _currentPlayer.CurrentScene;
        SceneDirection lcSceneDirection = Db.Connection.Table<SceneDirection>().Where<SceneDirection>(
             x => x.FromSceneId == lcCurrentSceneId & x.Label == pDirection
            ).ToList<SceneDirection>().First<SceneDirection>();

       int lcNextSceneId= lcSceneDirection.ToSceneId;
       Scene lcScene = Db.Connection.Table<Scene>().Where<Scene>(
             x => x.Id == lcNextSceneId
            ).ToList<Scene>().First<Scene>();

        _currentPlayer.CurrentScene = lcScene.Id;

        // Update the Current player
        Db.Connection.InsertOrReplace(_currentPlayer);

    }

    List<Item> GetCurrentSceneItems()
    {
        // Get the SceneItems for the current scene
        List<SceneItem> lcSceneItems = Db.Connection.Table<SceneItem>().Where<SceneItem>(
              x => x.SceneId == _currentPlayer.CurrentScene
             ).ToList<SceneItem>();

        // Use the list of scene items, to select items from the items table
        List<Item> lcItems = Db.Connection.Table<Item>().Where<Item>(
            x =>
                  lcSceneItems.Where<SceneItem>(y => x.ItemId == y.ItemId).ToList<SceneItem>().Count > 0
            ).ToList<Item>();

        return lcItems;
    }
}
