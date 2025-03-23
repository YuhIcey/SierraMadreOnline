namespace MadreServer.Hasher
{
    public static class BanHasher
    {
        public static string GetBanHash(string playerId)
        {
            return Hasher.SHA256($"ban:{playerId}");
        }

        public static bool IsPlayerBanned(string playerId, IEnumerable<string> banList)
        {
            var hash = GetBanHash(playerId);
            return banList.Contains(hash);
        }
    }
}
