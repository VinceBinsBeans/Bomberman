﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
namespace ImprovisedTextField
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D textFieldTex;
        Texture2D explosionTex;

        Texture2D ExplosionY;
        Texture2D ExplosionX;

        Texture2D explosionSegment;

        SpriteFont spriteFont;

        Player player;

        List <Blocks> wallBlock;
        List <Blocks> hardBlocks;
        List <Blocks> softBlocks;

        List<Bomb> bombList;
        List<Explosion> explosionList;

        List<Explosion> explosionXList;
        List<Explosion> explosionYList;

        bool playerKilled;
        bool gameEnd;

        bool inGameSave;

        int timer;

        List<Blocks> spaceChecker;
        List<Point> availableSpace;
        List<Point> collidingPositions;
        List<Point> NonCollidingPositions;

        List<ScoreTracker> scores;
        List<BinarySaveClass> binaryScores;

        List<enemyList> enemies;

        Blocks exitBlock;
        Blocks FireBlock;
        Blocks DoubleBlock;

        Button OkayBtn;
        Button ClearBtn;

        Button saveBtn;
        Button loadBtn;

        Button HiScoreBtn;
        Button StartBtn;

        Button titleCard;

        int[] spaceIndeces;
        int deathDelay;

        bool checkPress;
    
        bool showSaved;
        bool showWarning;

        bool layingBombs;

        bool isPlaying;

        bool isHighScore;
        bool isMainMenu;
        bool isStart;
        bool isPaused;
        bool isSaving;
        bool isLoading;

        bool collided;

        string name;
        string fileName;
        string saveName="";
        int score;

        int exitIndex;
        bool isPressingEsc;
        int rangeBombIndex;
        int doubleBombIndex;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            deathDelay = 0;
            
        }

        protected override void Initialize()
        {
            Texture2D OkBtn = Content.Load<Texture2D>("OKBtn");
            Texture2D CancelBtn = Content.Load<Texture2D>("ClearBtn");

            Texture2D HiBtn = Content.Load<Texture2D>("HighScoreBtn");
            Texture2D startBtn = Content.Load<Texture2D>("StartBtn");

            Texture2D titleTex = Content.Load<Texture2D>("Title");

            Texture2D HardBlockTex = Content.Load<Texture2D>("HardBlocks");
            Texture2D SoftBlockTex = Content.Load<Texture2D>("SoftBlocks");

            Texture2D playerTex = Content.Load<Texture2D>("PlayerWalkCycle");

            Texture2D saveTex = Content.Load<Texture2D>("saveBtn");
            Texture2D loadTex = Content.Load<Texture2D>("loadBtn");

            explosionTex = Content.Load<Texture2D>("explosionSheet");

            ExplosionX = Content.Load<Texture2D>("XPlosion");
            ExplosionY = Content.Load<Texture2D>("YPlosion");

            explosionSegment = Content.Load<Texture2D>("explosionSegment");

            OkayBtn = new Button(new Rectangle(200, Window.ClientBounds.Height / 2 + 10, 100, 50),OkBtn,Color.White);
            ClearBtn = new Button(new Rectangle(400, Window.ClientBounds.Height / 2 + 10, 100, 50), CancelBtn, Color.White);

            titleCard = new Button(new Rectangle(200, 20, 400,150), titleTex, Color.White);
            StartBtn = new Button(new Rectangle(300, 200, 200, 100), startBtn, Color.White);
            HiScoreBtn = new Button(new Rectangle(300, 400, 200, 100), HiBtn, Color.White);

            saveBtn = new Button(new Rectangle(300, 150, 200, 100), saveTex, Color.White);
            loadBtn = new Button(new Rectangle(300, 300, 200, 100), loadTex, Color.White);

            player = new Player(new Rectangle(40,40,30,30),playerTex,Color.White);

            wallBlock = new List<Blocks>();
            hardBlocks = new List<Blocks>();
            softBlocks = new List<Blocks>();

            spaceChecker = new List<Blocks>();

            bombList = new List<Bomb>();
            explosionList = new List<Explosion>();

            explosionXList = new List<Explosion>();
            explosionYList = new List<Explosion>();

            availableSpace = new List<Point>();
            enemies = new List<enemyList>();

            scores = new List<ScoreTracker>();
            binaryScores = new List<BinarySaveClass>();

            int RightPosition = 0;
            int BottomPosition = 0;


            #region surroundingWall
            for (int TopX = 0; TopX < 17; TopX++) // generate top region of the the wall
            {
                RightPosition = 40 * TopX; //the last index's point value will be used for the bottom portion
                Blocks b = new Blocks(new Rectangle(40*TopX,0,40,40),HardBlockTex,Color.White); //multiply the X value with the index in order to arrange each top block
                wallBlock.Add(b); //add temp value to list
            }

            for(int LeftY = 0; LeftY<11; LeftY++) // generation the left portion of the wall
            {
                BottomPosition = 40 * LeftY; //last index's point value will be used for the right portion of the wall
                Blocks b = new Blocks(new Rectangle(0, 40*LeftY, 40, 40), HardBlockTex, Color.White); //multiply the Y value with the index in order to arrange each left block
                wallBlock.Add(b);
            }

            for(int BottomX = 0; BottomX < 17; BottomX++) //generate bottom side of the wall
            {
                Blocks b =new Blocks(new Rectangle(40 * BottomX, BottomPosition, 40, 40), HardBlockTex, Color.White);
                wallBlock.Add(b);
            }

            for(int RightY = 0; RightY<11; RightY++) //generate right side of the wall
            {
                Blocks b = new Blocks(new Rectangle(RightPosition, 40 * RightY, 40, 40), HardBlockTex, Color.White);
                wallBlock.Add(b);
            }
            #endregion
            #region HardBlocksInField
            for(int x = 0; x < 7; x++) //generate hard/non-destroyable blocks in the playing field
            {

                for(int y = 0; y < 5; y++)
                {
                    Blocks Block = new Blocks(new Rectangle(80 * (x + 1), 80*y, 40, 40), HardBlockTex, Color.White);
                    hardBlocks.Add(Block);
                }

            }
            #endregion
                
            #region softblockGeneration
            for (int X = 0; X < 7; X++)
            {
                for (int Y = 0; Y < 4; Y++)
                {
                    Blocks block = new Blocks(new Rectangle(40 * (X + 1), 40 * (Y + 1), 40, 40), SoftBlockTex, Color.White); //generate a pool of 28 blocks to be placed in random locations later on 
                    softBlocks.Add(block);
                }
            }

            for (int x = 0; x < 15; x++)
            {
                for(int y = 0; y < 9; y++)
                {
                    Blocks b = new Blocks(new Rectangle(40*(x+1),40*(y+1),40,40),SoftBlockTex,Color.White); //generate 135 blocks/rectangles to check where the hard blocks are. 
                    spaceChecker.Add(b);                                            //this will be used to determine where we can place objects
                }
            }
            //create a grid for the playing field
            for(int x = 0; x < 15; x++)
            {
                for(int y = 0; y< 9; y++)
                {
                    for(int z = 0; z < spaceChecker.Count-1; z++) 
                    {
                        for(int w = 0; w < hardBlocks.Count; w++) //check how many non destructive blocks there are
                        {
                            if (spaceChecker[z].BlockRect.Intersects(hardBlocks[w].BlockRect)) //if the populated grid hits a hard block
                            {
                                spaceChecker.Remove(spaceChecker[z]); //delete the index that hit a block.
                            }                                          
                        }
                    }
                }
            }
            Random randPos = new Random();
            spaceIndeces = new int[spaceChecker.Count - 1]; //indeces with available space

            for (int j = 0; j < spaceChecker.Count - 1; j++)
            {
                Point space = spaceChecker[j].BlockRect.Location;  //assign the spacechecker to a temporary point
                availableSpace.Add(space);           //since spacechecker does not have any positions intersecting with a hard block
            }                                   //we will assign it to a Point called availableSpace for easier readability

            for (int i = 0; i < spaceIndeces.Length; i++)
            {
                spaceIndeces[i] = randPos.Next(i, spaceChecker.Count - 1); //randomize the indeces of the availableSpace  list
            }

            //below will be the randomization of destructible blocks
           
            Blocks tempBlock;
            int[] randomCollection;
            randomCollection = new int[softBlocks.Count - 1];

            tempBlock = new Blocks(new Rectangle(80, 40, 40, 40), SoftBlockTex, Color.White); //temporary block

            for (int i = 0; i < softBlocks.Count - 1; i++)
            {
                softBlocks[i].Position = availableSpace[spaceIndeces[i]]; //assign the available random position to the soft block/destructible block list
                availableSpace[spaceIndeces[i]] = tempBlock.Position; //assign the temporary block to a random position
                tempBlock.Position = softBlocks[i].Position; //temporary block will be equal to the new soft block position
            }
            //there are some positions wherein the soft blocks are on top of each other
            //to lessen this, we will check which positions have colliding soft blocks

            collidingPositions = new List<Point>();
            NonCollidingPositions = new List<Point>();
            
            foreach (Blocks s in softBlocks)
            {
                foreach (Blocks ss in softBlocks) //iterate through the softblocks
                {
                    if (s.BlockRect.Intersects(ss.BlockRect)) //check if a soft block is colliding with another soft block
                    {
                        if (s != ss) //check to see if the softblock is not colliding with itself
                        {
                            Point p  = s.Position; 
                            collidingPositions.Add(p); //assign the position of the softblocks colliding with each other to a list
                        } //the list will be later used to determine
                    }
                }
            }
            availableSpace.Clear(); //clear the available space variable in order to be reused in
                                    //repositioning the colliding blocks

            //use space checker variable again since this covers the entire playing field
            for(int i = 0; i < spaceChecker.Count-1; i++)
            {
                for(int j = 0; j < collidingPositions.Count-1; j++)
                {
                    if(spaceChecker[i].Position == collidingPositions[j]) 
                    {
                        Console.WriteLine("W3W"); //debugging how many times this shows up to know how many objects are still colliding
                    }else//if spacechecker isnt colliding with collidingPositions
                    {
                        Point p = spaceChecker[i].Position;  //assign the available positions to p
                        availableSpace.Add(p); //then add to the list of availableSpace
                    }
                }
            }
            randPos = new Random();
            int[] spaceIndex; 
            spaceIndex = new int[availableSpace.Count]; //create another list to contain the index of available spaces

           for(int i = 0; i < availableSpace.Count; i++)
            {
                spaceIndex[i]=randPos.Next(i,availableSpace.Count-1); //randomize the order of available space index
                
            }

           //finally, we will move the colliding soft blocks
            for(int i = 0; i < softBlocks.Count-1; i++)
            {
                for(int j = 0; j < collidingPositions.Count - 1; j++) 
                {
                    if(softBlocks[i].Position == collidingPositions[j]) //check if a soft block's position is colliding with one position
                    {                                                   //alam mo naman dapat may mag adjust.
                        softBlocks[i].Position = availableSpace[spaceIndex[j]]; //move one of the colliding blocks to an available space
                    }
                }
            }
            //tempBlock = new Blocks(new Rectangle(80, 40, 40, 40), SoftBlockTex, Color.White); //temporary block
            Random randExit = new Random();

            Texture2D exitTexture = Content.Load<Texture2D>("ExitTex");
            Texture2D multiTexture = Content.Load<Texture2D>("MultiBomb");
            Texture2D rangeTexture = Content.Load<Texture2D>("LifeTex");

            exitIndex = randExit.Next(0, softBlocks.Count);
            doubleBombIndex = randExit.Next(exitIndex, softBlocks.Count);
            rangeBombIndex = randExit.Next(doubleBombIndex, softBlocks.Count);

            exitBlock = new Blocks(new Rectangle(0, 0, 30, 30), exitTexture, Color.White);
            DoubleBlock = new Blocks(new Rectangle(0, 0, 30, 30), multiTexture, Color.White);
            FireBlock = new Blocks(new Rectangle(0, 0, 30, 30), rangeTexture, Color.White);

            if (exitIndex == doubleBombIndex)
            {
                exitIndex = randExit.Next(0, softBlocks.Count);
                doubleBombIndex = randExit.Next(0, rangeBombIndex);
            }

            if (doubleBombIndex == rangeBombIndex)
            {
                rangeBombIndex = randExit.Next(exitIndex, doubleBombIndex);
            }

            if (exitIndex == rangeBombIndex)
            {
                exitIndex = randExit.Next(0, rangeBombIndex);
            }

            exitBlock.Position = softBlocks[exitIndex].Position;
            DoubleBlock.Position = softBlocks[doubleBombIndex].Position;
            FireBlock.Position = softBlocks[rangeBombIndex].Position;

            exitBlock.BlockRect = new Rectangle(exitBlock.Position.X+5,exitBlock.Position.Y + 5, 30,30);
            DoubleBlock.BlockRect = new Rectangle(DoubleBlock.Position.X + 5, DoubleBlock.Position.Y + 5, 30, 30);
            FireBlock.BlockRect = new Rectangle(FireBlock.Position.X + 5, FireBlock.Position.Y + 5, 30, 30);

            Console.WriteLine("This Index has the exit:" + exitIndex + " soft block exit:" + softBlocks[exitIndex].Position);
            Console.WriteLine("This Index has double bomb:" + doubleBombIndex + " soft block exit:" + softBlocks[doubleBombIndex].Position);
            Console.WriteLine("This Index has range bomb:" + rangeBombIndex + " soft block exit:" + softBlocks[rangeBombIndex].Position);

            //spaceChecker.Clear();
            #endregion

            #region enemyGeneration

            //generation of enemies

            //create a list of available space for the enemy
            Point[] availableEnemySpace;
            //initialize 5 spaces for the enemy
            availableEnemySpace = new Point[5];
            Texture2D enemyTex = Content.Load<Texture2D>("enemyMove");
          //  Console.WriteLine("BEFORE REMOVAL:"+spaceChecker.Count);
            for (int i = 0; i < 6; i++)
            {
                enemyList e = new enemyList(new Rectangle(40, 160, 27, 27), enemyTex, Color.White); //create a list of enemies
                enemies.Add(e);
            }

            spaceIndex = new int[spaceChecker.Count]; //create a list of available indeces

            for(int i = 0; i < spaceChecker.Count-1; i++) {
                        
                for(int j = 0; j < softBlocks.Count - 1; j++) //iterate through soft blocks
                {
                    if (spaceChecker[i].BlockRect.Intersects(softBlocks[j].BlockRect)) //if the space checker hits a soft block
                    {
                        spaceChecker.Remove(spaceChecker[i]);  //remove the space checker
                                                                //so that enemies will not be placed here
                    }
                }
            }

            spaceIndex = new int[spaceChecker.Count-1]; //create an enemy position index randomizer

            for(int i = 0; i < spaceIndex.Length; i++)
            {
                spaceIndex[i] = randPos.Next(i,spaceIndex.Length-1); //randomize the indeces
            }
            //space index contains the indeces with available space
            //we will assign the index to an array to determine which position will be assigned to an enemy 

            //below is the assignment of positions to enemies
            for (int i = 0; i < spaceIndex.Length; i++)
            {
                int previousIndex = 0;  //used to check what the previous index value was

                while (previousIndex == spaceIndex[i]) //while the previous index is equal to the current space index
                {                                      
                    if(previousIndex != spaceIndex[i]) 
                    {
                        break; //break out of the while loop once the we find a value that is not equal to the current index
                    }
                    //if the number is still equal to the current index
                    if (spaceIndex.Length-1 > i++) 
                        spaceIndex[i] = spaceIndex[i++]; // assign the next index to the current index
                                            //since this is the end of the loop, the current index will be the previous index
                                            //this will be used to check duplicate indeces
                                             //to prevent enemies spawning on top of each other
                }
                
                    previousIndex = spaceIndex[i];
            }

            for (int i = 0; i < 6; i++)
            {
                enemies[i].Position = new Point(spaceChecker[spaceIndex[i]].Position.X + (enemies[i].enemyRect.Width / 4), spaceChecker[spaceIndex[i]].Position.Y + (enemies[i].enemyRect.Height / 4)); //assign a random position to the list of enemies
            }

            #endregion

            // load();
            binaryLoadScore();
           
            name = "";
            fileName = "";
            score = 0;

            

            isMainMenu = true;
            checkPress = true;

            base.Initialize();
        }
        protected override void LoadContent()
        { 
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>(@"sf");
            textFieldTex = Content.Load<Texture2D>(@"Inputfield");

        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
           
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                if (isStart || isLoading)
                {
                    isMainMenu = true;
                    isStart = false;
                    isLoading = false;
                }

                if (isPlaying && !isPressingEsc)
                {
                    inGameSave = true;
                    isPlaying = false;
                }
                else
                if(inGameSave && !isPressingEsc)
                {
                    isPlaying = true;
                    inGameSave = false;
                }
                isPressingEsc = true;
            }
            else
            {
                isPressingEsc = false;
            }
            if (inGameSave)
                {
                Rectangle mouseIn = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 50, 50);
                Texture2D OkBtn = Content.Load<Texture2D>("OKBtn");
                    OkayBtn = new Button(new Rectangle(200, Window.ClientBounds.Height / 2 + 10, 100, 50), OkBtn, Color.White);

                    Keys[] d = Keyboard.GetState().GetPressedKeys();

                    foreach (Keys k in d)
                    {

                        if (checkPress)
                        {
                            try
                            {
                                char charfileName = char.Parse(k.ToString());

                                if (char.IsLetter(charfileName) && saveName.Length <= 10)
                                {
                                    saveName += k.ToString();
                                }
                            }
                            catch { }

                            if (k == Keys.Back && saveName.Length > 0)
                            {
                                saveName = saveName.Substring(0, saveName.Length - 1);
                            }

                            if (k == Keys.Space)
                            {
                                saveName += " ";
                            }

                            if (k == Keys.OemMinus)
                            {
                                saveName += "_";
                            }
                        }
                    }

                    if (d.Length > 0)
                    {
                        checkPress = false;
                    }
                    else
                    {
                        checkPress = true;
                    }

                    if (OkayBtn.btnRect.Intersects(mouseIn))
                    {
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                        {
                            if (saveName.Length > 0)
                            {
                                showSaved = true;
                                showWarning = false;

                                isLoading = false;
                                isStart = false;
                             // xmlSave(saveName);
                            binarySave(saveName);
                        }
                            else
                            {
                                showSaved = false;
                                showWarning = true;
                            }
                        }
                    }

                    if (ClearBtn.btnRect.Intersects(mouseIn))
                    {
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                        {
                            saveName = "";

                            showSaved = false;
                            showWarning = false;

                            isMainMenu = true;
                            isPlaying = false;
                            isSaving = false;
                            inGameSave = false;
                            isLoading = false;

                            Reset();
                            name = "";
                            score = 0;
                            player.lives = 3;
                        }
                    }
                }
            #region Menu
            if (!isPlaying && !inGameSave)
            {
                Rectangle mouseIn = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 50, 50);
                gameEnd = false;

                if (isMainMenu)
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        if (mouseIn.Intersects(StartBtn.btnRect))
                        {
                            isMainMenu = false;
                            isHighScore = false;
                            isStart = true;
                        }

                        if (mouseIn.Intersects(HiScoreBtn.btnRect))
                        {
                            isMainMenu = false;
                            isHighScore = true;
                        }

                        if (mouseIn.Intersects(loadBtn.btnRect))
                        {
                            isMainMenu = false;
                            isHighScore = false;
                            isLoading = true;
                        }
                    }
                }

                if (isHighScore)
                {
                    Texture2D OkBtn = Content.Load<Texture2D>("OKBtn");
                    OkayBtn = new Button(new Rectangle(Window.ClientBounds.Width/2 - 100, Window.ClientBounds.Height - 50, 100, 50), OkBtn, Color.White);

                    if (mouseIn.Intersects(OkayBtn.btnRect))
                    {
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                        {
                            isHighScore = false;
                            isMainMenu = true;
                        }
                    }
                }
                
                if (isLoading)
                {
                    Texture2D OkBtn = Content.Load<Texture2D>("OKBtn");
                    OkayBtn = new Button(new Rectangle(200, Window.ClientBounds.Height / 2 + 10, 100, 50), OkBtn, Color.White);

                    Keys[] d = Keyboard.GetState().GetPressedKeys();

                    foreach (Keys k in d)
                    {
                        if (checkPress)
                        {
                            try
                            {
                                char charfileName = char.Parse(k.ToString());

                                if (char.IsLetter(charfileName) && fileName.Length <= 10)
                                {
                                    fileName += k.ToString();
                                }
                            }
                            catch { }

                            if (k == Keys.Back && fileName.Length > 0)
                            {
                                fileName = fileName.Substring(0, fileName.Length - 1);
                            }

                            if (k == Keys.Space)
                            {
                                fileName += " ";
                            }

                            if (k == Keys.OemMinus)
                            {
                                fileName += "_";
                            }
                        }
                    }

                    if (d.Length > 0)
                    {
                        checkPress = false;
                    }
                    else
                    {
                        checkPress = true;
                    }

                    if (OkayBtn.btnRect.Intersects(mouseIn))
                    {
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                        {
                            if (fileName.Length > 0)
                            {
                                showSaved = true;
                                showWarning = false;

                                isPlaying = true;
                                isLoading = false;
                                isStart = false;
                                //xmlLoad(fileName);
                                binaryLoad(fileName);
                            }
                            else
                            {
                                showSaved = false;
                                showWarning = true;
                            }
                        }
                    }

                    if (ClearBtn.btnRect.Intersects(mouseIn))
                    {
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                        {
                            fileName = "";

                            showSaved = false;
                            showWarning = false;
                        }
                    }

                }
                if (isStart)
                {
                    Texture2D OkBtn = Content.Load<Texture2D>("OKBtn");
                    OkayBtn = new Button(new Rectangle(200, Window.ClientBounds.Height / 2 + 10, 100, 50), OkBtn, Color.White);

                    Keys[] d = Keyboard.GetState().GetPressedKeys();

                    foreach (Keys k in d)
                    {
                        if (checkPress)
                        {
                            try
                            {
                                char charname = char.Parse(k.ToString());

                                if (char.IsLetter(charname) && name.Length <= 10)
                                {
                                    name += k.ToString();
                                }
                            }
                            catch { }

                            if (k == Keys.Back && name.Length > 0)
                            {
                                name = name.Substring(0, name.Length - 1);
                            }

                            if (k == Keys.Space)
                            {
                                name += " ";
                            }

                            if (k == Keys.OemMinus)
                            {
                                name += "_";
                            }

                        }
                    }

                    if (d.Length > 0)
                    {
                        checkPress = false;
                    }
                    else
                    {
                        checkPress = true;
                    }

                    if (OkayBtn.btnRect.Intersects(mouseIn))
                    {
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                        {
                            if (name.Length > 0)
                            {
                                showSaved = true;
                                showWarning = false;

                                isPlaying = true;
                                isStart = false;
                            }
                            else
                            {
                                showSaved = false;
                                showWarning = true;
                            }
                        }
                    }

                    if (ClearBtn.btnRect.Intersects(mouseIn))
                    {
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                        {
                            name = "";

                            showSaved = false;
                            showWarning = false;
                        }
                    }
                }
            }
            #endregion
            #region gameplay
            if (isPlaying && !inGameSave)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    Reset();
                }

                if (Keyboard.GetState().IsKeyDown(Keys.K) && !isSaving)
                {
                    //  xmlSave(fileName);
                    binarySave(fileName);
                    isSaving = true;
                }else
                if ((Keyboard.GetState().IsKeyUp(Keys.K))){
                    isSaving = false;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.L) && !isLoading)
                {
                    //xmlLoad(fileName);
                    binaryLoad(fileName);
                    isLoading = true;
                } else
                    if ((Keyboard.GetState().IsKeyUp(Keys.L)))
                {
                    isLoading = false;
                }


                player.Move();
                player.playerAnimation();

                // Console.WriteLine("Current Position:"+player.Position+"snap to:"+new Point(Math.Abs(player.Position.X - (player.Position.X % 40)), Math.Abs(player.Position.Y - (player.Position.Y % 40))));
                
               // if(Math.Abs(player.Position.X - (player.Position.X % 40)), Math.Abs(player.Position.Y - (player.Position.Y % 40)))
                if (Keyboard.GetState().IsKeyDown(Keys.Space) && !layingBombs)
                {
                    if (player.hasMulti)
                    {
                        if (bombList.Count < 2)
                        {
                            Texture2D bombTex = Content.Load<Texture2D>(@"bombSheet");
                            Bomb b = new Bomb(new Rectangle(Math.Abs((player.Position.X) - (player.Position.X % 40)) +4, Math.Abs(player.Position.Y - (player.Position.Y % 40))+5, 35, 35), bombTex, Color.White);
                            bombList.Add(b);
                        }
                    }else
                    {
                        if (bombList.Count < 1)
                        {
                            Texture2D bombTex = Content.Load<Texture2D>(@"bombSheet");
                            Bomb b = new Bomb(new Rectangle(Math.Abs((player.Position.X) - (player.Position.X % 40)) +4,Math.Abs(player.Position.Y - (player.Position.Y % 40))+5, 35, 35), bombTex, Color.White);
                            bombList.Add(b);
                        }
                    }
                    layingBombs = true;
                }
                else
                if (Keyboard.GetState().IsKeyUp(Keys.Space))
                {
                    layingBombs = false;
                }

                foreach (Bomb b in bombList)
                {
                    b.Time++;

                    if (b.Time > 120)
                    {
                        Explosion CenterSegment = new Explosion(new Rectangle((b.Position.X + 5) - 5, (b.Position.Y + 5) - 5, 30, 30), explosionSegment, Color.White);
                        CenterSegment.isCenter = true;
                        explosionYList.Add(CenterSegment);

                        Explosion FirstRightSegment = new Explosion(new Rectangle((b.Position.X + 5) + 25, (b.Position.Y + 5) - 5, 30, 30), explosionSegment, Color.White);
                        FirstRightSegment.firstRight = true;
                        explosionXList.Add(FirstRightSegment);
                        Explosion FirstLeftSegment = new Explosion(new Rectangle((b.Position.X - 5) - 25, (b.Position.Y + 5) - 5, 30, 30), explosionSegment, Color.White);
                        FirstLeftSegment.firstLeft = true;
                        explosionXList.Add(FirstLeftSegment);

                        Explosion FirstUpSegment = new Explosion(new Rectangle((b.Position.X + 5) - 5, (b.Position.Y - 5) - 25, 30, 30), explosionSegment, Color.White);
                        FirstUpSegment.firstUp = true;
                        explosionYList.Add(FirstUpSegment);

                        Explosion FirstDownSegment = new Explosion(new Rectangle((b.Position.X + 5) - 5, (b.Position.Y + 5) + 25, 30, 30), explosionSegment, Color.White);
                        FirstDownSegment.firstDown = true;
                        explosionYList.Add(FirstDownSegment);

                        bombList.Remove(b);
                        break;
                    }
                }

                foreach (Blocks b in hardBlocks)
                {
                    player.Collisions(b.BlockRect);
                    foreach (enemyList e in enemies)
                    {
                        e.Collisions(b);
                    }

                    foreach (Explosion e in explosionXList)
                    {
                        if (b.BlockRect.Intersects(e.ExplosionRect))
                        {
                            explosionXList.Remove(e);
                            break;
                        }
                    }

                    foreach (Explosion e in explosionYList)
                    {
                        if (b.BlockRect.Intersects(e.ExplosionRect))
                        {
                            explosionYList.Remove(e);
                            break;
                        }
                    }
                }
                overHere:
                foreach (Explosion e in explosionYList)
                {
                    if (e.ExplosionRect.Intersects(player.PlayerRect))
                    {
                        playerKilled = true;
                        explosionYList.Remove(e);
                        break;
                    }
                    foreach (Blocks s in softBlocks)
                    {
                        if (e.ExplosionRect.Intersects(s.BlockRect))
                        {
                            if(!e.isCenter)
                                softBlocks.Remove(s);
                            explosionYList.Remove(e);
                            e.hasKilled = true;
                            goto overHere;
                        }
                    }

                    foreach(enemyList en in enemies)
                    {
                        if (en.enemyRect.Intersects(e.ExplosionRect) && !e.ExplosionRect.Intersects(exitBlock.BlockRect))
                        {
                            enemies.Remove(en);
                            score += 100;
                            break;
                        }
                    }
                    e.Animation();
                    if (e.cleanUp())
                    {
                        explosionYList.Remove(e);
                        break;
                    }
                }
                
                    foreach (Explosion e in explosionXList)
                    {
                    if (e.ExplosionRect.Intersects(player.PlayerRect))
                    {
                        playerKilled = true;
                        explosionXList.Remove(e);
                        break;
                    }
                    foreach (Blocks s in softBlocks)
                            {
                                if (e.ExplosionRect.Intersects(s.BlockRect))
                                {
                                    softBlocks.Remove(s);
                                    explosionXList.Remove(e);
                                    goto somewhere;

                                }
                            }
                    foreach (enemyList en in enemies)
                    {
                        if (en.enemyRect.Intersects(e.ExplosionRect) && !e.ExplosionRect.Intersects(exitBlock.BlockRect))
                        {
                            enemies.Remove(en);
                            score += 100;
                            break;
                        }
                    }

                    e.Animation();
                        if (e.cleanUp())
                        {
                            explosionXList.Remove(e);
                            break;
                        }
                }

                somewhere:
                foreach (Bomb bm in bombList)
                {
                    player.Collisions(bm.BombRect);

                    foreach(enemyList e in enemies)
                    {
                        e.bombCollide(bm);
                    }

                    foreach (Blocks b in wallBlock)
                    {
                        if (bm.BombRect.Intersects(b.BlockRect))
                        {
                            bombList.Remove(bm);
                            goto notHere;
                        }
                    }

                    foreach(Blocks b in hardBlocks)
                    {
                        if (bm.BombRect.Intersects(b.BlockRect))
                        {
                            bombList.Remove(bm);
                            goto notHere;
                        }
                    }

                    foreach(Blocks b in softBlocks)
                    {
                        if (bm.BombRect.Intersects(b.BlockRect))
                        {
                            bombList.Remove(bm);
                            goto notHere;
                        }
                    }
                }

                notHere:
                foreach (Blocks b in softBlocks)
                {
                    player.Collisions(b.BlockRect);

                    foreach (enemyList e in enemies)
                    {
                        if (!collided)
                        {
                           
                            if (e.enemyRect.Intersects(b.BlockRect))
                            {
                                collided = true;
                                Console.WriteLine("Hit something");
                                e.Left = true;
                                break;
                            }
                        }
                        e.Collisions(b);
                    }
                    
                    if (player.PlayerRect.Intersects(FireBlock.BlockRect) && !FireBlock.hasPower && !b.BlockRect.Intersects(FireBlock.BlockRect))
                    {
                        FireBlock.hasPower = true;
                        player.lives++;
                        player.hasBombRange = true;
                    }

                    if (player.PlayerRect.Intersects(DoubleBlock.BlockRect) && !DoubleBlock.hasPower && !b.BlockRect.Intersects(DoubleBlock.BlockRect))
                    {
                        DoubleBlock.hasPower = true;
                        player.hasMulti = true;
                    }

                    foreach(Explosion e in explosionXList)
                    {
                        if (e.ExplosionRect.Intersects(exitBlock.BlockRect) && !exitBlock.hasPower)
                        {
      
                            exitBlock.hasPower = true;
                        }
                    }
                    foreach (Explosion e in explosionYList)
                    {
                        if (e.ExplosionRect.Intersects(exitBlock.BlockRect)&& !exitBlock.hasPower)
                        {    
                            exitBlock.hasPower = true;
                        }
                    }
                }

                if (exitBlock.hasPower)
                {
                    if (enemies.Count != 10) { 
                        Texture2D enemyTex = Content.Load<Texture2D>("enemyMove");
                        enemyList en = new enemyList(new Rectangle(exitBlock.Position.X, exitBlock.Position.Y, 27, 27), enemyTex, Color.White); //create a list of enemies

                        enemies.Add(en);
                    }
                }
                foreach (Blocks b in wallBlock)
                {
                    player.Collisions(b.BlockRect);

                    foreach (enemyList e in enemies)
                    {
                        e.Collisions(b);
                    }
                }
                foreach(enemyList e in enemies)
                {
                   e.move();

                    if (e.enemyRect.Intersects(player.PlayerRect))
                    {
 
                        playerKilled = true;
                    }
                }

                foreach(Blocks b in spaceChecker)
                {
                    foreach (enemyList e in enemies)
                    {
                        if(Math.Abs(b.BlockRect.X - e.enemyRect.X-(e.enemyRect.Width/4)) <= 10)
                        {
                            e.Up = true;
                        }
                    }
                }
              
                if(playerKilled && player.lives > 0 && !gameEnd)
                {
                    Reset();
                    playerKilled = false;
                    player.lives--;

                    player.hasBombRange = false;
                    player.hasMulti = false;
                    exitBlock.hasPower = false;
                }
                if (gameEnd)
                {
                    Reset();
                    
                }

                if (player.lives == 0 || (player.PlayerRect.Intersects(exitBlock.BlockRect) && enemies.Count == 0))
                {
                    playerKilled = false;
                    

                        gameEnd = true;
                        save();
                        binarySaveScore();
                        player.lives = 3;

                        player.hasBombRange = false;
                        player.hasMulti = false;

                        isPlaying = false;
                        isMainMenu = true;
                        exitBlock.hasPower = false;

                        score = 0;
                        name = "";

                        Console.WriteLine("SAVED");

                        deathDelay = 0;
                   
                }
            }
           foreach(Blocks s in softBlocks)
            {
                foreach(Blocks ss in softBlocks)
                {
                    if (s != ss)
                    {
                        if (s.BlockRect.Intersects(ss.BlockRect))
                        {
                            softBlocks.Remove(ss);
                            goto fey;
                        }
                    }
                }
            }
           fey:
            #endregion
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            #region drawMenu
            if (!isPlaying)
            {
                if (isStart||isLoading||inGameSave)
                {
                    spriteBatch.Draw(textFieldTex, new Rectangle(0, Window.ClientBounds.Height / 2 - 100, 800, 50), Color.White);
                    spriteBatch.Draw(OkayBtn.btnTex, OkayBtn.btnRect, OkayBtn.btnColor);
                    spriteBatch.Draw(ClearBtn.btnTex, ClearBtn.btnRect, ClearBtn.btnColor);

                    if(isStart)
                        spriteBatch.DrawString(spriteFont, name, new Vector2(20, ((Window.ClientBounds.Height / 2) - 100) + 10), Color.Blue);

                    if (isLoading)
                        spriteBatch.DrawString(spriteFont, "LOAD File Name", new Vector2(20, ((Window.ClientBounds.Height / 2) - 200) + 10), Color.Blue);
                    if (inGameSave)
                    {
                        spriteBatch.DrawString(spriteFont, "SAVE File Name", new Vector2(20, ((Window.ClientBounds.Height / 2) - 200) + 10), Color.Blue);
                        spriteBatch.DrawString(spriteFont, saveName, new Vector2(20, ((Window.ClientBounds.Height / 2) - 100) + 10), Color.Blue);
                      //  name = "";
                        fileName = "";
                    }
                    if (showSaved && isStart)
                    {
                        spriteBatch.DrawString(spriteFont, "Data Saved!", new Vector2(Window.ClientBounds.Width / 2 - 100, ((Window.ClientBounds.Height / 2) + 100) + 10), Color.Blue);
                    }

                    if (isLoading)
                    {
                        spriteBatch.DrawString(spriteFont, fileName, new Vector2(20, ((Window.ClientBounds.Height / 2) - 100) + 10), Color.Blue);
                       // name="";
                        saveName = "";
                    }

                    if (showWarning)
                    {
                        spriteBatch.DrawString(spriteFont, "Empty Input!", new Vector2(Window.ClientBounds.Width / 2 - 100, ((Window.ClientBounds.Height / 2) + 100) + 10), Color.Blue);
                    }

                    
                }

                if (isMainMenu)
                {
                    spriteBatch.Draw(titleCard.btnTex, titleCard.btnRect, titleCard.btnColor);
                    spriteBatch.Draw(StartBtn.btnTex,StartBtn.btnRect, StartBtn.btnColor);
                    spriteBatch.Draw(HiScoreBtn.btnTex,HiScoreBtn.btnRect,HiScoreBtn.btnColor);
                    spriteBatch.Draw(loadBtn.btnTex, loadBtn.btnRect, loadBtn.btnColor);
                }

                if (isHighScore)
                {
                    spriteBatch.Draw(OkayBtn.btnTex, OkayBtn.btnRect, OkayBtn.btnColor);
                    spriteBatch.DrawString(spriteFont, "High Scores", new Vector2 (Window.ClientBounds.Width / 2 - 100, 20), Color.White);
                    spriteBatch.DrawString(spriteFont, "Name", new Vector2(200, 50), Color.White);
                    spriteBatch.DrawString(spriteFont, "Scores", new Vector2(450, 50), Color.White);
                    for (int i = 0; i < binaryScores.Count; i++)
                    {
                        if (i < 10)
                        {
                            //stream reader and writer method
                            //spriteBatch.DrawString(spriteFont, (i + 1).ToString(), new Vector2(100, 40 +(40* (i + 1))), Color.White);
                            //spriteBatch.DrawString(spriteFont, scores[i].name, new Vector2(200, 40 + (40 * (i + 1))), Color.White);
                            //spriteBatch.DrawString(spriteFont, scores[i].score.ToString(), new Vector2(450, 40 + (40 * (i + 1))), Color.White);
                            
                            //binary save and load
                            spriteBatch.DrawString(spriteFont, (i + 1).ToString(), new Vector2(100, 40 + (40 * (i + 1))), Color.White);
                            spriteBatch.DrawString(spriteFont, binaryScores[i].name, new Vector2(200, 40 + (40 * (i + 1))), Color.White);
                            spriteBatch.DrawString(spriteFont, binaryScores[i].Score.ToString(), new Vector2(450, 40 + (40 * (i + 1))), Color.White);
                        }
                    }
                }
            }
            #endregion

            #region drawIngame
            if (isPlaying)
            {
                spriteBatch.Draw(exitBlock.BlockTex, exitBlock.BlockRect, exitBlock.BlockColor);
                if (!DoubleBlock.hasPower)
                    spriteBatch.Draw(DoubleBlock.BlockTex, DoubleBlock.BlockRect, DoubleBlock.BlockColor);
                if (!FireBlock.hasPower)
                    spriteBatch.Draw(FireBlock.BlockTex, FireBlock.BlockRect, FireBlock.BlockColor);
                foreach (Explosion e in explosionXList)
                {
                    spriteBatch.Draw(e.ExplosionTex, e.ExplosionRect, Color.White);
                }

                foreach (Explosion e in explosionYList)
                {
                    spriteBatch.Draw(e.ExplosionTex, e.ExplosionRect, Color.White);
                }

                foreach (Blocks b in wallBlock)
                {
                    spriteBatch.Draw(b.BlockTex, b.BlockRect, b.BlockColor);
                }

                foreach (Blocks b in hardBlocks)
                {
                    spriteBatch.Draw(b.BlockTex, b.BlockRect, b.BlockColor);
                }

                foreach (Blocks b in softBlocks)
                {
                    spriteBatch.Draw(b.BlockTex, b.BlockRect, b.BlockColor);
                }
                spriteBatch.Draw(player.PlayerTex, player.PlayerRect, new Rectangle(player.SpriteSheetX, player.SpriteSheetY, 21, 25), player.PlayerColor);

                foreach (Bomb b in bombList)
                {
                    spriteBatch.Draw(b.BombTex, b.BombRect, new Rectangle(0, 0, 21, 16), b.BombColor);
                }

                foreach (enemyList e in enemies)
                {
                    spriteBatch.Draw(e.enemyTex, e.enemyRect,new Rectangle(0,16,21,16), e.enemyColor);
                }
                if (player.lives != 0)
                {
                    spriteBatch.DrawString(spriteFont, "Lives:" + player.lives.ToString(), new Vector2(250, Window.ClientBounds.Height - 35), Color.White);
                    spriteBatch.DrawString(spriteFont, "Score:" + score.ToString(), new Vector2(30, Window.ClientBounds.Height - 35), Color.White);
                    spriteBatch.DrawString(spriteFont, "Name:" + name.ToString(), new Vector2(400, Window.ClientBounds.Height - 35), Color.White);
                }
                else
                {
                    spriteBatch.DrawString(spriteFont, "Game Over!", new Vector2(30, Window.ClientBounds.Height - 35), Color.White);
                }
               
            }
            #endregion

            spriteBatch.End();
            base.Draw(gameTime);
        }

        #region saveAndLoad

        #region XMLSaveAndLoad
        void xmlLoad(String fN)
        {
            #region deletionOfBlocks
            softBlocks.Clear();
            enemies.Clear();
            #endregion

            #region TextureLoading
            Texture2D softTex = Content.Load<Texture2D>("SoftBlocks");
            Texture2D enemyTex = Content.Load<Texture2D>("enemyMove");

            Texture2D exitTexture = Content.Load<Texture2D>("ExitTex");
            Texture2D multiTexture = Content.Load<Texture2D>("MultiBomb");
            Texture2D rangeTexture = Content.Load<Texture2D>("LifeTex");
            #endregion

            #region ObjectDeclaration
            ScoreTracker savedScore = new ScoreTracker();

            Player savedPlayer = new Player();

            Blocks savedPowLife = new Blocks();
            Blocks savedPowDouble = new Blocks();
            Blocks savedExit = new Blocks();

            Blocks savedSoftBlocks = new Blocks();
            enemyList savedEnemies = new enemyList();
            #endregion

            #region serialization
            StreamReader sws = new StreamReader(fN+"_score");
            StreamReader swl = new StreamReader(fN+"_powlife");
            StreamReader swp = new StreamReader(fN+"_player");
            StreamReader swd = new StreamReader(fN+"_powdouble");
            StreamReader swe = new StreamReader(fN + "_powexit");
            StreamReader swsb = new StreamReader(fN+"_softblock");
            StreamReader swen = new StreamReader(fN + "_enemy");
            // xmlSerializer loadData = new XmlSerializer()

            XmlSerializer scoreLoadData = new XmlSerializer(savedScore.GetType());
            
            XmlSerializer playerLoadData = new XmlSerializer(savedPlayer.GetType());

            XmlSerializer powLifeLoadData = new XmlSerializer(savedPowLife.GetType());
            XmlSerializer powDoubleLoadData = new XmlSerializer(savedPowDouble.GetType());
            XmlSerializer exitLoadData = new XmlSerializer(savedExit.GetType());

            XmlSerializer SoftBlocksloadData = new XmlSerializer(savedSoftBlocks.GetType());
            XmlSerializer enemyloadData = new XmlSerializer(savedEnemies.GetType());

            ScoreTracker loadedScore = scoreLoadData.Deserialize(sws) as ScoreTracker;
            Player loadedPlayer = playerLoadData.Deserialize(swp) as Player;

            Blocks loadedPowLife = powLifeLoadData.Deserialize(swl) as Blocks;
            Blocks loadedPowDouble = powLifeLoadData.Deserialize(swd) as Blocks;
            Blocks loadedExit = exitLoadData.Deserialize(swe) as Blocks;

            Blocks loadedSoftBlocks = SoftBlocksloadData.Deserialize(swsb) as Blocks;
            enemyList loadedEnemies = enemyloadData.Deserialize(swen) as enemyList;

            Console.WriteLine();

            sws.Close();
            swp.Close();
            swl.Close();
            swd.Close();
            swe.Close();
            swsb.Close();
            swen.Close();

            name = loadedScore.savedName;
            score = loadedScore.savedScore;

            player.Position = loadedPlayer.Position;
            player.lives = loadedPlayer.lives;

            FireBlock.Position = loadedPowLife.Position;
            FireBlock.hasPower = loadedPowLife.hasPower;

            DoubleBlock.Position = loadedPowDouble.Position;
            DoubleBlock.hasPower = loadedPowDouble.hasPower;

            exitBlock.Position = loadedExit.Position;
            exitBlock.hasPower = loadedExit.hasPower;

            for(int i = 0; i < loadedSoftBlocks.numberOfBlocks; i++)
            {
                Blocks sB = new Blocks(new Rectangle(loadedSoftBlocks.position[i].X, loadedSoftBlocks.position[i].Y, 40, 40),softTex,Color.White);
                softBlocks.Add(sB);
            }

            for(int i =  0; i < loadedEnemies.numberOfEnemies; i++)
            {
                enemyList e = new enemyList(new Rectangle(loadedEnemies.position[i].X,loadedEnemies.position[i].Y,27,27),enemyTex,Color.White);
                enemies.Add(e);
            }

            #endregion
            //exitBlock.BlockRect = new Rectangle(exitBlock.Position.X + 5, exitBlock.Position.Y + 5, 30, 30);
            //DoubleBlock.BlockRect = new Rectangle(DoubleBlock.Position.X + 5, DoubleBlock.Position.Y + 5, 30, 30);
            //FireBlock.BlockRect = new Rectangle(FireBlock.Position.X + 5, FireBlock.Position.Y + 5, 30, 30);
            //enemyList e = new enemyList(new Rectangle(40, 160, 27, 27), enemyTex, Color.White); //create a list of enemies
            //enemies.Add(e);
            Console.WriteLine("LOADED!"+loadedSoftBlocks.numberOfBlocks);
            
        }
      
        void xmlSave(String fN)
        {
            #region ObjectDeclaration
            ScoreTracker savedScore = new ScoreTracker();

            Player savedPlayer = new Player();

            Blocks savedPowLife = new Blocks();
            Blocks savedPowDouble = new Blocks();
            Blocks savedExit = new Blocks();

            Blocks savedSoftBlocks = new Blocks();
            enemyList savedEnemies = new enemyList();
            #endregion

            #region savedObjectAssignment
            savedScore.savedScore = score;
            savedScore.savedName = name;

            savedPlayer.lives = player.lives;
            savedPlayer.Position = player.Position;
            savedPlayer.hasMulti = player.hasMulti;

            savedPowLife.Position = FireBlock.Position;
            savedPowDouble.Position = DoubleBlock.Position;
            savedExit.Position = exitBlock.Position;

            savedPowLife.hasPower = FireBlock.hasPower;
            savedPowDouble.hasPower = DoubleBlock.hasPower;
            savedExit.hasPower = exitBlock.hasPower;

            savedSoftBlocks.numberOfBlocks = softBlocks.Count;
            savedEnemies.numberOfEnemies = enemies.Count;

            
            foreach (Blocks s in softBlocks)
            {
                savedSoftBlocks.position.Add(s.Position);               
            }
            
            foreach(enemyList e in enemies)
            {
                savedEnemies.position.Add(e.Position);
            }
            #endregion

            #region serialization
            StreamWriter sws = new StreamWriter(fN+"_score");
            StreamWriter swp = new StreamWriter(fN+"_player");
            StreamWriter swl = new StreamWriter(fN+"_powlife");
            StreamWriter swd = new StreamWriter(fN+"_powdouble");
            StreamWriter swe = new StreamWriter(fN+"_powexit");
            StreamWriter swsb = new StreamWriter(fN+"_softblock");
            StreamWriter swen = new StreamWriter(fN+"_enemy");

            XmlSerializer savedScoreData = new XmlSerializer(savedScore.GetType());
            XmlSerializer savedPlayerData = new XmlSerializer(savedPlayer.GetType());
            XmlSerializer savedPowLifeData = new XmlSerializer(savedPowLife.GetType());
            XmlSerializer savedPowDoubleData = new XmlSerializer(savedPowDouble.GetType());

            XmlSerializer savedExitData = new XmlSerializer(savedExit.GetType());
             
            XmlSerializer saveSoftBlockData = new XmlSerializer(savedSoftBlocks.GetType());
            XmlSerializer saveEnemyData = new XmlSerializer(savedEnemies.GetType());

            savedScoreData.Serialize(sws,savedScore);
            savedPlayerData.Serialize(swp, savedPlayer);
            savedPowLifeData.Serialize(swl, savedPowLife);
            savedPowDoubleData.Serialize(swd, savedPowDouble);

            savedExitData.Serialize(swe, savedExit);

            saveSoftBlockData.Serialize(swsb, savedSoftBlocks);
            saveEnemyData.Serialize(swen, savedEnemies);


            sws.Close();
            swp.Close();
            swl.Close();
            swd.Close();
            swe.Close();
            swsb.Close();
            swen.Close();
            #endregion
            Console.WriteLine("SAVED!"+savedSoftBlocks.numberOfBlocks);
            
        }
        #endregion
        #region StreamReaderAndStreamWriter
        void load()
        { 
            StreamReader sr = new StreamReader("HighScores");

            string filedata = "";

            while((filedata = sr.ReadLine()) != null)
            {
                string[] data = filedata.Split(',');

                string prevName = data[0];
                int prevScore = int.Parse(data[1]);

                ScoreTracker temp = new ScoreTracker(prevScore,prevName);
                scores.Add(temp);
            }

            scores.Sort((a, b) => -1 * a.score.CompareTo(b.score));

            sr.Close();
        }
        void save()
        {
            ScoreTracker temp = new ScoreTracker(score, name);
            scores.Add(temp);

            StreamWriter sw = new StreamWriter("HighScores");
            scores.Sort((a, b) => -1 * a.score.CompareTo(b.score));

            foreach (ScoreTracker s in scores)
            {
                sw.WriteLine(s.name + "," + s.score);
            }


            sw.Close();

        }
        #endregion

        #region BinarySaveAndLoad
        void binaryLoadScore()
        {
            binaryScores = new List<BinarySaveClass>();

            FileStream highScoreStreamLoad = new FileStream("BIN/HighScore.DAB", FileMode.Open);

            BinaryFormatter scorebinaryDeserialization = new BinaryFormatter();
            BinarySaveClass[] scoreLoad = scorebinaryDeserialization.Deserialize(highScoreStreamLoad) as BinarySaveClass[];

            for (int i = 0; i < scoreLoad.Length; i++)
            {
                BinarySaveClass BScore = new BinarySaveClass();
                BScore.name = scoreLoad[i].name;
                BScore.Score = scoreLoad[i].Score;
                binaryScores.Add(BScore);
            }

            binaryScores.Sort((a, b) => -1 * a.Score.CompareTo(b.Score));

            highScoreStreamLoad.Close();
        }

        void binarySaveScore()
        {
            BinarySaveClass tempSave = new BinarySaveClass();
            tempSave.name = name;
            tempSave.Score = score;

            binaryScores.Add(tempSave);

            binaryScores.Sort((a, b) => -1 * a.Score.CompareTo(b.Score));

            FileStream highScoreStreamSave = new FileStream("BIN/HighScore.DAB", FileMode.Create);
            BinaryFormatter binarySerialization = new BinaryFormatter();
            BinarySaveClass[] saveScores = new BinarySaveClass[binaryScores.Count];

            for (int i = 0; i < binaryScores.Count; i++)
            {
                saveScores[i] = new BinarySaveClass();

                saveScores[i].name = binaryScores[i].name;
                saveScores[i].Score = binaryScores[i].Score;
            }

            binarySerialization.Serialize(highScoreStreamSave, saveScores);
            highScoreStreamSave.Close();
        }
        void binaryLoad(String fN)
        {
            Texture2D SoftBlockTex = Content.Load<Texture2D>("SoftBlocks");
            Texture2D enemyTex = Content.Load<Texture2D>("enemyMove");
            Texture2D exitTexture = Content.Load<Texture2D>("ExitTex");
            Texture2D multiTexture = Content.Load<Texture2D>("MultiBomb");
            Texture2D rangeTexture = Content.Load<Texture2D>("LifeTex");

            FileStream blockStreamLoad = new FileStream("BIN/" + fN + "_blockFile.DAB", FileMode.Open);
            FileStream enemyStreamLoad = new FileStream("BIN/" + fN + "enemyFile.DAB", FileMode.Open);

            FileStream lifePowUpStreamLoad = new FileStream("BIN/" + fN + "lifeFile.DAB", FileMode.Open);
            FileStream doublePowUpStreamLoad = new FileStream("BIN/" + fN + "doubleFile.DAB", FileMode.Open);
            FileStream exitStreamLoad = new FileStream("BIN/" + fN + "exitFile.DAB", FileMode.Open);
            FileStream currentScoreStreamLoad = new FileStream("BIN/" + fN + "CURRENT_SCORE.DAB", FileMode.Open);

            FileStream playerStreamLoad = new FileStream("BIN/" + fN + "playerFile.DAB", FileMode.Open);
           
            BinaryFormatter BlockbinaryDeserialization = new BinaryFormatter();
            BinaryFormatter EnemybinaryDeserialization = new BinaryFormatter();

            BinaryFormatter currentScoreBinaryDeserialization = new BinaryFormatter();
            BinaryFormatter LifePowbinaryDeserialization = new BinaryFormatter();
            BinaryFormatter DoublePowbinaryDeserialization = new BinaryFormatter();
            BinaryFormatter exitbinaryDeserialization = new BinaryFormatter();
            BinaryFormatter PlayerbinaryDeserialization = new BinaryFormatter();

            BinarySaveClass[] BlockLoad = BlockbinaryDeserialization.Deserialize(blockStreamLoad) as BinarySaveClass[];
            BinarySaveClass[] EnemyLoad = EnemybinaryDeserialization.Deserialize(enemyStreamLoad) as BinarySaveClass[];

            BinarySaveClass currentScoreLoad = currentScoreBinaryDeserialization.Deserialize(currentScoreStreamLoad) as BinarySaveClass;
            BinarySaveClass lifePowLoad = LifePowbinaryDeserialization.Deserialize(lifePowUpStreamLoad) as BinarySaveClass;
            BinarySaveClass doublePowLoad = DoublePowbinaryDeserialization.Deserialize(doublePowUpStreamLoad) as BinarySaveClass;
            BinarySaveClass exitLoad = exitbinaryDeserialization.Deserialize(exitStreamLoad) as BinarySaveClass;
            BinarySaveClass PlayerLoad = PlayerbinaryDeserialization.Deserialize(playerStreamLoad) as BinarySaveClass;

            //   BinarySaveClass
            softBlocks = new List<Blocks>();
            enemies = new List<enemyList>();
            
            for (int i = 0; i < BlockLoad.Length; i++)
            {
                Blocks sB = new Blocks(new Rectangle(BlockLoad[i].blockRectPosX,BlockLoad[i].blockRectPosY,BlockLoad[i].blockRectWidth,BlockLoad[i].blockRectHeight),
                    SoftBlockTex, Color.White);
                softBlocks.Add(sB);
            }

            for (int i = 0; i < EnemyLoad.Length; i++)
            {
                enemyList eN = new enemyList(new Rectangle(EnemyLoad[i].EnemyRectPosX, EnemyLoad[i].EnemyRectPosY, EnemyLoad[i].EnemyRectWidth, EnemyLoad[i].EnemyRectHeight),
                    enemyTex, Color.White);

                eN.decision = EnemyLoad[i].EnemyDirection;
                eN.Timer = EnemyLoad[i].EnemyTimer;

                enemies.Add(eN);
            }

            FireBlock.Position = new Point(lifePowLoad.blockRectPosX,lifePowLoad.blockRectPosY);
            FireBlock.hasPower = lifePowLoad.blockHasInteracted;

            DoubleBlock.Position = new Point(doublePowLoad.blockRectPosX, doublePowLoad.blockRectPosY);
            DoubleBlock.hasPower = doublePowLoad.blockHasInteracted;

            exitBlock.Position = new Point(exitLoad.blockRectPosX,exitLoad.blockRectPosY);
            exitBlock.hasPower = exitLoad.blockHasInteracted;

            player.Position = new Point(PlayerLoad.PlayerPositionX, PlayerLoad.PlayerPositionY);
            player.lives = PlayerLoad.PlayerLife;

            name = currentScoreLoad.name;
            score = currentScoreLoad.Score;

            Console.WriteLine("CURRENT SIZE ON SAVE:"+softBlocks.Count);

            exitStreamLoad.Close();

            currentScoreStreamLoad.Close();
            blockStreamLoad.Close();
            enemyStreamLoad.Close();

            lifePowUpStreamLoad.Close();
            doublePowUpStreamLoad.Close();
            playerStreamLoad.Close();
        }

        void binarySave(String fN)
        {
            FileStream blockStreamSave = new FileStream("BIN/" + fN + "_blockFile.DAB",FileMode.Create);
            FileStream enemyStreamSave = new FileStream("BIN/" + fN + "enemyFile.DAB", FileMode.Create);
            FileStream currentScoreStreamSave = new FileStream("BIN/" + fN + "CURRENT_SCORE.DAB", FileMode.Create);

            FileStream lifePowUpStreamSave = new FileStream("BIN/" + fN + "lifeFile.DAB", FileMode.Create);
            FileStream doublePowUpStreamSave = new FileStream("BIN/" + fN + "doubleFile.DAB", FileMode.Create);
            FileStream exitStreamSave = new FileStream("BIN/" + fN + "exitFile.DAB", FileMode.Create);
            FileStream playerStreamSave = new FileStream("BIN/" + fN + "playerFile.DAB", FileMode.Create);

            BinaryFormatter binarySerialization = new BinaryFormatter();

            BinarySaveClass[] saveBlocks = new BinarySaveClass[softBlocks.Count];
            BinarySaveClass[] saveEnemies = new BinarySaveClass[enemies.Count];

            BinarySaveClass saveCurrentScore = new BinarySaveClass();
            BinarySaveClass saveLifePowUp = new BinarySaveClass();
            BinarySaveClass saveDoublePowUp = new BinarySaveClass();
            BinarySaveClass saveExit = new BinarySaveClass();

            BinarySaveClass savePlayer = new BinarySaveClass();

            Console.WriteLine(binaryScores.Count + "SCORES");

            saveCurrentScore.name = name;
            saveCurrentScore.Score = score;

            for (int i = 0; i < softBlocks.Count; i++)
            {
                saveBlocks[i] = new BinarySaveClass();
                    
                saveBlocks[i].blockRectPosX = softBlocks[i].Position.X; //save posX
                saveBlocks[i].blockRectPosY = softBlocks[i].Position.Y; // save posY
                saveBlocks[i].blockRectWidth = softBlocks[i].BlockRect.Width; // save blockWidth
                saveBlocks[i].blockRectHeight = softBlocks[i].BlockRect.Height; //save blockHeight
                    
            }

            for(int i = 0; i < enemies.Count; i++)
            {
                saveEnemies[i] = new BinarySaveClass();

                saveEnemies[i].EnemyRectPosX = enemies[i].Position.X; //save posX
                saveEnemies[i].EnemyRectPosY = enemies[i].Position.Y; // save posY
                saveEnemies[i].EnemyRectWidth = enemies[i].enemyRect.Width; // save blockWidth
                saveEnemies[i].EnemyRectHeight = enemies[i].enemyRect.Height; //save blockHeight

                saveEnemies[i].EnemyDirection = enemies[i].decision;
                saveEnemies[i].EnemyTimer = enemies[i].Timer;
            }
           
            

            saveLifePowUp.blockRectPosX = FireBlock.Position.X;
            saveLifePowUp.blockRectPosY = FireBlock.Position.Y;
            saveLifePowUp.blockRectWidth = FireBlock.BlockRect.Width;
            saveLifePowUp.blockRectHeight = FireBlock.BlockRect.Height;
            saveLifePowUp.blockHasInteracted = FireBlock.hasPower;

            saveDoublePowUp.blockRectPosX = DoubleBlock.Position.X;
            saveDoublePowUp.blockRectPosY = DoubleBlock.Position.Y;
            saveDoublePowUp.blockRectWidth = DoubleBlock.BlockRect.Width;
            saveDoublePowUp.blockRectHeight = DoubleBlock.BlockRect.Height;
            saveDoublePowUp.blockHasInteracted = DoubleBlock.hasPower;

            savePlayer.PlayerPositionX = player.Position.X;
            savePlayer.PlayerPositionY = player.Position.Y;
            savePlayer.PlayerLife = player.lives;

            saveExit.blockRectPosX = exitBlock.Position.X;
            saveExit.blockRectPosY = exitBlock.Position.Y;
            saveExit.blockHasInteracted = exitBlock.hasPower;

            binarySerialization.Serialize(currentScoreStreamSave, saveCurrentScore);

            binarySerialization.Serialize(blockStreamSave, saveBlocks);
            binarySerialization.Serialize(enemyStreamSave, saveEnemies);
            binarySerialization.Serialize(exitStreamSave, saveExit);

            binarySerialization.Serialize(lifePowUpStreamSave, saveLifePowUp);
            binarySerialization.Serialize(doublePowUpStreamSave, saveDoublePowUp);
            binarySerialization.Serialize(playerStreamSave, savePlayer);

            currentScoreStreamSave.Close();

            blockStreamSave.Close();
            enemyStreamSave.Close();
            exitStreamSave.Close();

            lifePowUpStreamSave.Close();
            doublePowUpStreamSave.Close();
            playerStreamSave.Close();

            
            Console.WriteLine("SAVED! CURRENT SIZE ON LOAD:" + softBlocks.Count);

        }
        #endregion
        #endregion

        void Reset()
        {
            Texture2D titleTex = Content.Load<Texture2D>("Title");

            Texture2D HardBlockTex = Content.Load<Texture2D>("HardBlocks");
            Texture2D SoftBlockTex = Content.Load<Texture2D>("SoftBlocks");

            Texture2D playerTex = Content.Load<Texture2D>("PlayerWalkCycle");
            explosionTex = Content.Load<Texture2D>("explosionSheet");

            ExplosionX = Content.Load<Texture2D>("XPlosion");
            ExplosionY = Content.Load<Texture2D>("YPlosion");

            player.Position = new Point(40, 40);
            //player = new Player(new Rectangle(40, 40, 30, 30), playerTex, Color.White);

            wallBlock = new List<Blocks>();
            hardBlocks = new List<Blocks>();
            softBlocks = new List<Blocks>();

            spaceChecker = new List<Blocks>();

            bombList = new List<Bomb>();
            explosionList = new List<Explosion>();

            explosionXList = new List<Explosion>();
            explosionYList = new List<Explosion>();

            availableSpace = new List<Point>();
            enemies = new List<enemyList>();

            int RightPosition = 0;
            int BottomPosition = 0;

            for (int TopX = 0; TopX < 17; TopX++) // generate top region of the the wall
            {
                RightPosition = 40 * TopX; //the last index's point value will be used for the bottom portion
                Blocks b = new Blocks(new Rectangle(40 * TopX, 0, 40, 40), HardBlockTex, Color.White); //multiply the X value with the index in order to arrange each top block
                wallBlock.Add(b); //add temp value to list
            }

            for (int LeftY = 0; LeftY < 11; LeftY++) // generation the left portion of the wall
            {
                BottomPosition = 40 * LeftY; //last index's point value will be used for the right portion of the wall
                Blocks b = new Blocks(new Rectangle(0, 40 * LeftY, 40, 40), HardBlockTex, Color.White); //multiply the Y value with the index in order to arrange each left block
                wallBlock.Add(b);
            }

            for (int BottomX = 0; BottomX < 17; BottomX++) //generate bottom side of the wall
            {
                Blocks b = new Blocks(new Rectangle(40 * BottomX, BottomPosition, 40, 40), HardBlockTex, Color.White);
                wallBlock.Add(b);
            }

            for (int RightY = 0; RightY < 11; RightY++) //generate right side of the wall
            {
                Blocks b = new Blocks(new Rectangle(RightPosition, 40 * RightY, 40, 40), HardBlockTex, Color.White);
                wallBlock.Add(b);
            }
     
            for (int x = 0; x < 7; x++) //generate hard/non-destroyable blocks in the playing field
            {

                for (int y = 0; y < 5; y++)
                {
                    Blocks Block = new Blocks(new Rectangle(80 * (x + 1), 80 * y, 40, 40), HardBlockTex, Color.White);
                    hardBlocks.Add(Block);
                }

            }
            for (int X = 0; X < 7; X++)
            {
                for (int Y = 0; Y < 4; Y++)
                {
                    Blocks block = new Blocks(new Rectangle(40 * (X + 1), 40 * (Y + 1), 40, 40), SoftBlockTex, Color.White); //generate a pool of 28 blocks to be placed in random locations later on 
                    softBlocks.Add(block);
                }
            }

            for (int x = 0; x < 15; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    Blocks b = new Blocks(new Rectangle(40 * (x + 1), 40 * (y + 1), 40, 40), SoftBlockTex, Color.White); //generate 135 blocks/rectangles to check where the hard blocks are. 
                    spaceChecker.Add(b);                                            //this will be used to determine where we can place objects
                }
            }
            //create a grid for the playing field
            for (int x = 0; x < 15; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    for (int z = 0; z < spaceChecker.Count - 1; z++)
                    {
                        for (int w = 0; w < hardBlocks.Count; w++) //check how many non destructive blocks there are
                        {
                            if (spaceChecker[z].BlockRect.Intersects(hardBlocks[w].BlockRect)) //if the populated grid hits a hard block
                            {
                                spaceChecker.Remove(spaceChecker[z]); //delete the index that hit a block.
                            }
                        }
                    }
                }
            }
            Random randPos = new Random();
            spaceIndeces = new int[spaceChecker.Count - 1]; //indeces with available space

            for (int j = 0; j < spaceChecker.Count - 1; j++)
            {
                Point space = spaceChecker[j].BlockRect.Location;  //assign the spacechecker to a temporary point
                availableSpace.Add(space);           //since spacechecker does not have any positions intersecting with a hard block
            }                                   //we will assign it to a Point called availableSpace for easier readability

            for (int i = 0; i < spaceIndeces.Length; i++)
            {
                spaceIndeces[i] = randPos.Next(i, spaceChecker.Count - 1); //randomize the indeces of the availableSpace  list
            }

            //below will be the randomization of destructible blocks

            Blocks tempBlock;
            int[] randomCollection;
            randomCollection = new int[softBlocks.Count - 1];

            tempBlock = new Blocks(new Rectangle(80, 40, 40, 40), SoftBlockTex, Color.White); //temporary block

            for (int i = 0; i < softBlocks.Count - 1; i++)
            {
                softBlocks[i].Position = availableSpace[spaceIndeces[i]]; //assign the available random position to the soft block/destructible block list
                availableSpace[spaceIndeces[i]] = tempBlock.Position; //assign the temporary block to a random position
                tempBlock.Position = softBlocks[i].Position; //temporary block will be equal to the new soft block position
            }
            //there are some positions wherein the soft blocks are on top of each other
            //to lessen this, we will check which positions have colliding soft blocks

            collidingPositions = new List<Point>();
            NonCollidingPositions = new List<Point>();

            foreach (Blocks s in softBlocks)
            {
                foreach (Blocks ss in softBlocks) //iterate through the softblocks
                {
                    if (s.BlockRect.Intersects(ss.BlockRect)) //check if a soft block is colliding with another soft block
                    {
                        if (s != ss) //check to see if the softblock is not colliding with itself
                        {
                            Point p = s.Position;
                            collidingPositions.Add(p); //assign the position of the softblocks colliding with each other to a list
                        } //the list will be later used to determine
                    }
                }
            }
            availableSpace.Clear(); //clear the available space variable in order to be reused in
                                    //repositioning the colliding blocks

            //use space checker variable again since this covers the entire playing field
            for (int i = 0; i < spaceChecker.Count - 1; i++)
            {
                for (int j = 0; j < collidingPositions.Count - 1; j++)
                {
                    if (spaceChecker[i].Position == collidingPositions[j])
                    {
                        Console.WriteLine("W3W"); //debugging how many times this shows up to know how many objects are still colliding
                    }
                    else//if spacechecker isnt colliding with collidingPositions
                    {
                        Point p = spaceChecker[i].Position;  //assign the available positions to p
                        availableSpace.Add(p); //then add to the list of availableSpace
                    }
                }
            }
            randPos = new Random();
            int[] spaceIndex;
            spaceIndex = new int[availableSpace.Count]; //create another list to contain the index of available spaces

            for (int i = 0; i < availableSpace.Count; i++)
            {
                spaceIndex[i] = randPos.Next(i, availableSpace.Count - 1); //randomize the order of available space index

            }

            //finally, we will move the colliding soft blocks
            for (int i = 0; i < softBlocks.Count - 1; i++)
            {
                for (int j = 0; j < collidingPositions.Count - 1; j++)
                {
                    if (softBlocks[i].Position == collidingPositions[j]) //check if a soft block's position is colliding with one position
                    {                                                   //alam mo naman dapat may mag adjust.
                        softBlocks[i].Position = availableSpace[spaceIndex[j]]; //move one of the colliding blocks to an available space
                    }
                }
            }
            //spaceChecker.Clear();
            Random randExit = new Random();

            Texture2D exitTexture = Content.Load<Texture2D>("ExitTex");
            Texture2D multiTexture = Content.Load<Texture2D>("MultiBomb");
            Texture2D rangeTexture = Content.Load<Texture2D>("LifeTex");

            exitIndex = randExit.Next(0, softBlocks.Count);
            doubleBombIndex = randExit.Next(exitIndex, softBlocks.Count);
            rangeBombIndex = randExit.Next(doubleBombIndex, softBlocks.Count);

            exitBlock = new Blocks(new Rectangle(0, 0, 30, 30), exitTexture, Color.White);
            DoubleBlock = new Blocks(new Rectangle(0, 0, 30, 30), multiTexture, Color.White);
            FireBlock = new Blocks(new Rectangle(0, 0, 30, 30), rangeTexture, Color.White);

            if(exitIndex == doubleBombIndex)
            {
                exitIndex = randExit.Next(0, softBlocks.Count);
                doubleBombIndex = randExit.Next(0, rangeBombIndex);
            }

            if(doubleBombIndex == rangeBombIndex)
            {
                rangeBombIndex = randExit.Next(exitIndex, doubleBombIndex);
            }

            if(exitIndex == rangeBombIndex)
            {
                exitIndex = randExit.Next(0, rangeBombIndex);
            }

            exitBlock.Position = softBlocks[exitIndex].Position;
            DoubleBlock.Position = softBlocks[doubleBombIndex].Position;
            FireBlock.Position = softBlocks[rangeBombIndex].Position;

            exitBlock.BlockRect = new Rectangle(exitBlock.Position.X + 5, exitBlock.Position.Y + 5, 30, 30);
            DoubleBlock.BlockRect = new Rectangle(DoubleBlock.Position.X + 5, DoubleBlock.Position.Y + 5, 30, 30);
            FireBlock.BlockRect = new Rectangle(FireBlock.Position.X + 5, FireBlock.Position.Y + 5, 30, 30);

            Console.WriteLine("This Index has the exit:" + exitIndex + " soft block exit:" + softBlocks[exitIndex].Position);
            Console.WriteLine("This Index has double bomb:" + doubleBombIndex + " soft block exit:" + softBlocks[doubleBombIndex].Position);
            Console.WriteLine("This Index has range bomb:" + rangeBombIndex + " soft block exit:" + softBlocks[rangeBombIndex].Position);

            //generation of enemies

            //create a list of available space for the enemy
            Point[] availableEnemySpace;
            //initialize 5 spaces for the enemy
            availableEnemySpace = new Point[5];
            Texture2D enemyTex = Content.Load<Texture2D>("enemyMove");
            //  Console.WriteLine("BEFORE REMOVAL:"+spaceChecker.Count);
            for (int i = 0; i < 6; i++)
            {
                enemyList e = new enemyList(new Rectangle(40, 160, 27, 27), enemyTex, Color.White); //create a list of enemies
                enemies.Add(e);
            }

            spaceIndex = new int[spaceChecker.Count]; //create a list of available indeces

            for (int i = 0; i < spaceChecker.Count - 1; i++)
            {

                for (int j = 0; j < softBlocks.Count - 1; j++) //iterate through soft blocks
                {
                    if (spaceChecker[i].BlockRect.Intersects(softBlocks[j].BlockRect)) //if the space checker hits a soft block
                    {
                        spaceChecker.Remove(spaceChecker[i]);  //remove the space checker
                                                               //so that enemies will not be placed here
                    }
                }
            }

            spaceIndex = new int[spaceChecker.Count - 1]; //create an enemy position index randomizer

            for (int i = 0; i < spaceIndex.Length; i++)
            {
                spaceIndex[i] = randPos.Next(i, spaceIndex.Length - 1); //randomize the indeces
            }
            //space index contains the indeces with available space
            //we will assign the index to an array to determine which position will be assigned to an enemy 

            //below is the assignment of positions to enemies
            for (int i = 0; i < spaceIndex.Length; i++)
            {
                int previousIndex = 0;  //used to check what the previous index value was

                while (previousIndex == spaceIndex[i]) //while the previous index is equal to the current space index
                {
                    if (previousIndex != spaceIndex[i])
                    {
                        break; //break out of the while loop once the we find a value that is not equal to the current index
                    }
                    //if the number is still equal to the current index
                    if (spaceIndex.Length - 1 > i++)
                        spaceIndex[i] = spaceIndex[i++]; // assign the next index to the current index

                 //   previousIndex = spaceIndex[i]; //since this is the end of the loop, the current index will be the previous index
                                                   //this will be used to check duplicate indeces
                                                   //to prevent enemies spawning on top of each other
                }

                previousIndex = spaceIndex[i];
            }

            for (int i = 0; i < 6; i++)
            {
                enemies[i].Position = new Point(spaceChecker[spaceIndex[i]].Position.X + (enemies[i].enemyRect.Width / 4), spaceChecker[spaceIndex[i]].Position.Y + (enemies[i].enemyRect.Height / 4)); //assign a random position to the list of enemies
            }
        }
    }
}
