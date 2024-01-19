using System;
using System.Threading.Tasks;
using MCQuery;

namespace LegendLauncher.Managers
{
    public static class ServerInfoManager
    {
        private static string address = "legend.myftp.org";
        private static int port = 25565;

        #region Is Server Online
        public static async Task<bool> IsServerOnline()
        {
            try
            {
                MCServer server = new MCServer(address, port);
                ServerStatus status = await Task.Run(() => server.Status(2000));
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        #endregion

        #region Get Online Players
        public static string GetOnlinePlayers()
        {
            try
            {
                if (string.IsNullOrEmpty(address) || port <= 0)
                {
                    return "0";
                }

                MCServer server = new MCServer(address, port);

                if (server == null)
                {
                    return "0";
                }

                ServerStatus status = server.Status();

                if (status == null)
                {
                    return "0";
                }

                try
                {
                    if (status.Players == null)
                    {
                        return "0";
                    }

                    string onlinePlayers = $"{status.Players.Online}";
                    return onlinePlayers;
                }
                catch (NullReferenceException)
                {
                    return "0";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting online players: {ex.Message}");
                    return "0";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "0";
            }
        }
        #endregion
    }
}
