using ColossalFramework;
using UnityEngine;

namespace CSLShowCommuterDestination
{
    /// <summary>
    /// The <c>Bridge</c> class is responsible for interactions with the Cities: Skylines game engine
    /// 
    /// This keeps a clear separation between the mod functionality and the hackish code that is sometimes required to work with the game
    /// </summary>
    public class Bridge
    {
        public static NetNode GetStopNode(ushort stopId)
        {
            return Singleton<NetManager>.instance.m_nodes.m_buffer[stopId];
        }

        public static ushort GetStopTransportLineId(ushort stopId)
        {
            return GetStopNode(stopId).m_transportLine;
        }

        public static Vector3 GetStopPosition(ushort stopId)
        {
            return GetStopNode(stopId).m_position;
        }

        public static int GetStopPassengerCount(ushort stopId)
        {
            var transportLineId = GetStopTransportLineId(stopId);

            return Singleton<TransportManager>.instance.m_lines.m_buffer[transportLineId].CalculatePassengerCount(stopId);
        }

        public static string GetStopLineName(ushort stopId)
        {
            var transportLineId = GetStopTransportLineId(stopId);

            return Singleton<TransportManager>.instance.GetLineName(transportLineId);
        }

        public static int GetStopIndex(ushort stopId)
        {
            var transportLineId = GetStopTransportLineId(stopId);

            ushort currentStop = Singleton<TransportManager>.instance.m_lines.m_buffer[transportLineId].m_stops;

            int index = 1;
            while (currentStop != 0)
            {
                if (currentStop == stopId)
                {
                    return index;
                }
                ++index;

                currentStop = TransportLine.GetNextStop(currentStop);

                if (index >= 32768)
                {
                    break;
                }
            }

            return 0;
        }

        public static Vector3 GetBuildingPosition(ushort buildingId)
        {
            return Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingId].m_position;
        }

        public static void SetCameraOnStop(ushort stopId)
        {
            InstanceID instanceId = InstanceID.Empty;
            instanceId.NetNode = stopId;

            var stopPosition = GetStopPosition(stopId);

            ToolsModifierControl.cameraController.SetTarget(instanceId, stopPosition, false);
        }

        public static bool IsCitizenInRangeOfStop(ushort citizenId, ushort stopId, double range)
        {
            CitizenInstance citizen = Singleton<CitizenManager>.instance.m_instances.m_buffer[(int)citizenId];

            Vector3 stopPosition = Bridge.GetStopPosition(stopId);

            return Vector3.SqrMagnitude((Vector3)citizen.m_targetPos - stopPosition) < range;
        }
    }
}
