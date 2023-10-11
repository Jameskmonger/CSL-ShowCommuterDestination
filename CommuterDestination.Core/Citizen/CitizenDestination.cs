namespace CommuterDestination.Core.Citizen
{
    /// <summary>
    /// Represents the destination of a waiting citizen.
    /// </summary>
    public struct CitizenDestination
    {
        /// <summary>
        /// The stop ID of the stop at which the citizen will alight.
        /// </summary>
        public ushort stopId;

        /// <summary>
        /// The building ID of the building the citizen is travelling to,
        /// their journey's end.
        /// </summary>
        public ushort buildingId;
    }
}
