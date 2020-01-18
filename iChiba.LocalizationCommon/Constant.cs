using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace iChiba.LocalizationCommon
{
    public enum AccountTypeConfig
    {
        /// <summary>
        /// ghi nợ
        /// </summary>
        Credit = 1,
        /// <summary>
        /// ghi có
        /// </summary>
        Debit = 2,
        /// <summary>
        /// lưỡng tính
        /// </summary>
        Duality = 3,
        /// <summary>
        /// không số dư
        /// </summary>
        Nobalance = 4
    }

}
