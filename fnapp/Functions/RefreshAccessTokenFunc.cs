using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;

namespace ldam.co.za.fnapp.Functions
{
    public class RefreshAccessTokenFunc
    {
        
        public RefreshAccessTokenFunc()
        {

        }

        [Function(nameof(RefreshAccessTokenFunc))]
        public async Task Run([TimerTrigger("")] TimerInfo timerInfo, FunctionContext functionContext)
        {

        }
    }
}