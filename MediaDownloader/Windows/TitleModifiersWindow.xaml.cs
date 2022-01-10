using System.Collections.ObjectModel;
using MediaDownloader.Data;
using static MediaDownloader.Data.DatabaseContext;

namespace MediaDownloader.Windows
{
    public partial class TitleModifiersWindow
    {
        public TitleModifiersWindow()
        {
            InitializeComponent();
            DataContext =  this;
            Closed      += (s, e) => DBConnection.SaveChanges();
        }

        public ObservableCollection<TitleModifier> TitleModifiers { get; } = TitleModifier.GetLocalSavedDownloads();
    }
}
