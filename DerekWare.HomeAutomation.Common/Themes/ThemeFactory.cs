using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using DerekWare.Collections;
using DerekWare.Diagnostics;

namespace DerekWare.HomeAutomation.Common.Themes
{
    public class ThemeFactory : Factory<Theme, IReadOnlyThemeProperties>
    {
        public static readonly ThemeFactory Instance = new();

        readonly XmlSerializer Serializer = new(typeof(List<UserTheme>));

        ThemeFactory()
        {
        }

        public IReadOnlyCollection<UserTheme> UserThemes => Items.Values.OfType<UserTheme>().ToList();

        public UserTheme AddUserTheme(string name)
        {
            var theme = new UserTheme { Name = name };
            Items.Add(theme.Name, theme);
            return theme;
        }

        public bool Contains(object other)
        {
            return this.Any(i => i.Matches(other));
        }

        public void LoadUserThemes(string storage)
        {
            List<UserTheme> themes;

            Items.RemoveWhere(i => i.Value is UserTheme);

            if(storage.IsNullOrEmpty())
            {
                return;
            }

            using(var reader = new StringReader(storage))
            {
                try
                {
                    themes = (List<UserTheme>)Serializer.Deserialize(reader);
                }
                catch(Exception ex)
                {
                    Debug.Error(this, ex);
                    return;
                }
            }

            foreach(var theme in themes)
            {
                Items.Add(theme.Name, theme);
            }
        }

        public bool RemoveUserTheme(string name)
        {
            return Items.RemoveWhere(i => i.Value is UserTheme && i.Key.Equals(name)) > 0;
        }

        public string SaveUserThemes()
        {
            using var writer = new StringWriter();
            Serializer.Serialize(writer, Items.Values.OfType<UserTheme>().ToList());
            return writer.ToString();
        }
    }
}
