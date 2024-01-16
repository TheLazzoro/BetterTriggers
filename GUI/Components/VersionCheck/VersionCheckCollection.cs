namespace GUI.Components.VersionCheck
{
    public class VersionCheckCollection
    {
        public VersionDTO? VersionDTO;
        public VersionCheckEnum VersionCheckEnum;
        public string? CurrentVersion;

        public VersionCheckCollection(VersionDTO? VersionDTO, VersionCheckEnum VersionCheckEnum)
        {
            this.VersionDTO = VersionDTO;
            this.VersionCheckEnum = VersionCheckEnum;
        }

        public VersionCheckCollection(VersionDTO? VersionDTO, VersionCheckEnum VersionCheckEnum, string CurrentVersion)
        {
            this.VersionDTO = VersionDTO;
            this.VersionCheckEnum = VersionCheckEnum;
            this.CurrentVersion = CurrentVersion;
        }
    }
}
