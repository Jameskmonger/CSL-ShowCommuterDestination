namespace CommuterDestination.Core.Bridge
{
    public class GameBridge
    {
        private static IGameBridge _bridge = null;

        public static IGameBridge Instance { get { return _bridge; } }

        public static void SetInstance(IGameBridge bridge)
        {
            _bridge = bridge;
        }
    }
}
