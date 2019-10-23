﻿using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using FroggerStarter.Model;

namespace FroggerStarter.Controller
{
    /// <summary>
    ///     Manages all aspects of the game play including moving the player,
    ///     the Vehicles as well as lives and score.
    /// </summary>
    public class GameManager
    {
        #region Data members

        private const int BottomLaneOffset = 5;
        private const int TopOfGameOffset = 100;
        private const int TileHeight = 50;
        private const int LaneNum = 5;

        private readonly double roadHeight;
        private readonly double backgroundWidth;

        private Canvas gameCanvas;
        private DispatcherTimer timer;
        private RoadManager rm;
        private PlayerManager player;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the lives the player has left.
        /// </summary>
        /// <value>
        ///     The lives.
        /// </value>
        public int Lives => this.player.Lives;

        /// <summary>
        ///     Gets the score of the player.
        /// </summary>
        /// <value>
        ///     The score.
        /// </value>
        public int Score => this.player.Score;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameManager" /> class.
        /// </summary>
        /// <param name="backgroundHeight">Height of the background.</param>
        /// <param name="backgroundWidth">Width of the background.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     backgroundHeight &lt;= 0
        ///     or
        ///     backgroundWidth &lt;= 0
        /// </exception>
        public GameManager(double backgroundHeight, double backgroundWidth)
        {
            if (backgroundHeight <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(backgroundHeight));
            }

            if (backgroundWidth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(backgroundWidth));
            }

            this.backgroundWidth = backgroundWidth;
            this.roadHeight = backgroundHeight - BottomLaneOffset - TileHeight;

            this.setupGameTimer();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Occurs when [lives updated].
        /// </summary>
        public event EventHandler<EventArgs> LivesUpdated;

        /// <summary>
        ///     Occurs when [score updated].
        /// </summary>
        public event EventHandler<EventArgs> ScoreUpdated;

        /// <summary>
        ///     Occurs when [game over].
        /// </summary>
        public event EventHandler<EventArgs> GameOver;

        private void createRoadManager()
        {
            var traffic = new List<(int, VehicleType)> {
                (2, VehicleType.Car),
                (3, VehicleType.Bus),
                (3, VehicleType.Car),
                (2, VehicleType.Bus),
                (3, VehicleType.Car)
            };
            var flow = new List<(int, Direction)> {
                (1, Direction.Left),
                (2, Direction.Right),
                (3, Direction.Left),
                (4, Direction.Left),
                (5, Direction.Right)
            };
            this.rm = new RoadManager(LaneNum, traffic, flow);
            this.rm.SetUpLanes(this.roadHeight, this.backgroundWidth);
            this.initializeRoad();
        }

        private void setupGameTimer()
        {
            this.timer = new DispatcherTimer();
            this.timer.Tick += this.timerOnTick;
            this.timer.Interval = new TimeSpan(0, 0, 0, 0, 15);
            this.timer.Start();
        }

        /// <summary>
        ///     Initializes the game working with appropriate classes to play frog
        ///     and vehicle on game screen.
        ///     Precondition: background != null
        ///     Postcondition: Game is initialized and ready for play.
        /// </summary>
        /// <param name="gamePage">The game page.</param>
        /// <exception cref="ArgumentNullException">gameCanvas</exception>
        public void InitializeGame(Canvas gamePage)
        {
            this.gameCanvas = gamePage ?? throw new ArgumentNullException(nameof(gamePage));
            this.createAndPlacePlayer();
            this.createRoadManager();
        }

        private void initializeRoad()
        {
            foreach (var t in this.rm.Lanes)
            {
                foreach (var v in t.Vehicles)
                {
                    this.gameCanvas.Children.Add(v.Sprite);
                }
            }
        }

        private void createAndPlacePlayer()
        {
            this.player = new PlayerManager();
            this.gameCanvas.Children.Add(this.player.Player.Sprite);

            this.setPlayerToCenterOfBottomLane();
        }

        private void setPlayerToCenterOfBottomLane()
        {
            this.player.SetLocation(this.backgroundWidth / 2 - this.player.Player.Sprite.Width / 2, this.roadHeight);
        }

        private void timerOnTick(object sender, object e)
        {
            this.rm.OnTick(this.backgroundWidth);
            if (this.rm.CheckCollision(this.player.Player))
            {
                this.onCollision();
            }
        }

        private void onCollision()
        {
            this.rm.ResetLanes();
            this.setPlayerToCenterOfBottomLane();
            this.player.LoseLife();
            this.onLivesUpdated();
            if (this.Lives > 0)
            {
                return;
            }

            this.timer.Stop();
            this.onGameOver();
        }

        /// <summary>
        ///     Moves the player to the left.
        ///     Precondition: none
        ///     Postcondition: player.X = player.X@prev - player.Width
        /// </summary>
        public void MovePlayerLeft()
        {
            this.player.MoveLeft();
        }

        /// <summary>
        ///     Moves the player to the right.
        ///     Precondition: none
        ///     Postcondition: player.X = player.X@prev + player.Width
        /// </summary>
        public void MovePlayerRight()
        {
            this.player.MoveRight(this.backgroundWidth);
        }

        /// <summary>
        ///     Moves the player up.
        ///     Precondition: none
        ///     Postcondition: player.Y = player.Y@prev - player.Height
        /// </summary>
        public void MovePlayerUp()
        {
            this.player.MoveUp(TopOfGameOffset);

            this.checkVictory();
        }

        /// <summary>
        ///     Moves the player down.
        ///     Precondition: none
        ///     Postcondition: player.Y = player.Y@prev + player.Height
        /// </summary>
        public void MovePlayerDown()
        {
            this.player.MoveDown(this.roadHeight);
        }

        private void checkVictory()
        {
            if (!this.player.CheckWin(TopOfGameOffset + 1))
            {
                return;
            }

            this.setPlayerToCenterOfBottomLane();
            this.onScoreUpdated();
            if (this.Score < 3)
            {
                return;
            }

            this.timer.Stop();
            this.onGameOver();
        }

        private void onLivesUpdated()
        {
            this.LivesUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void onScoreUpdated()
        {
            this.ScoreUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void onGameOver()
        {
            this.GameOver?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}