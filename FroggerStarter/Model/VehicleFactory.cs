﻿namespace FroggerStarter.Model
{
    /// <summary>
    /// Creates vehicles
    /// </summary>
    public static class VehicleFactory
    {
        /// <summary>
        /// Makes the vehicle of given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="speed">The speed.</param>
        /// <param name="direction">The direction.</param>
        /// <returns></returns>
        public static Vehicle MakeVehicle(VehicleType type, int speed, Direction direction)
        {
            Vehicle vehicle = null;
            switch (type)
            {
                case VehicleType.Car:
                    vehicle = new Car(speed, direction);
                    break;
                case VehicleType.Bus:
                    vehicle = new Bus(speed, direction);
                    break;

            }

            return vehicle;
        }
    }
}