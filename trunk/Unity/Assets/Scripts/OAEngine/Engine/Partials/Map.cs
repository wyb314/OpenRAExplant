using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Partials;

namespace Engine.Maps
{
    public partial class Map
    {
        public List<PlayerReference> Players { set; get; }

        public List<ActorReference> Actors { set; get; }
        
        public List<string> Rules { set; get; }
    }
}
