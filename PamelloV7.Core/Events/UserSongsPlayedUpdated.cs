﻿using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class UserSongsPlayedUpdated : PamelloEvent
    {
        public UserSongsPlayedUpdated() : base(EEventName.UserSongsPlayedUpdated) { }

        public int UserId { get; set; }
        public int SongsPlayed { get; set; }
    }
}
