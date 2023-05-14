using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdaDanaService.Dtos;

namespace AdaDanaService.AsyncDataService
{
    public interface IMessageClient
    {
        void PublishTopupWallet(TopupWalletPublishDto topupWalletPublishDto);
    }
}