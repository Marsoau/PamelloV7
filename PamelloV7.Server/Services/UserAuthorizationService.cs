namespace PamelloV7.Server.Services
{
    public class UserAuthorizationService
    {
		private readonly Dictionary<int, ulong> _userCodes;

		public UserAuthorizationService() {
			_userCodes = new Dictionary<int, ulong>();
		}

		public int GetCode(ulong discordId) {
			int code;
			if (_userCodes.ContainsValue(discordId)) {
				code = _userCodes.First(v => v.Value == discordId).Key;
			}
			else {
				code = Random.Shared.Next(100000, 999999);
				_userCodes.Add(code, discordId);
			}

			return code;
		}

		public ulong? GetDiscordId(int code) {
			if (_userCodes.TryGetValue(code, out ulong discordId)) {
				_userCodes.Remove(code);
				return discordId;
			}
			return null;
		}
    }
}
