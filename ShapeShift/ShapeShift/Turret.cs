using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeShift
{
    class Turret : Entity
    {
        private Diamond tDiamond;
        private Entity owner;

        private Boolean deployed, dropped, expired = true, awaitingReset = false;
        private float currTime;

        private const float EXPIRE_TIME = 6f;
        private const double CONVERSION = Math.PI / 180;

        public Turret(ContentManager content, InputManager input, Entity owner)
        {
            LoadContent(content, input);
            this.owner = owner;
        }

        public override void LoadContent(ContentManager content, InputManager input)
        {
            base.LoadContent(content, input);
            currTime = 0;
            tDiamond = new Diamond(content);
            entityShape = tDiamond;
            moveAnimation = new SpriteSheetAnimation();
            moveAnimation.position = position;
            invulnPeriod = .01f;

            expired = false;
            deployed = false;
            dropped = false;

            tDiamond.scaleShape(.75f);
        }

        public void lockToOwner()
        {
            position = owner.position;
            entityShape.setPosition(position);
            moveAnimation.Position = position;
        }

        public void setOwner(Entity owner)
        { this.owner = owner; }

        public Entity getOwner()
        { return owner; }

        public Boolean isDeployed()
        { return deployed; }

        public Boolean isDropped()
        { return dropped; }

        public Boolean isExpired()
        { return expired; }

        public override void die()
        {
            Console.WriteLine("KILL ME");
            expired = true;
           deployed = false;
            dropped = false;
        }

        public override Rectangle getRectangle()
        {
            return new Rectangle((int)position.X, (int)position.Y, entityShape.getWidth(), entityShape.getHeight());
        }

        public override Boolean isEnemy()
        { return false; }

        public Boolean isAwaitingReset()
        { return awaitingReset; }

        public void reset()
        {
            awaitingReset = false;
        }

        public void shoot(GameTime gameTime, MatrixTileEnemy enemy)
        {
            if (tDiamond.isReady())
            {
                double xComposite = (enemy.getPositionX()+12.5 - position.X);
                double yComposite = (position.Y - (enemy.getPositionY() + 12.5));
                double radians = Math.Atan2(xComposite, yComposite);
                double degrees = radians / CONVERSION;
                tDiamond.shoot((int)degrees);
            }
        }

        public void deploySelf()
        {
            expired = false;
            deployed = true;
            tDiamond.turretGetDeployed();
        }

        public void dropSelf()
        {
            dropped = true;
            deployed = false;
            tDiamond.turretGetDropped();
        }
        public override Shape getShape()
        {
            return tDiamond;
        }

        public override void Update(GameTime gameTime, InputManager input, Collision col, Layers layer)
        {

           // base.Update(gameTime,input, col, layer);
            this.gameTime = gameTime;
            this.input = input;
            this.col = col;
            this.layer = layer;


            for (int i = 0; i < col.CollisionMap.Count; i++)
            {
                for (int j = 0; j < col.CollisionMap[i].Count; j++)
                {

                    if (col.CollisionMap[i][j] == "x") //Collision against solid objects (ex: Tiles)
                    {
                        //Creates a rectangle that is the current tiles postion and size
                        lastCheckedRectangle = new Rectangle((int)(j * layer.TileDimensions.X), (int)(i * layer.TileDimensions.Y), (int)(layer.TileDimensions.X), (int)(layer.TileDimensions.Y));

                        tDiamond.checkBulletCollision(position,lastCheckedRectangle,layer.getColorData(i, j, col.CollisionMap[i].Count));

                    }
                }
            }
                        

            tDiamond.Update(gameTime);

            if (!expired)
            {
                if (deployed)
                    lockToOwner();
                if (dropped)
                    currTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (currTime > EXPIRE_TIME)
                {
                    currTime = 0;
                    expired = true;
                }

                //tDiamond.collides();
                //TODO THIS ALL NEEDS TO BE FIXED
               /* foreach (Bullet bullet in tDiamond.getActiveBullets())
                {
                    if (bullet.outOfBounds())
                    {
                        bullet.hit();
                        tDiamond.clearBullets();
                    }
                }
                */

                if (expired)
                {
                    dropped = false;
                    tDiamond.clearBullets();
                    awaitingReset = true;
                }

                // Update all of the enabled animations
                List<SpriteSheetAnimation> Animations = entityShape.getActiveTextures();
                foreach (SpriteSheetAnimation animation in Animations)
                {
                    if (animation.IsEnabled)
                        animation.Update(gameTime);
                }

               

            }
            else
                tDiamond.clearBullets();
        }

        

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if(!expired)
                entityShape.Draw(spriteBatch);
        }
    }
}
