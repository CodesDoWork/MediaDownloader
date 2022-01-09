using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using static MediaDownloader.Data.DatabaseContext;

namespace MediaDownloader.Data
{
    public class TitleModifier
    {
        [Key] public int Id { get; set; }

        public string Target { get; set; }

        public bool IsActivated { get; set; } = true;

        public static ObservableCollection<TitleModifier> GetLocalSavedDownloads()
        {
            DBConnection.TitleModifiers.Load();
            return DBConnection.TitleModifiers.Local;
        }
    }
}
