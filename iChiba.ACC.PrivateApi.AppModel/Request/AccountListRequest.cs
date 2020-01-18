using Core.AppModel.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace iChiba.ACC.PrivateApi.AppModel.Request
{
   public class AccountListRequest : SortRequest
    {
        public string Keyword { get; set; }
        public string Name { get; set; }
        public int? Type { get; set; }
        public bool Actives { get; set; }
    }
}
