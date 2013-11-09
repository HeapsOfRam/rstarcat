using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeShift
{
    public class Layers
    {
        //See tutorial 29 for a detailed explanation of how the map class works

        List<List<List<Vector2>>> tileMap;
        List<List<Vector2>> layer;
        List<Vector2> tile;

        ContentManager content;
        FileManager fileManager;

        Texture2D[] tileSets;
        Vector2 tileDimensions;

        Boolean playback = false;
        int layerNumber;

        int currentTexture = 0;

        List<List<string>> attributes, contents;

        const int NUM_TILE_FRAMES = 18;
        int frameCounter;
        int switchFrame; //Is essentially the speed or however many frame counts we would like to wait before switching the frame.

        public int LayerNumber
        {
            set { layerNumber = value; }
        }

        public Vector2 TileDimensions
        {
            get { return tileDimensions; }
        }

        public void LoadContent(ContentManager content, string mapID)
        {
            //Map consists of layers and events(ex: end of level, powerups, etc.)

            this.content = new ContentManager(content.ServiceProvider, "Content");

            frameCounter = 0;
            switchFrame = 50;

            tileSets = new Texture2D[NUM_TILE_FRAMES];

            tile = new List<Vector2>();
            layer = new List<List<Vector2>>();
            tileMap = new List<List<List<Vector2>>>();
            attributes = new List<List<string>>();
            contents = new List<List<string>>();

            fileManager = new FileManager();
            fileManager.LoadContent("Load/Maps/" + mapID + ".starcat", attributes, contents, "Layers");

            for (int i = 0; i < attributes.Count; i++)
            {
                for (int j = 0; j < attributes[i].Count; j++)
                {
                    switch (attributes[i][j])
                    {
                        case "TileSet":
                            for (int k = 0; k < NUM_TILE_FRAMES; k++)
                                tileSets[k] = this.content.Load<Texture2D>("TileSets/tileSet" + (k + 1));                           
                           
                            break;
                        case "TileDimensions":
                            string[] split = contents[i][j].Split(',');
                            tileDimensions = new Vector2(int.Parse(split[0]), int.Parse(split[1]));
                            break;
                        case "StartLayer":
                            for (int k = 0; k < contents[i].Count; k++)
                            {
                                split = contents[i][k].Split(',');
                                tile.Add(new Vector2(int.Parse(split[0]), int.Parse(split[1])));
                            }
                            if (tile.Count > 0)
                                layer.Add(tile);
                            tile = new List<Vector2>();
                            break;
                        case "EndLayer":
                            if (layer.Count > 0)
                                tileMap.Add(layer);
                            layer = new List<List<Vector2>>();
                            break;


                    }
                }

            }

        }

        public void UnloadContent()
        {

            this.content.Unload();
            tileMap.Clear();
            layer.Clear();
            tile.Clear();
            attributes.Clear();
            fileManager = null;

        }



        public void Draw(SpriteBatch spriteBatch)
        {


            for (int k = 0; k < tileMap.Count; k++) //to draw all the layers
            {
                for (int i = 0; i < tileMap[k].Count; i++)
                {
                    for (int j = 0; j < tileMap[k][i].Count; j++)
                    {
                        spriteBatch.Draw(tileSets[currentTexture], new Vector2(j * tileDimensions.X, i * tileDimensions.Y),
                            new Rectangle((int)tileMap[k][i][j].X * (int)tileDimensions.X,
                                (int)tileMap[k][i][j].Y * (int)tileDimensions.Y,
                                (int)tileDimensions.X, (int)tileDimensions.Y), Color.White);
                    }
                }
            }
        }




        public void Update(GameTime gameTime)
        {
            frameCounter += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (frameCounter >= switchFrame)
            {
                frameCounter = 0;

                  
                   if (playback)
                       currentTexture--;
                   else
                       currentTexture++;

                if (currentTexture > NUM_TILE_FRAMES - 1)
                {
                    playback = true;
                    currentTexture--;
                }

                if (currentTexture < 0)
                {
                    playback = false;
                    currentTexture = 0;
                }
                
            }
        }


    }
}
