using UnityEngine;

namespace CommuterDestination.Core.Bridge
{
    /// <summary>
    /// Represents an abstract "bridge" between the mod code and the game code.
    /// 
    /// Generally these functions will be thin layer to interact with the underlying game.
    /// </summary>
    public interface IGameBridge
    {
        /// <summary>
        /// Gets the index of the stop within the line
        /// </summary>
        /// <remarks>the first stop is 1, the second is 2, etc</remarks>
        /// <param name="stopId">the stop ID to look up</param>
        /// <returns>the index of the stop</returns>
        int GetStopIndex(ushort stopId);

        /// <summary>
        /// Gets the count of passengers currently waiting at the given stop ID.
        /// </summary>
        /// <param name="stopId">the stop ID to look up</param>
        /// <returns>the number of passengers</returns>
        int GetStopPassengerCount(ushort stopId);

        /// <summary>
        /// Gets the position for the given stop ID.
        /// </summary>
        /// <param name="stopId">the stop ID to look up</param>
        /// <returns>the world position of the stop</returns>
        Vector3 GetStopPosition(ushort stopId);

        /// <summary>
        /// Gets the ID of the previous stop on the line
        /// </summary>
        /// <param name="stopId">the stop ID to look from</param>
        /// <returns>the ID of the previous stop</returns>
        ushort GetPrevStop(ushort stopId);

        /// <summary>
        /// Gets the ID of the next stop on the line
        /// </summary>
        /// <param name="stopId">the stop ID to look from</param>
        /// <returns>the ID of the next stop</returns>
        ushort GetNextStop(ushort stopId);

        /// <summary>
        /// Gets the transport line ID for the given stop ID.
        /// </summary>
        /// <param name="stopId">the stop ID to look up</param>
        /// <returns>the transport line ID of the stop</returns>
        ushort GetStopTransportLineId(ushort stopId);

        /// <summary>
        /// Gets the color for the transport line that the given stop ID is part of.
        /// </summary>
        /// <param name="stopId">the stop ID to look up</param>
        /// <returns>the color of the stop's transport line</returns>
        Color32 GetStopLineColor(ushort stopId);

        /// <summary>
        /// Gets the name for the transport line that the given stop ID is part of.
        /// </summary>
        /// <param name="stopId">the stop ID to look up</param>
        /// <returns>the name of the stop's transport line</returns>
        string GetStopLineName(ushort stopId);

        /// <summary>
        /// Get the transit range of a given stop.
        /// </summary>
        /// <remarks>
        /// These values are taken from the LoadPassengers game methods.
        /// </remarks>
        /// <param name="stopId">the stop to get the transit range for</param>
        /// <returns>the transit range</returns>
        float GetStopRange(ushort stopId);

        /// <summary>
        /// Gets the position of the given building ID.
        /// </summary>
        /// <param name="buildingId">the building ID to look up</param>
        /// <returns>the position of the building</returns>
        Vector3 GetBuildingPosition(ushort buildingId);

        /// <summary>
        /// Get the closest destination stop for a citizen, given their origin stop<br />
        /// i.e. the stop at which they will get off the transit line after getting on at the origin stop
        /// </summary>
        /// <remarks>most of this is ripped from TransportArriveAtTarget</remarks>
        /// <param name="originalStopId">the citizen's origin stop</param>
        /// <param name="citizenId">the citizen's id</param>
        /// <returns>the stop ID</returns>
        ushort GetDestinationStopId(ushort originalStopId, ushort citizenId);

        /// <summary>
        /// Is the citizen within range of the given stop ID's?
        /// </summary>
        /// <param name="citizenId">the citizen ID</param>
        /// <param name="stopId">the stop ID</param>
        /// <param name="range">the range to check</param>
        /// <returns>`true` if the citizen is within the stop's radius, `false` otherwise</returns>
        bool IsCitizenInRangeOfStop(ushort citizenId, ushort stopId, double range);

        /// <summary>
        /// Sets the player's camera on the position of the given stop ID.
        /// </summary>
        /// <param name="stopId">the stop ID to look up</param>
        void SetCameraOnStop(ushort stopId);
    }
}
