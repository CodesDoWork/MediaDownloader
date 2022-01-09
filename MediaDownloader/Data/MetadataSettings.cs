using MediaDownloader.Utils;

namespace MediaDownloader.Data
{
    public class MetadataSettings
    {
        public NotifyProperty<bool> SaveThumbnail { get; } = new(true);

        public NotifyProperty<bool> SavePlaylistAsAlbum { get; } = new(true);

        public void CopyFrom(MetadataSettings metadataSettings)
        {
            SaveThumbnail.Value       = metadataSettings.SaveThumbnail.Value;
            SavePlaylistAsAlbum.Value = metadataSettings.SavePlaylistAsAlbum.Value;
        }
    }
}
