using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Maps;

namespace Engine.Partials
{
    public class ActorInitInfo
    {
        public int ActorIdx { set; get; }

        public LocationInit Location { set; get; }

        public OwnerInit Owner { set; get; }
    }

    public partial class ActorReference
    {
        public string ActorTypeName { set; get; }

        public ActorInitInfo InitInfo { set; get; }

    }
}
