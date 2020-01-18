using System;
using System.Collections.Generic;
using System.Text;

namespace iChiba.ACC.PrivateApi.AppModel.Model
{
   public class AccountView
    {
        public int Id { get; set; }
        public string No { get; set; }
        public string Name { get; set; }
        public int? Parent { get; set; }
        public int Type { get; set; }
        public string TypeDisplay { get; set; }
        public string Note { get; set; }
        public bool Active { get; set; }
        public IList<AccountView> subChildren { get; set; }
        public AccountView()
        {
            subChildren = new List<AccountView>();
        }
    }
}
