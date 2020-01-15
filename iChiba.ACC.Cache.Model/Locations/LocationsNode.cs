using ProtoBuf;
using System.Collections.Generic;
using System.Linq;

namespace iChiba.ACC.Cache.Cache.Model
{
    [ProtoContract]
    public class LocationsNode
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public string Code { get; set; }

        [ProtoMember(3)]
        public int? Status { get; set; }

        [ProtoMember(4)]
        public LocationsNode Parent { get; set; }

        [ProtoMember(5)]
        public IList<LocationsNode> Childs { get; set; }

        private IList<LocationsNode> allMembers;
        private IList<LocationsNode> AllMembers
        {
            get
            {
                if (allIdMembers != null
                    && allIdMembers.Any())
                {
                    return allMembers;
                }

                allMembers = GetAllMembers(this);

                return allMembers;
            }
        }

        private IList<int> allIdMembers;
        public IList<int> AllIdMembers
        {
            get
            {
                if (allIdMembers != null 
                    && allIdMembers.Any())
                {
                    return allIdMembers;
                }

                allIdMembers = AllMembers.Select(m => m.Id)
                    .ToList();

                return allIdMembers;
            }
        }

        private IList<string> allCodeMembers;
        public IList<string> AllCodeMembers
        {
            get
            {
                if (allCodeMembers != null
                    && allCodeMembers.Any())
                {
                    return allCodeMembers;
                }

                allCodeMembers = AllMembers.Select(m => m.Code)
                    .ToList();

                return allCodeMembers;
            }
        }

        public LocationsNode()
        {
            Childs = new List<LocationsNode>();
        }

        private IList<LocationsNode> GetAllMembers(LocationsNode node)
        {
            IList<LocationsNode> results = new List<LocationsNode>();

            results.Add(node);

            if (node.Parent != null)
            {
                results.Add(node.Parent);
            }

            node.Childs
               .Select(m => GetAllMembers(m))
               .SelectMany(m => m)
               .ToList()
               .ForEach(m => results.Add(m));

            return results;
        }
    }
}
