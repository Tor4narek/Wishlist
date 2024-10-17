using System;
using System.Threading.Tasks;
using Models;
using Repository;
using Presenter;
using Presenter.Services;
using View;
using System;
using Npgsql;
namespace Whishlist
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // var connString = "Host=80.64.24.84;Port=5432;Username=wishlist_user;Password=yourpassword;Database=wishlist_db";
            //
            // using var conn = new NpgsqlConnection(connString);
            // conn.Open();
            //
            // using (var cmd = new NpgsqlCommand("SELECT NOW()", conn))
            // {
            //     var now = cmd.ExecuteScalar();
            //     Console.WriteLine($"Current time: {now}");
            // }
            
            UserView userView = new UserView();
            await userView.Start();
        }


    }
}