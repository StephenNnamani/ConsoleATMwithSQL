using ATMApp.App;
using ATMApp.Domain.Entities;
using ATMApp.Domain.Enums;
using ATMApp.Domain.Interfaces;
using ATMApp.UI;
using ConsoleTables;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ATMApp
{
    public class ATMAppServices : IUserLogin, IUserAccountActions, ITransaction
    {
        //private readonly ATMDbContext _dbContext;
        private readonly ATMDbContext _dbContext = new ATMDbContext();
        private UserAccount selectedAccount;
        private List<Transaction> _listOfTransactions;
        private const decimal minimumKeptAmount = 500;
        private readonly AppScreen screen;
        private bool _disposed;
        private List<UserAccount> userAccountList;
        public ATMAppServices()
        {
            screen = new AppScreen();
        }
        
        public async Task Run()
        {
            AppScreen.Welcome();
            await CheckUserCardNumAndPassword();
            AppScreen.WelcomeCustomer(selectedAccount.FullName);
            while (true)
            {
                AppScreen.DisplayAppMenu();
                await ProcessMenuoption();
            }
        }
        public async Task CreateDB()
        {
            var sqlConn = await _dbContext.OpenConnection();
            //sqlConn.Open();

            string insertQuery =
                $"DROP DATABASE ATM";

            SqlCommand command = new SqlCommand(insertQuery, sqlConn);

            await command.ExecuteNonQueryAsync();

            string usedIdQuery =
                $"SELECT SCOPE_IDENTITY();";

            command.CommandText = usedIdQuery;

            command.ExecuteReader(CommandBehavior.CloseConnection);

        }

        public async Task CreateTable()
        {
            var sqlConn = await _dbContext.OpenConnection();
            //sqlConn.Open();

            string insertQuery =
                $"CREATE TABLE User(" +
                    $"[Id][int] NULL," +
                    $"[Firstname][nvarchar] (50) NULL," +
                    $"[Lastname][nvarchar] (50) NULL," +
                    $"[AccountNumber][bigint] NULL," +
                    $"[CardNumber][bigint] NULL," +
                    $"[CardPin][int] NULL," +
                    $"[AccountBalance][money] NULL," +
                    $"[isLocked][bit] NOT NULL" +
                    $") ON[PRIMARY]";

            SqlCommand command = new SqlCommand(insertQuery, sqlConn);

            await command.ExecuteNonQueryAsync();

            string usedIdQuery =
                $"SELECT SCOPE_IDENTITY();";

            command.CommandText = usedIdQuery;

            command.ExecuteReader(CommandBehavior.CloseConnection);
            
        }

        public async Task InsertUsers()
        {
            var sqlConn = await _dbContext.OpenConnection();
            //sqlConn.Open();

            string insertQuery =
                $"INSERT INTO User(Id, Firstname, Lastname, AccountNumber, CardNumber, CardPin, AccountBalance, isLocked)," +
                $"VALUES(1, 'Stephen', 'Nnamani', 1234567890, 1234512345, 1234, 10000000, 0);" +

                $"INSERT INTO User(Id, Firstname, Lastname, AccountNumber, CardNumber, CardPin, AccountBalance, isLocked)" +
                $"VALUES(1, 'Stanley', 'Eze', 0987654321, 1234123455, 0812, 11000, 0);" +

                $"INSERT INTO User(Id, Firstname, Lastname, AccountNumber, CardNumber, CardPin, AccountBalance, isLocked)" +
                $"VALUES(1, 'Nneka', 'Igwe', 1234012340, 1111211112, 0123, 571000, 0);" +

                $"INSERT INTO User(Id, Firstname, Lastname, AccountNumber, CardNumber, CardPin, AccountBalance, isLocked)" +
                $"VALUES(1, 'Caleb', 'Okeke', 2222522225, 0000500005, 0321, 167000, 0);" +

                $"INSERT INTO User(Id, Firstname, Lastname, AccountNumber, CardNumber, CardPin, AccountBalance, isLocked)" +
                $"VALUES(1, 'Amara', 'Favour', 9999099990, 0101010101, 0000, 718000, 0); " +

                $"INSERT INTO User(Id, Firstname, Lastname, AccountNumber, CardNumber, CardPin, AccountBalance, isLocked)" +
                $"VALUES(1, 'Vincent', 'Kay', 9999099990, 0101010101, 0000, 718000, 0); ";

            SqlCommand command = new SqlCommand(insertQuery, sqlConn);

            await command.ExecuteNonQueryAsync();

            string usedIdQuery =
                $"SELECT SCOPE_IDENTITY();";

            command.CommandText = usedIdQuery;

            command.ExecuteReader(CommandBehavior.CloseConnection);

        }

        public async Task<UserAccount> CheckUserCardNumAndPassword()
        {
            Console.WriteLine("Type in your Card Number");
            string cardNum = Console.ReadLine();
            Console.WriteLine("Type in your Card Pin");
            string cardPin = Console.ReadLine();

            var sqlConn = await _dbContext.OpenConnection();
            //sqlConn.Open();

            string insertQuery =
                $"SELECT * From User";

            SqlCommand command = new SqlCommand(insertQuery, sqlConn);

            await command.ExecuteNonQueryAsync();

            string usedIdQuery =
                $"SELECT SCOPE_IDENTITY();";

            command.CommandText = usedIdQuery;

            command.ExecuteReader(CommandBehavior.CloseConnection);
            UserAccount UserAccounts = new UserAccount();
            return UserAccounts;
        }
        
        //public void CheckUserCardNumAndPassword()
        //{
        //    bool isCorrectLogin = false;
        //    while (isCorrectLogin == false)
        //    {
        //        UserAccount inputAccount = AppScreen.UserLoginForm();
        //        AppScreen.LoginProgress();
        //        foreach(UserAccount account in userAccountList)
        //        {
        //            selectedAccount = account;
        //            if (inputAccount.CardNumber.Equals(selectedAccount.CardNumber))
        //            {
        //                selectedAccount.TotalLogin++;

        //                if (inputAccount.CardPin.Equals(selectedAccount.CardPin))
        //                {
        //                    selectedAccount = account;

        //                    if(selectedAccount.IsLocked || selectedAccount.TotalLogin > 3)
        //                    {
        //                        AppScreen.PrintLockScreen();
        //                    }
        //                    else
        //                    {
        //                        selectedAccount.TotalLogin = 0;
        //                        isCorrectLogin = true;
        //                        break;
        //                    }
        //                }
        //            }
        //            if (isCorrectLogin == false)
        //            {
        //                Utility.PrintMessage("\nInvalid card number or PIN.", false);
        //                selectedAccount.IsLocked = selectedAccount.TotalLogin == 3;
        //                if (selectedAccount.IsLocked)
        //                {
        //                    AppScreen.PrintLockScreen();
        //                }
        //            }
        //          Console.Clear();
        //        }
        //    }            
        //}

        private async Task ProcessMenuoption()
        {
            switch(Validator.Convert<int>("an option:"))
            {
                case (int)AppMenu.CheckBalance:
                    CheckBalance();
                    break;
                case (int)AppMenu.PlaceDeposit:
                    PlaceDeposit();
                    break;
                case (int)AppMenu.MakeWithdrawal:
                    MakeWithDrawal();
                    break;
                case (int)AppMenu.InternalTransfer:
                   var internalTransfer = screen.InternalTransferForm();
                    ProcessInternalTransfer(internalTransfer);
                    break;
                case (int)AppMenu.ViewTransaction:
                    ViewTransaction();
                    break;
                case (int)AppMenu.Logout:
                    AppScreen.LogoutProgress();
                    Utility.PrintMessage("You have successfully logged out. Please collect " +
                        "your ATM card.");
                    await Run();
                    break;
                default:
                    Utility.PrintMessage("Invalid Option.", false);
                    break;
            }
        }

        public void CheckBalance()
        {
            Utility.PrintMessage($"Your account balance is: {Utility.FormatAmount(selectedAccount.AccountBalance)}");
        }

        public void PlaceDeposit()
        {
           Console.WriteLine("\nOnly multiples of 500 and 1000 naira allowed.\n");
            var transaction_amt = Validator.Convert<int>($"amount {AppScreen.cur}");

            //simulate counting
            Console.WriteLine("\nChecking and Counting bank notes.");
            Utility.PrintDotAnimation();
            Console.WriteLine("");

            //some gaurd clause
            if (transaction_amt <= 0)
            {
                Utility.PrintMessage("Amount needs to be greater than zero. Try again.", false); ;
                return;
            }
            if(transaction_amt % 500 != 0)
            {
                Utility.PrintMessage($"Enter deposit amount in multiples of 500 or 1000. Try again.", false);
                return;
            }

            if (PreviewBankNotesCount(transaction_amt) == false)
            {
                Utility.PrintMessage($"You have cancelled your action.", false);
                return;
            }

            //bind transaction details to transaction object
            InsertTransaction(selectedAccount.Id, TransactionType.Deposit, transaction_amt, "");

            //update account balance
            selectedAccount.AccountBalance += transaction_amt;

            //print success message
            Utility.PrintMessage($"Your deposit of {Utility.FormatAmount(transaction_amt)} was " +
                $"succesful.", true);



        }

        public void MakeWithDrawal()
        {
            var transaction_amt = 0;
            int selectedAmount = AppScreen.SelectAmount();
            if (selectedAmount == -1)
            {
                MakeWithDrawal();
                return;
            }
            else if (selectedAmount != 0)
            {
                transaction_amt = selectedAmount;
            }
            else
            {
                transaction_amt = Validator.Convert<int>($"amount {AppScreen.cur}");
            }

            //input validation
            if(transaction_amt <= 0)
            {
                Utility.PrintMessage("Amount needs to be greater than zero. Try agin", false);
                return;
            }
            if(transaction_amt % 500 != 0)
            {
                Utility.PrintMessage("You can only withdraw amount in multiples of 500 or 1000 naira. Try again.", false);
                return;
            }
            //Business logic validations

            if(transaction_amt > selectedAccount.AccountBalance)
            {
                Utility.PrintMessage($"Withdrawal failed. Your balance is too low to withdraw" +
                    $"{Utility.FormatAmount(transaction_amt)}", false);
                return;
            }
            if((selectedAccount.AccountBalance - transaction_amt) < minimumKeptAmount)
            {
                Utility.PrintMessage($"Withdrawal failed. Your account needs to have " +
                    $"minimum {Utility.FormatAmount(minimumKeptAmount)}", false);
                return;
            }
            //Bind withdrawal details to transaction object
            InsertTransaction(selectedAccount.Id, TransactionType.Withdrawal, -transaction_amt, "");
            //update account balance
            selectedAccount.AccountBalance -= transaction_amt;
            //success message
            Utility.PrintMessage($"You have successfully withdrawn " +
                $"{Utility.FormatAmount(transaction_amt)}.", true);
        }

        private bool PreviewBankNotesCount(int amount)
        {
            int thousandNotesCount = amount / 1000;
            int fiveHundredNotesCount = (amount % 1000) / 500;

            Console.WriteLine("\nSummary");
            Console.WriteLine("------");
            Console.WriteLine($"{AppScreen.cur}1000 X {thousandNotesCount} = {1000 * thousandNotesCount}");
            Console.WriteLine($"{AppScreen.cur}500 X {fiveHundredNotesCount} = {500 * fiveHundredNotesCount}");
            Console.WriteLine($"Total amount: {Utility.FormatAmount(amount)}\n\n");

            int opt = Validator.Convert<int>("1 to confirm");
            return opt.Equals(1);
            
        }

        public void InsertTransaction(long _UserBankAccountId, TransactionType _tranType, decimal _tranAmount, string _desc)
        {
            //create a new transaction object
            var transaction = new Transaction()
            {
                TransactionId = Utility.GetTransactionId(),
                UserBankAccountId = _UserBankAccountId,
                TransactionDate = DateTime.Now,
                TransactionType = _tranType,
                TransactionAmount = _tranAmount,
                Descriprion = _desc
            };

            //add transaction object to the list
            _listOfTransactions.Add(transaction);
        }

        public void ViewTransaction()
        {
            var filteredTransactionList = _listOfTransactions.Where(t => t.UserBankAccountId == selectedAccount.Id).ToList();
            //check if there's a transaction
            if(filteredTransactionList.Count <= 0)
            {
                Utility.PrintMessage("You have no transaction yet.", true);
            }
            else
            {
                var table = new ConsoleTable("Id", "Transaction Date", "Type", "Descriptions", "Amount " + AppScreen.cur);
                foreach(var tran in filteredTransactionList)
                {
                    table.AddRow(tran.TransactionId, tran.TransactionDate, tran.TransactionType, tran.Descriprion, tran.TransactionAmount);
                }
                table.Options.EnableCount = false;
                table.Write();
                Utility.PrintMessage($"You have {filteredTransactionList.Count} transaction(s)", true);
            }           
        }
        private void ProcessInternalTransfer(InternalTransfer internalTransfer)
        {
           if(internalTransfer.TransferAmount <= 0)
            {
                Utility.PrintMessage("Amount needs to be more than zero. Try again.", false);
                return;
            }
           //check sender's account balance
           if(internalTransfer.TransferAmount > selectedAccount.AccountBalance)
            {
                Utility.PrintMessage($"Transfer failed. You do not hav enough balance" +
                    $" to transfer {Utility.FormatAmount(internalTransfer.TransferAmount)}", false);
                return;
            }
           //check the minimum kept amount 
           if((selectedAccount.AccountBalance - internalTransfer.TransferAmount) < minimumKeptAmount)
            {
                Utility.PrintMessage($"Transfer faile. Your account needs to have minimum" +
                    $" {Utility.FormatAmount(minimumKeptAmount)}", false);
                return;
            }

            //check reciever's account number is valid
            var selectedBankAccountReciever = (from userAcc in userAccountList
                                               where userAcc.AccountNumber == internalTransfer.ReciepeintBankAccountNumber
                                               select userAcc).FirstOrDefault();
            if(selectedBankAccountReciever == null)
            {
                Utility.PrintMessage("Transfer failed. Recieber bank account number is invalid.", false);
                return;
            }
            //check receiver's name
            if(selectedBankAccountReciever.FullName != internalTransfer.RecipientBankAccountName)
            {
                Utility.PrintMessage("Transfer Failed. Recipient's bank account name does not match.", false);
                return;
            }

            //add transaction to transactions record- sender
            InsertTransaction(selectedAccount.Id, TransactionType.Transfer, -internalTransfer.TransferAmount, "Transfered " +
                $"to {selectedBankAccountReciever.AccountNumber} ({selectedBankAccountReciever.FullName})");
            //update sender's account balance
            selectedAccount.AccountBalance -= internalTransfer.TransferAmount;

            //add transaction record-reciever
            InsertTransaction(selectedBankAccountReciever.Id, TransactionType.Transfer, internalTransfer.TransferAmount, "Transfered from " +
                $"{selectedAccount.AccountNumber}({selectedAccount.FullName})");
            //update reciever's account balance
            selectedBankAccountReciever.AccountBalance += internalTransfer.TransferAmount;
            //print success message
            Utility.PrintMessage($"You have successfully transfered" +
                $" {Utility.FormatAmount(internalTransfer.TransferAmount)} to " +
                $"{internalTransfer.RecipientBankAccountName}",true);

        }

        protected virtual void Dispose(bool disposing)
        {

            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _dbContext.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
