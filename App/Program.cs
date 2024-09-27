using System;
using System.Threading.Tasks;
using Models;
using Repository;
using Presenter;
using Presenter.Services;
using View;

namespace Whishlist
{
    class Program
    {
        static async Task Main(string[] args)
        {
            UserView userView = new UserView();
            await userView.Start();
        }


    }
}