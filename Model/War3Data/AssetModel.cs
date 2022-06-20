using System;
using System.Collections.Generic;
using System.Text;

namespace Model.War3Data
{
    public class AssetModel
    {
        public string DisplayName;
        public string Path;
        public string Category;

        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var toCompare = obj as AssetModel;
            if (toCompare == null)
                return false;

            return Path.Equals(toCompare.Path);
        }
    }
}
