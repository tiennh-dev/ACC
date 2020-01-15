using System.ComponentModel.DataAnnotations;

namespace iChiba.OM.PrivateApi.Configs
{
    public enum ConfigSettingDefine
    {
        [Display(Name = "ConnectionStrings:DbiChibaShoppingCmsConnectionString")]
        DbiChibaShoppingCmsConnectionString,

        [Display(Name = "ConnectionStrings:DbiChibaCustomerConnectionString")]
        DbiChibaCustomerConnectionString,
            [Display(Name = "ConnectionStrings:DbWarehouseConnectionString")]
        DbWarehouseConnectionString
    }
}
