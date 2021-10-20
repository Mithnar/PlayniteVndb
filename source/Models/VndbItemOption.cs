using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VndbMetadata.Models
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
