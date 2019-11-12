﻿using System.Collections.Generic;
using FroggerStarter.Utils;

namespace FroggerStarter.Model
{
    /// <summary>
    ///     The River
    /// </summary>
    /// <seealso cref="FroggerStarter.Model.Road" />
    public class River : Road
    {
        #region Data members

        /// <summary>
        ///     The start of river
        /// </summary>
        public double StartOfRiver;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="River" /> class.
        /// </summary>
        /// <param name="laneSettingsCollection">The collection of lane settings for this road's lanes.</param>
        /// <param name="totalHeight">The total height of the road.</param>
        /// <param name="laneLength">Length of the lanes.</param>
        public River(ICollection<LaneSettings> laneSettingsCollection, double totalHeight, double laneLength) : base(
            laneSettingsCollection, totalHeight, laneLength)
        {
            this.StartOfRiver = totalHeight;
            SetUpLanes(totalHeight, laneLength);

            this.placeAllVehiclesInLanes(laneLength);
        }

        private void placeAllVehiclesInLanes(double laneLength)
        {
            foreach (Lane lane in this.Lanes)
            {
                double laneY = 0;
                foreach (var debris in lane)
                {
                    laneY = debris.Y;
                }

                for (int i = 0; i < lane.MaxNumVehicles; i++)
                {
                    lane.AddVehicle();
                }

                lane.PlaceAllVehiclesInLane(laneLength);
                lane.SetVehicleYs(laneY);
            }
        }

        /// <summary>
        ///     Moves the objects in the river, returning the value by which the player needs to be moved if it
        ///     is riding on one..
        /// </summary>
        /// <param name="laneLen">Length of the lane.</param>
        /// <param name="player">The player.</param>
        /// <returns></returns>
        public double MoveRiver(double laneLen, GameObject player)
        {
            double changeX = 0;

            foreach (var debris in this)
            {
                if (debris.IsColliding(player))
                {
                    changeX = getDebrisMovement(debris);
                }
            }

            MoveLaneVehicles(laneLen);

            return changeX;
        }

        private static double getDebrisMovement(Vehicle.Vehicle debris)
        {
            double changeX;
            if (debris.Direction == Direction.Right)
            {
                changeX = debris.Speed;
            }
            else
            {
                changeX = -debris.Speed;
            }

            return changeX;
        }

        #endregion
    }
}