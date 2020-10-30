using Playnite.SDK;

namespace PlayniteVndbExtension
{
    public class VndbItemOption : GenericItemOption
    {
        public uint Id { get; }
        
        public VndbItemOption(string name, string description, uint id) : base(name, description)
        {
            Id = id;
        }
    }
}