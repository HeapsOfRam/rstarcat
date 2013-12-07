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

        private const float EXPIRE_TIME = 10f;
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

        public void forceExpire()
        {
            expired = true;
        }

        public override Boolean isEnemy()
        { return false; }

        public Boolean isAwaitingReset()
        { return awaitingReset; }

        public void reset()
        {
            awaitingReset = false;
        }

        public void shoot(GameTime gameTime, Entity enemy)
        {
            double xComposite = (enemy.getPositionX() - position.X);
            double yComposite = (position.Y - enemy.getPositionY());
            double radians = Math.Atan2(xComposite, yComposite);
            double degrees = radians / CONVERSION;
            tDiamond.shoot((int) degrees);
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

        public override void Update(GameTime gameTime, InputManager input, Collision col, Layers layer)
        {
            base.Update(gameTime, col, layer, this, tDiamond.getActiveBullets());

            if (!expired)
            {
                
                tDiamond.Update(gameTime);
                
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
                foreach (Bullet bullet in tDiamond.getActiveBullets())
                {
                    if (bullet.outOfBounds())
                    {
                        bullet.hit();
                        tDiamond.clearBullets();
                    }
                }


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

        public List<Shape> getActiveBullets()
        {
            Console.WriteLine("Bullets = " + entityShape.getActiveBullets().Count);
            return entityShape.getActiveBullets();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if(!expired)
                entityShape.Draw(spriteBatch);
        }
    }
}
