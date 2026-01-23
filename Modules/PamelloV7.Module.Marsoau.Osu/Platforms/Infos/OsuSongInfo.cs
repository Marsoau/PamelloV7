using osu.NET.Models.Beatmaps;
using PamelloV7.Core.Platforms;
using PamelloV7.Core.Platforms.Infos;

namespace PamelloV7.Module.Marsoau.Osu.Platforms.Infos;

public class OsuSongInfo : ISongInfo
{
    public ISongPlatform Platform { get; }
    
    public BeatmapSetExtended Beatmap { get; }

    public string Key => Beatmap.Id.ToString();
    public string Name => Beatmap.Title;
    public string CoverUrl => Beatmap.Covers.List;
    public List<IEpisodeInfo> Episodes { get; }
    
    public OsuSongInfo(ISongPlatform platform, BeatmapSetExtended beatmap) {
        Platform = platform;
        Beatmap = beatmap;
    }
}
