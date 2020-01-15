namespace iChiba.ACC.PrivateApi.AppService.Implement.Configs
{
    public class AppConfig
    {
        public string DepositPrivateKeyPepper { get; set; }
        public int DepositAmountMinimum { get; set; }

        public string WithdrawPrivateKeyPepper { get; set; }
        public int WithdrawAmountMinimum { get; set; }

        public int FreezeTemporaryDepositValue { get; set; }
        public string FreezePrivateKeyPepper { get; set; }

        public string CustomerWalletPrivateKeyPepper { get; set; }

        public int PaymentPayOrderAmountMinimum { get; set; }
        public int PaymentCancelOrderAmount { get; set; }
        public string PaymentPrivateKeyPepper { get; set; }

        public string AppGroupResourceKey { get; set; }
    }
}
