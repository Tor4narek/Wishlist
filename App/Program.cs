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
            try
            {
                await userView.AuthUser();
                await userView.ShowUser();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }


    }
}