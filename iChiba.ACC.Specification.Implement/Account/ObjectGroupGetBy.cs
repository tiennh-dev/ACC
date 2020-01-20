using Core.Specification.Abstract;
using iChiba.ACC.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace iChiba.ACC.Specification.Implement
{
   public class ObjectGroupGetBy : SpecificationBase<ObjectGroup>
    {
        public ObjectGroupGetBy(string Keyword, string groupId, string groupName, string note, bool Active)
         : base(m=> (m.GroupName.Contains(Keyword) || m.GroupId.Contains(Keyword) || m.Note.Contains(Keyword))
                    && (string.IsNullOrWhiteSpace(groupId) || m.GroupId.Contains(groupId))
                    && (string.IsNullOrWhiteSpace(groupName) || m.GroupName.Contains(groupName))
                    && (string.IsNullOrWhiteSpace(note) || m.Note.Contains(note))
                    && (Active==true ? m.Active==true : m.Active==false))
        {
        }
    }
}
