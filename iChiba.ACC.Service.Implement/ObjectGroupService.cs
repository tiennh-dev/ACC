using Core.Common;
using iChiba.ACC.Model;
using iChiba.ACC.Repository.Interface;
using iChiba.ACC.Service.Interface;
using iChiba.ACC.Specification.Implement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iChiba.ACC.Service.Implement
{
   public class ObjectGroupService : IObjectGroupService
    {
        private readonly IObjectGroupRepository objectGroupRepository;
        public ObjectGroupService(IObjectGroupRepository objectGroupRepository)
        {
            this.objectGroupRepository = objectGroupRepository;
        }

        public IList<ObjectGroup> GetObjectGroups(string Keyword, string groupId, string groupName, string note, bool Active, Sorts sorts, Paging paging)
        {
            return objectGroupRepository.Find(new ObjectGroupGetBy(Keyword, groupId, groupName, note, Active), sorts, paging).ToList();
        }

        public ObjectGroup GetById(int Id)
        {
            return objectGroupRepository.FindById(Id);
        }

        public IList<ObjectGroup> GetAllObjectGroups()
        {
            return objectGroupRepository.Find().ToList();
        }

        public void Add(ObjectGroup objectGroup)
        {
            objectGroupRepository.Add(objectGroup);
        }

        public void Delete(ObjectGroup objectGroup)
        {
            objectGroupRepository.Delete(objectGroup);
        }

        public void Update(ObjectGroup objectGroup)
        {
            objectGroupRepository.Update(objectGroup);

        }
    }
}
