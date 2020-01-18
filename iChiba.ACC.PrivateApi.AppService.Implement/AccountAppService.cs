using Core.CustomException;
using Core.Repository.Interface;
using iChiba.ACC.CustomException;
using iChiba.ACC.Model;
using iChiba.ACC.PrivateApi.AppModel.Model;
using iChiba.ACC.PrivateApi.AppModel.Request;
using iChiba.ACC.PrivateApi.AppModel.Response.Account;
using iChiba.ACC.PrivateApi.AppService.Interface;
using iChiba.ACC.Service.Interface;
using iChiba.LocalizationCommon;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iChiba.ACC.PrivateApi.AppService.Implement
{
    public class AccountAppService : BaseAppService, IAccountAppService
    {
        private readonly IAccountService accountService;
        public AccountAppService(ILogger<IAccountAppService> logger,
            IUnitOfWork unitOfWork, IAccountService accountService) : base(logger)
        {
            this.accountService = accountService;
        }

        public Task<AccountListResponse> GetAccount(AccountListRequest request)
        {
            var response = new AccountListResponse();

            TryCatch(() =>
            {
                var sort = request.Sorts;
                var paging = request.ToPaging();
                var data = accountService.GetAccounts(request.Keyword, request.Name, request.Type, request.Actives, sort, paging);
                var responseData = data
                    .Select(m =>
                    {
                        var model = AutoMapper.Mapper.Map<Account, AccountView>(m);
                        if (model.Type == (int)AccountTypeConfig.Credit)
                        {
                            model.TypeDisplay = "Ghi nợ";
                        }
                        else if (model.Type == (int)AccountTypeConfig.Debit)
                        {
                            model.TypeDisplay = "Ghi có";
                        }
                        else if (model.Type == (int)AccountTypeConfig.Duality)
                        {
                            model.TypeDisplay = "Lưỡng tính";
                        }
                        else if (model.Type == (int)AccountTypeConfig.Nobalance)
                        {
                            model.TypeDisplay = "Không có số dư";
                        }
                        else
                        {
                            model.TypeDisplay = "";
                        }

                        var lstChildrenAccount = GetListParentAccountView(model.Id);
                        model.subChildren = lstChildrenAccount;

                        return model;
                    })
                    .ToList();

                response.SetData(responseData).Successful();
            }, response);
            return Task.FromResult(response);
        }

        public List<AccountView> GetListParentAccountView(int parent)
        {
            try
            {
                var data = accountService.GetListAccountsByParent(parent);
                var dataMappings = data.Select(x =>
                {
                    var model = AutoMapper.Mapper.Map<Account, AccountView>(x);
                    return model;
                }).ToList();
                return dataMappings;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return null;
            }

        }

        public Task<AccountResponse> GetAccountById(int Id)
        {
            var response = new AccountResponse();
            TryCatch(() =>
            {
                var account = accountService.GetAccountById(Id);
                if (account == null)
                {
                    throw new ErrorCodeParameterException(ErrorCodeDefine.NOT_FOUND, new Dictionary<string, string>()
                    {
                        { nameof(Id),Id.ToString()}
                    });
                }
                var dataMappings = AutoMapper.Mapper.Map<Account, AccountView>(account);
                response.SetData(dataMappings).Successful();
            }, response);
            return Task.FromResult(response);
        }
    }
}
