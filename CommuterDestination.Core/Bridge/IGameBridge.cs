using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine;

namespace CommuterDestination.Core.Bridge
{
    public interface IGameBridge
    {
        int GetStopIndex(ushort stopId);
        int GetStopPassengerCount(ushort stopId);
        Vector3 GetStopPosition(ushort stopId);
        ushort GetPrevStop(ushort stopId);
        ushort GetNextStop(ushort stopId);
        ushort GetStopTransportLineId(ushort stopId);
        Color32 GetStopLineColor(ushort stopId);
        string GetStopLineName(ushort stopId);
        float GetStopRange(ushort stopId);
        Vector3 GetBuildingPosition(ushort buildingId);
        ushort GetDestinationStopId(ushort originalStopId, ushort citizenId);
        bool IsCitizenInRangeOfStop(ushort citizenId, ushort stopId, double range);
        void SetCameraOnStop(ushort stopId);
    }
}
