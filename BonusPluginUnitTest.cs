using MetaQuotes.MT5CommonAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Tritech.Plugins.Test.Log4Net;

namespace Tritech.Plugins.Test
{
     [TestClass]
     public class BonusPluginUnitTest
     {
          class COperationType
          {
               public CIMTDeal.EnDealAction Value { get; set; }
               public string Name { get; set; }
               public override string ToString() { return (Name); }
          }
          //---
          CManager m_manager = null;
          bool isInit = false;

          string m_Server = "";
          UInt64 m_login = 0;
          string m_Password = "";

          UInt64 m_client_login = 0;

          public BonusPluginUnitTest()
          {
               Logger.LogIn();

               m_Server = "";
               m_login = ;
               m_Password = "forex123";
               m_client_login = ;

               m_manager = new CManager();
               if (!m_manager.Initialize())
               {
                    isInit = false;
                    Logger.Log("Initialize manager not success");
               }
               else
               {
                    isInit = true;
                    Logger.Log("Initialize manager success");
               }
          }

          [TestMethod]
          public void LoginTest()
          {
               Assert.IsTrue(isInit, "Initialize manager not success");
               bool loginResult = m_manager.Login(m_Server, m_login, m_Password);
               Logger.Log($"{{ loginResult : {loginResult} }}");
               Assert.IsTrue(loginResult, "Login failed");
               m_manager.Logout();


          }

          [TestMethod]
          public void InitialTest()
          {
               Assert.IsTrue(isInit, "Initialize manager not success");
               bool loginResult = m_manager.Login(m_Server, m_login, m_Password);
               Logger.Log($"{{ loginResult : {loginResult} }}");
               Assert.IsTrue(loginResult, "Login failed");

               (bool getUserInfo, UserInfo uinfo) = m_manager.GetUserInfo(m_client_login);
               Assert.IsTrue(getUserInfo, "GetUserInfo failed");

               (bool getAccountInfo, double balance, double equity) = m_manager.GetAccountInfo(m_client_login);
               uinfo.Balance = balance;
               uinfo.Equity = equity;
               Logger.Log($"{{ getAccountInfo : {getAccountInfo} }}");
               Assert.IsTrue(getAccountInfo, "GetAccountInfo failed");

               //deposit the amount
               double amount = 100;
               bool depositAmount = m_manager.DealerBalance(m_client_login, amount, (uint)CIMTDeal.EnDealAction.DEAL_CREDIT, "Initial deposit", true);
               Logger.Log($"{{ depositAmount : {depositAmount} }}");
               Assert.IsTrue(depositAmount, "DepositAmount failed");

               (getAccountInfo, balance, equity) = m_manager.GetAccountInfo(m_client_login);
               bool balanceCheck = (balance + amount) >= uinfo.Balance;
               Logger.Log($"{{ balanceCheck : {balanceCheck} }}");
               Assert.IsTrue(balanceCheck, "BalanceCheck failed");

               bool equityCheck = equity > uinfo.Equity;
               Assert.IsTrue(equityCheck, "EquityCheck  failed"); 

               CIMTDealArray deal_array = null;
               CIMTDeal deal = null;
               UInt64 login = m_client_login;
               DateTime from = DateTime.Now.AddHours(-1);
               DateTime to = DateTime.Now;
               string stype = string.Empty; 
         
               //--- get deal array 
               bool getUserDeals = m_manager.GetUserDeal(out deal_array, login, from, to);
               Assert.IsTrue(getUserDeals, "GetUserDeal failed");

              
               //--- for all deal in array
               for (uint i = 0; i < deal_array.Total(); i++)
               {
                    //--- get deal
                    deal = deal_array.Next(i);
                    //--- check error
                    if (deal == null) break;
                    //--- check action
                    switch ((CIMTDeal.EnDealAction)deal.Action())
                    {
                         case CIMTDeal.EnDealAction.DEAL_BALANCE:
                              stype = "Balance";
                              break;
                         case CIMTDeal.EnDealAction.DEAL_CREDIT:
                              stype = "Credit";
                              break;
                         case CIMTDeal.EnDealAction.DEAL_CHARGE:
                              stype = "Charge";
                              break;
                         case CIMTDeal.EnDealAction.DEAL_CORRECTION:
                              stype = "Correction";
                              break;
                         case CIMTDeal.EnDealAction.DEAL_BONUS:
                              stype = "Bonus";
                              break;
                         case CIMTDeal.EnDealAction.DEAL_COMMISSION:
                              stype = "Commission";
                              break;
                         default:
                              //--- skip other actions
                              continue;
                    }
                    //---
                    string stime = SMTTime.ToDateTime(deal.Time()).ToString("yyyy.MM.dd HH:mm:ss.fff");
                    Logger.Log($"{{ loginResult : {deal.Deal().ToString()} , {stype} , {deal.Profit().ToString()}}}");
               }
               //test
               m_manager.Logout();
          }

          ~BonusPluginUnitTest()
          {
               m_manager.Dispose();
          }
     }
}
