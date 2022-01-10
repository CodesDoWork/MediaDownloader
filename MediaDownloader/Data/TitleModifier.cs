using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using static MediaDownloader.Data.DatabaseContext;

namespace MediaDownloader.Data
{
    public class TitleModifier
    {
        public static readonly List<TitleModifier> DefaultModifiers = new()
        {
            new TitleModifier {Target = "[\\[\\(][Oo]fficial[Aa]udio[\\]\\)]"},
            new TitleModifier {Target = "[\\[\\(][Oo]riginal [Aa]udio[\\]\\)]"},
            new TitleModifier {Target = "[\\[\\(][Oo]fficial [Vv]ideo[^\\]\\)]*[\\]\\)]"},
            new TitleModifier {Target = "[\\[\\(][Oo]fficial [Mm]usic [Vv]ideo[^\\]\\)]*[\\]\\)]"},
            new TitleModifier {Target = "[\\[\\(][Ll]yrics? [Vv]ideo[^\\]\\)]*[\\]\\)]"}
        };

        [Key] public int Id { get; set; }

        public string Target { get; set; }

        public bool IsActivated { get; set; } = true;

        public static ObservableCollection<TitleModifier> GetLocalSavedDownloads()
        {
            DBConnection.TitleModifiers.Load();
            var result = DBConnection.TitleModifiers.Local;

            foreach (var defaultModifier in DefaultModifiers)
            {
                if (result.All(modifier => modifier.Target != defaultModifier.Target))
                {
                    DBConnection.TitleModifiers.Add(defaultModifier);
                }
            }

            DBConnection.SaveChanges();
            return result;
        }
    }
}
