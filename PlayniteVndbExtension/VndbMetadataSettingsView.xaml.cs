using System.Windows.Controls;

namespace PlayniteVndbExtension
{
    public partial class VndbMetadataSettingsView
    {
        public VndbMetadataSettingsView()
        {
            InitializeComponent();
        }
    }
    
    public enum SpoilerLevel
    {
        None,
        Minor,
        Major
    }
}