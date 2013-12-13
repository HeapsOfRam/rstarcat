using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;


namespace ShapeShift
{
    public class HighScores
    {
        List<string> menuItems;
        List<string> animationTypes, linkType, linkID;
        List<Texture2D> menuImages;
        List<List<Animation>> animation;
        List<string> initialsList;


        ContentManager content;
        FileManager fileManager;

        Vector2 position;
        int axis;
        string align;
        string filename;
        String scores;
        String names;
        List<string[]> highScoreList;
        List<string> highScoreNamesList;
        int parsedScore;
        bool newHighScore;
        String playerName = "player1";
        String currentKeyPressed = "";
        String nameInProgress = "";
        bool nameEntered = false;
        bool newHighScoringPlayerEntered = false;
        String aNullString = "";

        int highScore;

        List<List<string>> attributes, contents;
      

        List<Animation> tempAnimation;

        Rectangle source;

        SpriteFont font;
        SpriteFont font2;
        InputManager input;


        int itemNumber;

        private void SetMenuItems()
        {
            //make all the menu items to be equal
            for (int i = 0; i < menuItems.Count; i++)
            {
                if (menuImages.Count == i)
                    menuImages.Add(ScreenManager.Instance.NullImage);

            }
            for (int i = 0; i < menuImages.Count; i++)
            {
                if (menuItems.Count == i)
                    menuItems.Add("");
            }

        }

        private void SetAnimations()
        {

            Vector2 pos = Vector2.Zero;
            tempAnimation = new List<Animation>();
            Vector2 dimensions = Vector2.Zero;


            if (align.Contains("Center"))
            {

                //if the line is set to center
                //calculates total height and total width
                //subtract dimensions from the actual total screen width etc.
                for (int i = 0; i < menuItems.Count; i++)
                {
                    dimensions.X += font.MeasureString(menuItems[i]).X + menuImages[i].Width;
                    dimensions.Y += font.MeasureString(menuItems[i]).Y + menuImages[i].Height;
                    //This should give us: the total height or the total width of the images we're loading in

                    if (axis == 1)
                    {
                        pos.X = (ScreenManager.Instance.Dimensions.X - dimensions.X) / 2;
                    }

                    if (axis == 2)
                    {
                        pos.Y = (ScreenManager.Instance.Dimensions.Y - dimensions.Y) / 2;//starting position

                    }
                }

            }
            else
            {
                pos = position;
            }

            for (int i = 0; i < menuImages.Count; i++)
            {

                //Get the Width and Height of the current one that we are loading in
                dimensions = new Vector2(font.MeasureString(menuItems[i]).X + menuImages[i].Width, font.MeasureString(menuItems[i]).Y + menuImages[i].Height);
                if (axis == 1)
                    pos.Y = (ScreenManager.Instance.Dimensions.Y - dimensions.Y) / 2; //alignment
                else
                    pos.X = (ScreenManager.Instance.Dimensions.X - dimensions.X) / 2;

                for (int j = 0; j < animationTypes.Count; j++)
                {
                    switch (animationTypes[j])
                    {
                        case "Fade":
                            tempAnimation.Add(new FadeAnimation());
                            tempAnimation[tempAnimation.Count - 1].LoadContent(content, menuImages[i], menuItems[i], pos);
                            //what we are gonna do position-wise. Position based on actual axis
                            tempAnimation[tempAnimation.Count - 1].Font = font;
                            break;

                    }

                }

                if (tempAnimation.Count > 0)
                    animation.Add(tempAnimation);

                tempAnimation = new List<Animation>();


                //AXIS
                // (1) axis means horizontal - add the axis based off the width of the other one
                // (2) axis means vertical -- based off the height of the previous one
                //We will need to find out which is larger
                if (axis == 1)
                {
                    //add horizontally
                    pos.X += dimensions.X;
                }
                else
                {
                    pos.Y += dimensions.Y;
                }

            }

        }

        public void LoadContent(ContentManager content, string id)
        {
            this.content = new ContentManager(content.ServiceProvider, "Content");
            menuItems = new List<string>();
            animationTypes = new List<string>();
            menuImages = new List<Texture2D>();
            animation = new List<List<Animation>>();
            attributes = new List<List<string>>();
            contents = new List<List<string>>();
            linkType = new List<string>();
            linkID = new List<string>();
            itemNumber = 0;
            font = content.Load<SpriteFont>("Fonts/ScoreFont");
            font2 = content.Load<SpriteFont>("Fonts/ScoreFont2");
            filename = "Scores/Scores.txt";
            highScoreList = new List<string[]>();
            highScoreNamesList = new List<string>();
            parsedScore = 0;
            newHighScore = false;
            position = Vector2.Zero;
            initialsList = new List<string>();

            /*
            fileManager = new FileManager();
            fileManager.LoadContent("Load/Scores.starcat", attributes, contents, "Scores");

            for (int i = 0; i < attributes.Count; i++)
            {
                for (int j = 0; j < attributes[i].Count; j++)
                {
                    //Console.WriteLine("attributes[i][j]: " + attributes[i][j]);
                    switch (attributes[i][j])
                    {
                        case "Font":
                            font = this.content.Load<SpriteFont>("Fonts/" + contents[i][j]);
                            break;
                        case "Item":
                            menuItems.Add(contents[i][j]);
                            break;
                        case "Image":
                            menuImages.Add(this.content.Load<Texture2D>(contents[i][j]));
                            break;
                        case "Axis":
                            axis = int.Parse(contents[i][j]);
                            break;
                        case "Position":
                            string[] temp = contents[i][j].Split(' '); //split based on spaces
                            position = new Vector2(float.Parse(temp[0]), float.Parse(temp[1]));
                            break;
                        case "Source":
                            temp = contents[i][j].Split(' ');
                            source = new Rectangle(int.Parse(temp[0]), int.Parse(temp[1]), int.Parse(temp[2]), int.Parse(temp[3]));
                            break;
                        case "Animation":
                            animationTypes.Add(contents[i][j]);
                            break;
                        case "Align":
                            align = contents[i][j];
                            break;
                        case "LinkType":
                            linkType.Add(contents[i][j]);
                            break;
                        case "LinkID":
                            linkID.Add(contents[i][j]);
                            break;

                    }

                }
            }
             */

            using (StreamReader reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();//reads a line in the text file
                    string[] items = line.Split(',');
                    highScoreList.Add(items);
                    

                }

            }

            sort(highScoreList);
           

            //Code to sort the Values in the Text File from Highest to Lowest
            //highScoreList.Sort();

            for (int i = highScoreList.Count - 1; i >= 0; i--) //Sorted list
            {
                scores += highScoreList[i][0] + "\n";
                names += highScoreList[i][1] + "\n";//a space, then a line break
                Console.WriteLine(scores);
            }

            highScore = GameServices.GetService<int>();

            if (highScore != 0) //IF THE PLAYER ACHIEVES A NEW HIGH SCORE (Hence, higher then 3 scores are in the list)
            {
                newHighScore = true;

            }
            else //If there is no new high score
            {
                string[] scoreLine2 = { highScoreList[2][0] + "," + highScoreList[2][1], highScoreList[1][0] + "," + highScoreList[1][1], highScoreList[0][0] + "," + highScoreList[0][1]};

                System.IO.File.WriteAllLines(@".\Scores\Scores.txt", scoreLine2);
            }

            SetMenuItems();
            // SetAnimations();


        }
         

        public void UnloadContent()
        {
            content.Unload();
            fileManager = null;
            position = Vector2.Zero;
            animation.Clear();
            menuItems.Clear();
            menuImages.Clear();
            animationTypes.Clear();

        }

        public void Update(GameTime gameTime, InputManager inputManager)
        {
            input = inputManager;
            //MENU NAVIGATION - remember axis 1 is for horiz, axis 2 is for vertical
            if (axis == 1) //Horizontal
            {
                if (inputManager.KeyPressed(Keys.Right, Keys.D))   //Right, or D
                    itemNumber++;
                else if (inputManager.KeyPressed(Keys.Left, Keys.A))
                    itemNumber--;

            }
            else //axis = 2 (Vertical)
            {
                if (inputManager.KeyPressed(Keys.Down, Keys.S))   //Right, or D
                    itemNumber++;
                else if (inputManager.KeyPressed(Keys.Up, Keys.W))
                    itemNumber--;
            }

            /*if (inputManager.KeyPressed(Keys.Enter, Keys.Z))
            {
                if (linkType[itemNumber] == "Screen") //A link to a new screen
                {
                    //this is an easy, (C# way) to get the type and cast it as a game screen and create an instance
                    Type newClass = Type.GetType("ShapeShift." + linkID[itemNumber]); //whatever your namespace is
                    ScreenManager.Instance.AddScreen((GameScreen)Activator.CreateInstance(newClass), inputManager);
                }
                if (linkType[itemNumber] == "Score") // Display a score
                {
                    //this is an easy, (C# way) to get the type and cast it as a game screen and create an instance
                    Type newClass = Type.GetType("ShapeShift." + linkID[itemNumber]); //whatever your namespace is
                    ScreenManager.Instance.AddScreen((GameScreen)Activator.CreateInstance(newClass), inputManager);

                   
                }
            }*/

            currentKeyPressed = inputManager.getKeyPress();

            if (inputManager.KeyPressed(Keys.Space) && !newHighScore)
            {
                //this is an easy, (C# way) to get the type and cast it as a game screen and create an instance
                Type newClass = Type.GetType("ShapeShift.TitleScreen"); //whatever your namespace is
                ScreenManager.Instance.AddScreen((GameScreen)Activator.CreateInstance(newClass), inputManager);
            }
            else if (inputManager.KeyPressed(Keys.Enter) && !newHighScore)
            {
                //this is an easy, (C# way) to get the type and cast it as a game screen and create an instance
                Type newClass = Type.GetType("ShapeShift.GameplayScreen"); //whatever your namespace is
                ScreenManager.Instance.AddScreen((GameScreen)Activator.CreateInstance(newClass), inputManager);
            }

            if (itemNumber < 0)
                itemNumber = 0;

            else if (itemNumber > menuItems.Count - 1) //we do -1 because menu starts from zero
                itemNumber = menuItems.Count - 1;


            for (int i = 0; i < animation.Count; i++)
            {
                for (int j = 0; j < animation[i].Count; j++)
                {
                    if (itemNumber == i)
                        animation[i][j].IsActive = true;
                    else
                        animation[i][j].IsActive = false;

                    animation[i][j].Update(gameTime);
                }
            }

            if(!currentKeyPressed.Equals(aNullString) && newHighScore)
            {
                if(initialsList.Count < 3)
                initialsList.Add(currentKeyPressed);
                currentKeyPressed = aNullString;
              
            }

            if (inputManager.KeyPressed(Keys.Back) && initialsList.Count >= 1)
                initialsList.RemoveAt(initialsList.Count - 1);

            if (initialsList.Count == 3 && inputManager.KeyPressed(Keys.Enter))
            {              
                nameEntered = true;
            }

            if (nameEntered)
            {
                playerName = initialsList[0] + initialsList[1] + initialsList[2];
                Console.WriteLine("New High-Scoring Player: "+ playerName);
                newHighScore = false;
                nameEntered = false;
                newHighScoringPlayerEntered = true;
                initialsList.Clear();

                String[] score = new String[2];
                score[0] = highScore + "";
                score[1] = playerName;

                highScoreList.Add(score);

                sort(highScoreList);
               

                string[] scoreLine1 = { highScoreList[3][0] + "," + highScoreList[3][1], highScoreList[2][0] + "," + highScoreList[2][1], highScoreList[1][0] + "," + highScoreList[1][1] };
                scores = highScoreList[3][0] + " \n" + highScoreList[2][0] + "\n" + highScoreList[1][0] + "\n";//Used for the Score Screen, not the file
                names = highScoreList[3][1] + " \n" + highScoreList[2][1] + "\n" + highScoreList[1][1] + "\n";//Used for the Score Screen, not the file
                System.IO.File.WriteAllLines(@".\Scores\Scores.txt", scoreLine1);
            }

        }

        private void sort(List<string[]> highScoreList)
        {
            for (int s = 0; s <= highScoreList.Count - 1; s++)
            {
                for (int k = 0; k <= highScoreList.Count - 2; k++)
                {
                    if (int.Parse(highScoreList[k][0]) > int.Parse(highScoreList[k + 1][0]))
                    {   //comparing array values

                        String[] temp;
                        temp = highScoreList[k];     //storing value of array in temp variable 

                        highScoreList[k] = highScoreList[k + 1];    //swaping values
                        highScoreList[k + 1] = temp;    //now storing temp value in array


                    }    //end if block             
                }  // end inner loop    
            }
            //end outer loop
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < animation.Count; i++)
            {
                for (int j = 0; j < animation[i].Count; j++)
                {
                    animation[i][j].Draw(spriteBatch);
                }
            }
            spriteBatch.DrawString(font2, "High Scores", new Vector2(200, 50), Color.Blue);
            spriteBatch.DrawString(font, scores, new Vector2(200, 100), Color.White);
            spriteBatch.DrawString(font, names, new Vector2(100, 100), Color.White);

            if (newHighScore)
            {
                spriteBatch.DrawString(font2, "NEW HIGH SCORE!", new Vector2(100, 370), Color.Red);
                spriteBatch.DrawString(font2, "Please enter your name: ", new Vector2(120, 440), Color.Red);

                //For printing the letters that you typed
                if(initialsList.Count == 1)
                    spriteBatch.DrawString(font2, initialsList[0], new Vector2(530, 440), Color.Red);
                if (initialsList.Count == 2)
                {
                    spriteBatch.DrawString(font2, initialsList[0], new Vector2(530, 440), Color.Red);
                    spriteBatch.DrawString(font2, initialsList[1], new Vector2(560, 440), Color.Red);
                }
                if (initialsList.Count == 3)
                {
                    spriteBatch.DrawString(font2, initialsList[0], new Vector2(530, 440), Color.Red);
                    spriteBatch.DrawString(font2, initialsList[1], new Vector2(560, 440), Color.Red);
                    spriteBatch.DrawString(font2, initialsList[2], new Vector2(590, 440), Color.Red);
                }

                
            }
            if(newHighScoringPlayerEntered)
            spriteBatch.DrawString(font2, "Congratulations " + playerName + "!", new Vector2(120, 440), Color.Red);


            if (!newHighScore)
            {
                spriteBatch.DrawString(font2, "Press 'Space' to return to the Main Menu!", new Vector2(100, 500), Color.Orange);

                spriteBatch.DrawString(font2, "Press 'Enter' to play a New Game!", new Vector2(170, 570), Color.Yellow);
            }
        }

    }
}
