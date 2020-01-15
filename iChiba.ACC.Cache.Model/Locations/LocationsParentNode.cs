using ProtoBuf;
using System.Collections.Generic;
using System.Linq;

namespace iChiba.ACC.Cache.Cache.Model
{
    [ProtoContract]
    public class LocationsParentNode
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public string Code { get; set; }

        [ProtoMember(3)]
        public int? Status { get; set; }

        [ProtoMember(4)]
        public LocationsParentNode Parent { get; set; }

        [ProtoMember(5)]
        public IList<LocationsParentNode> Siblings { get; set; }

        private IList<LocationsParentNode> allMembers;
        private IList<LocationsParentNode> AllMembers
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

        public LocationsParentNode()
        {
            Siblings = new List<LocationsParentNode>();
        }

        private IList<LocationsParentNode> GetAllMembers(LocationsParentNode node)
        {
            var results = new List<LocationsParentNode>();

            results.Add(node);
            
            if (node.Siblings != null)
            {
                results.AddRange(node.Siblings);
            }
            
            if (node.Parent != null)
            {
                var items = GetAllMembers(node.Parent);

                results.AddRange(items);
            }

            return results;
        }
    }
}
