using Core.Common;
using iChiba.ACC.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace iChiba.ACC.Service.Interface
{
   public interface IObjectGroupService
    {
        IList<ObjectGroup> GetObjectGroups(string Keyword, string groupId, string groupName, string note,bool Active,Sorts sorts,Paging paging);
        IList<ObjectGroup> GetAllObjectGroups();
        void Add(ObjectGroup objectGroup);
        void Delete(ObjectGroup objectGroup);
        ObjectGroup GetById(int Id);
        void Update(ObjectGroup objectGroup);
    }
}
