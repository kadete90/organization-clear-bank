using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;

// Comments would not be in this class on production code, except rare cases where edge case requires extra explanation
namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IDataStore _dataStore;

        public PaymentService(IDataStore dataStore)
        {
            _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));

            // This logic would place it on Startup.ConfigureServices where would add the correct IDataStore implementation based on appsettings.json config file
            //var dataStoreType = ConfigurationManager.AppSettings["DataStoreType"] ?? throw new ArgumentNullException("DataStoreType");
            //_dataStore = dataStoreType switch
            //{
            //    "Backup" => new BackupAccountDataStore(),
            //    _ => new AccountDataStore()
            //};
        }

        // I would have make this method async so that datastore calls don't block the processing thread
        // but intructions say: "You should not change the method signature of the MakePayment method."
        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var account = _dataStore.GetAccount(request.DebtorAccountNumber);
            if (account == null)
            {
                // log warn/error or alternative throw exception InvalidOperationException
                return new MakePaymentResult
                {
                    Success = false
                };
            }

            var paymentResult = new MakePaymentResult
            {
                Success = request.PaymentScheme switch
                {
                    PaymentScheme.Bacs => AccountAllowsBacs(account),
                    PaymentScheme.FasterPayments => AllowsFasterPayments(account, request.Amount),
                    PaymentScheme.Chaps => AllowsChaps(account),
                    _ => false // default assumption
                }
            };

            if (paymentResult.Success)
            {
                account.Balance -= request.Amount;
                // ToDo handle UpdateAccount Exceptions
                _dataStore.UpdateAccount(account);
                return paymentResult;
            }

            _dataStore.UpdateAccount(account); // what needs to be updated if not sucessful?
            return paymentResult;
        }

        // Could refactor furthur, e.g. create interface 'IAllowsPaymentService' and each PaymentScheme would have his implementation, then in this class
        // 1. have a dictionary with PaymentScheme type and 'IAllowsPaymentService' implemenation, or 
        // 2. load all 'IAllowsPaymentService' implementations via reflection

        internal bool AccountAllowsBacs(Account account) =>
            account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs);

        internal bool AllowsFasterPayments(Account account, decimal amount) =>
            account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments) &&
            account.Balance >= amount;

        internal bool AllowsChaps(Account account) =>
            account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps) &&
            account.Status == AccountStatus.Live;
    }
}
