﻿using System;
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

        private Boolean deployed, dropped, expired;
        private float currTime;

        private const float EXPIRE_TIME = 5f;
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

        public override Boolean isEnemy()
        { return false; }

        public void shoot(GameTime gameTime, Entity enemy)
        {
            double xComposite = (enemy.getPositionX() - position.X);
            double yComposite = (position.Y - enemy.getPositionY());
            double radians = Math.Atan2(xComposite, yComposite);
            //double radians = Math.Atan(((Math.Abs(enemy.getPositionX() - position.X)) / (Math.Abs(enemy.getPositionY() - position.Y))));
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
            base.Update(gameTime, input, col, layer);

            if (dropped)
                currTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (currTime > EXPIRE_TIME)
            {
                currTime = 0;
                expired = true;
            }
            if (deployed)
                lockToOwner();

            if (expired)
            {
                dropped = false;
            }

            // (just a rectangle corresponding to the image)
            moveAnimation.Position = position;

            // Update all of the enabled animations
            List<SpriteSheetAnimation> Animations = entityShape.getActiveTextures();
            foreach (SpriteSheetAnimation animation in Animations)
            {               
                if (animation.IsEnabled)
                    animation.Update(gameTime);
            }

            tDiamond.Update(gameTime);

        }

        public List<Shape> getActiveBullets()
        {
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
