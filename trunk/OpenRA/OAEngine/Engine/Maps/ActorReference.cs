using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Primitives;

namespace Engine.Maps
{
    public partial class ActorReference
    {
        public string Type;
        TypeDictionary initDict;
        public TypeDictionary InitDict
        {
            get
            {
                if (initDict == null)
                {
                    //initDict = new TypeDictionary();
                    //foreach (var i in inits)
                    //    initDict.Add(LoadInit(i.Key, i.Value));
                }
                return initDict;
            }
        }
        
        //public ActorReference(string type, Dictionary<string, MiniYaml> inits)
        //{
        //    Type = type;
        //    initDict = Exts.Lazy(() =>
        //    {
        //        var dict = new TypeDictionary();
        //        foreach (var i in inits)
        //            dict.Add(LoadInit(i.Key, i.Value));
        //        return dict;
        //    });
        //}

        //static IActorInit LoadInit(string traitName, MiniYaml my)
        //{
        //    var info = Game.CreateObject<IActorInit>(traitName + "Init");
        //    FieldLoader.Load(info, my);
        //    return info;
        //}
        
        // for initialization syntax
        public void Add(object o) { InitDict.Add(o); }
        public IEnumerator GetEnumerator() { return InitDict.GetEnumerator(); }
    }
}
