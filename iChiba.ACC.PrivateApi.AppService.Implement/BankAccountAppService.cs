using Core.AppModel.Response;
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
    public class BankAccountAppService : BaseAppService, IBankAccountAppService
    {
        private readonly IBankAccountService bankAccountService;
        private readonly IUnitOfWork unitOfWork;
        public BankAccountAppService(ILogger<IAccountAppService> logger,
            IUnitOfWork unitOfWork, IBankAccountService bankAccountService) : base(logger)
        {
            this.bankAccountService = bankAccountService;
            this.unitOfWork = unitOfWork;
        }

        public Task<BankAccountListResponse> GetBankAccounts(BankAccountListRequest request)
        {
            var response = new BankAccountListResponse();

            TryCatch(() =>
            {
                var sorts = request.Sorts;
                var paging = request.ToPaging();
                var data = bankAccountService.GetBankAccounts(request.Keyword, request.BankAccount, request.BankName, request.Owner, request.Actives, sorts, paging);
                var responseData = data
                    .Select(m =>
                    {
                        var model = AutoMapper.Mapper.Map<Bank_Account, BankAccountView>(m);
                        return model;
                    })
                    .ToList();

                response.FromPaging(paging).SetData(responseData).Successful();
            }, response);
            return Task.FromResult(response);
        }

        public Task<BaseResponse> Add(BankAccountAddRequest request)
        {
            var response = new BaseResponse();
            TryCatch(() =>
            {

                var bankAccount = bankAccountService.GetAllBankAccounts();
                var arrBankAccount = bankAccount.Select(x => x.BankAccount).ToArray();
                if (arrBankAccount.Contains(request.BankAccount))
                {
                    throw new ErrorCodeException(ErrorCodeDefine.CHECK_BANK_ACCOUNT);
                }

                var model = new Bank_Account();
                model.BankAccount = request.BankAccount;
                model.BankName = request.BankName;
                model.Branch = request.Branch;
                model.Address = request.Address;
                model.Province = request.Province;
                model.Owner = request.Owner;
                model.Note = request.Note;
                bankAccountService.Add(model);

                unitOfWork.Commit();

                response.Successful();
                
            }, response);
            return Task.FromResult(response);
        }

        public Task<BaseResponse> Delete(int Id)
        {
            var response = new BaseResponse();
            TryCatch(() =>
            {
                var model = bankAccountService.GetById(Id);
                if (model == null)
                {
                    throw new ErrorCodeException(ErrorCodeDefine.GET_BANK_ACCOUNT);
                }

                bankAccountService.Delete(model);
                unitOfWork.Commit();
                response.Successful();
            },response);
            return Task.FromResult(response);
        }

    }
}
