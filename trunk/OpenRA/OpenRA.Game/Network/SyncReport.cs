﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;
//using System.Text;
//using OpenRA.Primitives;

//namespace OpenRA.Network
//{
//    using System.Globalization;
//    using NamesValuesPair = Pair<string[], object[]>;

//    class SyncReport
//    {
//        const int NumSyncReports = 5;
//        //static Cache<Type, TypeInfo> typeInfoCache = new Cache<Type, TypeInfo>(t => new TypeInfo(t));

//        readonly OrderManager orderManager;

//        readonly Report[] syncReports = new Report[NumSyncReports];
//        int curIndex = 0;

//        static NamesValuesPair DumpSyncTrait(ISync sync)
//        {
//            var type = sync.GetType();
//            TypeInfo typeInfo;
//            lock (typeInfoCache)
//                typeInfo = typeInfoCache[type];
//            var values = new object[typeInfo.Names.Length];
//            var index = 0;

//            foreach (var func in typeInfo.SerializableCopyOfMemberFunctions)
//                values[index++] = func(sync);

//            return Pair.New(typeInfo.Names, values);
//        }

//        public SyncReport(OrderManager orderManager)
//        {
//            this.orderManager = orderManager;
//            for (var i = 0; i < NumSyncReports; i++)
//                syncReports[i] = new Report();
//        }

//        internal void UpdateSyncReport()
//        {
//            GenerateSyncReport(syncReports[curIndex]);
//            curIndex = ++curIndex % NumSyncReports;
//        }

//        void GenerateSyncReport(Report report)
//        {
//            report.Frame = orderManager.NetFrameNumber;
//            report.SyncedRandom = orderManager.World.SharedRandom.Last;
//            report.TotalCount = orderManager.World.SharedRandom.TotalCount;
//            report.Traits.Clear();
//            report.Effects.Clear();

//            foreach (var actor in orderManager.World.ActorsHavingTrait<ISync>())
//                foreach (var syncHash in actor.SyncHashes)
//                    if (syncHash.Hash != 0)
//                        report.Traits.Add(new TraitReport()
//                        {
//                            ActorID = actor.ActorID,
//                            Type = actor.Info.Name,
//                            Owner = (actor.Owner == null) ? "null" : actor.Owner.PlayerName,
//                            Trait = syncHash.Trait.GetType().Name,
//                            Hash = syncHash.Hash,
//                            NamesValues = DumpSyncTrait(syncHash.Trait)
//                        });

//            foreach (var sync in orderManager.World.SyncedEffects)
//            {
//                var hash = Sync.Hash(sync);
//                if (hash != 0)
//                    report.Effects.Add(new EffectReport()
//                    {
//                        Name = sync.GetType().Name,
//                        Hash = hash,
//                        NamesValues = DumpSyncTrait(sync)
//                    });
//            }
//        }

//        internal void DumpSyncReport(int frame, IEnumerable<FrameData.ClientOrder> orders)
//        {
//            var reportName = "syncreport-" + DateTime.UtcNow.ToString("yyyy-MM-ddTHHmmssZ", CultureInfo.InvariantCulture) + ".log";
//            Log.AddChannel("sync", reportName);

//            foreach (var r in syncReports)
//            {
//                if (r.Frame == frame)
//                {
//                    var mod = Game.ModData.Manifest.Metadata;
//                    Log.Write("sync", "Player: {0} ({1} {2})", Game.Settings.Player.Name, Platform.CurrentPlatform, Environment.OSVersion);
//                    Log.Write("sync", "Game ID: {0} (Mod: {1} at Version {2})", orderManager.LobbyInfo.GlobalSettings.GameUid, mod.Title, mod.Version);
//                    Log.Write("sync", "Sync for net frame {0} -------------", r.Frame);
//                    Log.Write("sync", "SharedRandom: {0} (#{1})", r.SyncedRandom, r.TotalCount);
//                    Log.Write("sync", "Synced Traits:");
//                    foreach (var a in r.Traits)
//                    {
//                        Log.Write("sync", "\t {0} {1} {2} {3} ({4})".F(a.ActorID, a.Type, a.Owner, a.Trait, a.Hash));

//                        var nvp = a.NamesValues;
//                        for (int i = 0; i < nvp.First.Length; i++)
//                            if (nvp.Second[i] != null)
//                                Log.Write("sync", "\t\t {0}: {1}".F(nvp.First[i], nvp.Second[i]));
//                    }

//                    Log.Write("sync", "Synced Effects:");
//                    foreach (var e in r.Effects)
//                    {
//                        Log.Write("sync", "\t {0} ({1})", e.Name, e.Hash);

//                        var nvp = e.NamesValues;
//                        for (int i = 0; i < nvp.First.Length; i++)
//                            if (nvp.Second[i] != null)
//                                Log.Write("sync", "\t\t {0}: {1}".F(nvp.First[i], nvp.Second[i]));
//                    }

//                    Log.Write("sync", "Orders Issued:");
//                    foreach (var o in orders)
//                        Log.Write("sync", "\t {0}", o.ToString());

//                    return;
//                }
//            }

//            Log.Write("sync", "No sync report available!");
//        }

//        class Report
//        {
//            public int Frame;
//            public int SyncedRandom;
//            public int TotalCount;
//            public List<TraitReport> Traits = new List<TraitReport>();
//            public List<EffectReport> Effects = new List<EffectReport>();
//        }

//        struct TraitReport
//        {
//            public uint ActorID;
//            public string Type;
//            public string Owner;
//            public string Trait;
//            public int Hash;
//            public NamesValuesPair NamesValues;
//        }

//        struct EffectReport
//        {
//            public string Name;
//            public int Hash;
//            public NamesValuesPair NamesValues;
//        }

//        //struct TypeInfo
//        //{
//        //    static readonly ParameterExpression SyncParam = Expression.Parameter(typeof(ISync), "sync");
//        //    static readonly ConstantExpression NullString = Expression.Constant(null, typeof(string));
//        //    static readonly ConstantExpression TrueString = Expression.Constant(bool.TrueString, typeof(string));
//        //    static readonly ConstantExpression FalseString = Expression.Constant(bool.FalseString, typeof(string));

//        //    public Func<ISync, object>[] SerializableCopyOfMemberFunctions;
//        //    public string[] Names;

//        //    public TypeInfo(Type type)
//        //    {
//        //        const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
//        //        var fields = type.GetFields(Flags).Where(fi => !fi.IsLiteral && !fi.IsStatic && fi.HasAttribute<SyncAttribute>());
//        //        var properties = type.GetProperties(Flags).Where(pi => pi.HasAttribute<SyncAttribute>());

//        //        foreach (var prop in properties)
//        //            if (!prop.CanRead || prop.GetIndexParameters().Any())
//        //                throw new InvalidOperationException(
//        //                    "Properties using the Sync attribute must be readable and must not use index parameters.\n" +
//        //                    "Invalid Property: " + prop.DeclaringType.FullName + "." + prop.Name);

//        //        //var sync = Expression.Convert(SyncParam, type);
//        //        //SerializableCopyOfMemberFunctions = fields
//        //        //    .Select(fi => SerializableCopyOfMember(Expression.Field(sync, fi), fi.FieldType, fi.Name))
//        //        //    .Concat(properties.Select(pi => SerializableCopyOfMember(Expression.Property(sync, pi), pi.PropertyType, pi.Name)))
//        //        //    .ToArray();

//        //        //Names = fields.Select(fi => fi.Name).Concat(properties.Select(pi => pi.Name)).ToArray();
//        //    }

//        //    //static Func<ISync, object> SerializableCopyOfMember(MemberExpression getMember, Type memberType, string name)
//        //    //{
//        //    //    // We need to serialize a copy of the current value so if the sync report is generated, the values can
//        //    //    // be dumped as strings.
//        //    //    if (memberType.IsValueType)
//        //    //    {
//        //    //        // PERF: For value types we can avoid the overhead of calling ToString immediately. We can instead
//        //    //        // just box a copy of the current value into an object. This is faster than calling ToString. We
//        //    //        // can call ToString later when we generate the report. Most of the time, the sync report is never
//        //    //        // generated so we successfully avoid the overhead to calling ToString.
//        //    //        if (memberType == typeof(bool))
//        //    //        {
//        //    //            // PERF: If the member is a Boolean, we can also avoid the allocation caused by boxing it.
//        //    //            // Instead, we can just return the resulting strings directly.
//        //    //            var getBoolString = Expression.Condition(getMember, TrueString, FalseString);
//        //    //            return Expression.Lambda<Func<ISync, string>>(getBoolString, name, new[] { SyncParam }).Compile();
//        //    //        }

//        //    //        var boxedCopy = Expression.Convert(getMember, typeof(object));
//        //    //        return Expression.Lambda<Func<ISync, object>>(boxedCopy, name, new[] { SyncParam }).Compile();
//        //    //    }

//        //    //    // For reference types, we have to call ToString right away to get a snapshot of the value. We cannot
//        //    //    // delay, as calling ToString later may produce different results.
//        //    //    return MemberToString(getMember, memberType, name);
//        //    //}

//        //    //static Func<ISync, string> MemberToString(MemberExpression getMember, Type memberType, string name)
//        //    //{
//        //    //    // The lambda generated is shown below.
//        //    //    // TSync is actual type of the ISync object. Foo is a field or property with the Sync attribute applied.
//        //    //    var toString = memberType.GetMethod("ToString", Type.EmptyTypes);
//        //    //    Expression getString;
//        //    //    if (memberType.IsValueType)
//        //    //    {
//        //    //        // (ISync sync) => { return ((TSync)sync).Foo.ToString(); }
//        //    //        getString = Expression.Call(getMember, toString);
//        //    //    }
//        //    //    else
//        //    //    {
//        //    //        // (ISync sync) => { var foo = ((TSync)sync).Foo; return foo == null ? null : foo.ToString(); }
//        //    //        var memberVariable = Expression.Variable(memberType, getMember.Member.Name);
//        //    //        var assignMemberVariable = Expression.Assign(memberVariable, getMember);
//        //    //        var member = Expression.Block(new[] { memberVariable }, assignMemberVariable);
//        //    //        getString = Expression.Call(member, toString);
//        //    //        var nullMember = Expression.Constant(null, memberType);
//        //    //        getString = Expression.Condition(Expression.Equal(member, nullMember), NullString, getString);
//        //    //    }

//        //    //    return Expression.Lambda<Func<ISync, string>>(getString, name, new[] { SyncParam }).Compile();
//        //    //}
//        //}
//    }

//}
