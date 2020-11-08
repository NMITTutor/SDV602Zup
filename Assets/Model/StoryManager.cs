using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;



/*
 * The Story Manager 
 * - makes stories
 * - persists stories
 */
public class StoryManager{
    private Scene _FirstScene;
    private DataService Db = new DataService("ZUp.db");
    public DataService DB
    {
        get
        {
            return Db;
        }
    }

    public Scene FirstScene
    {
        get
        {
            return _FirstScene;
        }
       
    }

    /*
     *  Story Manager
     *  When the story manager is constructed:
     *  1. the Db data service is asked to create the database tables if they do not exist. 
     *  2. ask the Db data service to create the Scenes by inserting(updating) values into the Scene, and SceneDirection, Item and SceneItem tables.
     */
    public StoryManager()
    {
        /*
         * Create database tables if they do not exist.
         * - a call to create a db from a list of System.Type - 
         * - the C# "typeof" operator returns a System.Type value.
         * 
         */
        Db.CreateDB(new[] {
            typeof(Story),
            typeof(Scene),
            typeof(Item),
            typeof(SceneItem),
            typeof(SceneDirection),
            typeof(Player),
            typeof(InventoryItem),

       });

        if (Db.StoryCount() == 0)
        {
            MakeStory("ZUp", "Ooooer");
        }
        else
            GetStory("ZUp");

    }
    /*
     * GetStory
     * Gets the Story record and sets the Story for the GameModel
     */
    public void GetStory(string pStoryName)
    {
        GetStoryFirstScene(pStoryName);
    }

    /*
     * Make Story
     * Adds a Story record for a named story
     * Adds Scenes and Items if they do not exist.
     */
    public void MakeStory(string pStoryName, string pStoryDescription){

        Db.StoreIfNotExists<Story>(new Story
        {
            StoryName = pStoryName,
            Description = pStoryDescription
        });

        /*
         * Get the Story Back so we can update the FirstScene when we have it, using Linq
         * 
         * Build a "where lambda", execute with ToList, then get the first one from the list
         */
        Story theStory = Db.Connection.Table<Story>().Where(
                   x => x.StoryName == pStoryName
                   ).ToList<Story>().FirstOrDefault<Story>();

        /*
         *  The Scenes that make up the Story
         *  This Insert returns the Autoincrement ID
         */

        // Forest
        var lcForest = new Scene
        {
            StoryName = theStory.StoryName,
            Description = "You are lost in the Forest"
        };
        Db.Connection.Insert(lcForest);
        int lcForestID = lcForest.Id;

        // Now we have our first Scene for the story
        theStory.FirstSceneID = lcForestID;
        Db.Connection.InsertOrReplace(theStory);


        // Mall
        var lcMall = new Scene
        {
            StoryName = theStory.StoryName,
            Description = "You are at the Mall"
        };
        Db.Connection.Insert(lcMall);
        int lcMallID = lcMall.Id;

        // Mall Items, wallet, key, gold. Inserts a record for each into the Item table.
        Item[] lcItems = {
                 new Item
                 {
                    Description = "A Wallet"
                },
                new Item
                {
                    Description = "A key"
                },
                new Item
                {
                    Description = "Gold"
                }
            };
        // Insert Items, they will now get their ID back
        lcItems.Where(x =>
        {
            Db.Connection.Insert(x);

            // Insert SceneItems too
            Db.Connection.Insert(new SceneItem
            {
                SceneId = lcMallID,
                ItemId = x.ItemId
            });
            return true;
        }).ToList<Item>();



        // Cliff
        Scene lcCliff = new Scene
        {
            StoryName = theStory.StoryName,
            Description = "You fell off a cliff"
        };
        Db.Connection.Insert(lcCliff);
        int lcCliffID = lcCliff.Id;

        /* 
         * Now the SceneDirections
         * 
         * Forest, NORTH to Mall
         * Mall, NORTH to Cliff
         * Mall, SOUTH to Forest
         * Cliff, WEST to Forest
         */
        SceneDirection[] lcDirections = {
            new SceneDirection {FromSceneId = lcForestID, ToSceneId = lcMallID, Label = "NORTH"},
            new SceneDirection {FromSceneId = lcMallID, ToSceneId = lcCliffID, Label = "NORTH"},
            new SceneDirection {FromSceneId = lcMallID, ToSceneId = lcForestID, Label = "SOUTH"},
            new SceneDirection {FromSceneId = lcCliffID, ToSceneId = lcForestID, Label = "WEST"}

        };

        Db.Connection.InsertAll(lcDirections); // << CHECK THIS WORKS??





    }

    public void GetStoryFirstScene(string pStoryName)
    {
        var theStory = Db.Connection.Table<Story>().Where<Story>(
             x => x.StoryName == pStoryName
            ).ToList<Story>().First<Story>();

        _FirstScene = Db.Connection.Table<Scene>().Where<Scene>(
            x => x.Id == theStory.FirstSceneID
            ).ToList<Scene>().First<Scene>();
    } 

    public void SaveStory()
    {
       
    }

    public void LoadStory()
    {

    }
	
}
