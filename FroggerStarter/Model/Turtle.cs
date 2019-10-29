﻿using FroggerStarter.View.Sprites;

namespace FroggerStarter.Model
{
    /// <summary>
    ///     Defines the turtle model
    /// </summary>
    /// <seealso cref="FroggerStarter.Model.GameObject" />
    public class Turtle : GameObject
    {
        #region Data members

        private const int SpeedXDirection = 50;
        private const int SpeedYDirection = 50;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Turtle" /> class.
        /// </summary>
        public Turtle()
        {
            Sprite = new TurtleSprite();
            SetSpeed(SpeedXDirection, SpeedYDirection);
        }

        /// <summary>
        /// Changes the sprite.
        /// Postcondition: this.Sprite = the specified sprite.
        /// </summary>
        /// <param name="sprite">The sprite.</param>
        public void ChangeSprite(BaseSprite sprite)
        {
            Sprite = sprite;
        }

        #endregion
    }
}