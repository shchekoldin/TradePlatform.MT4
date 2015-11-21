﻿namespace TradePlatform.MT4.SDK.Library.UnitTests
{
    using System.Diagnostics;
    using TradePlatform.MT4.Core;
    using TradePlatform.MT4.Core.Utils;
    using TradePlatform.MT4.Engine.Log;
    using TradePlatform.MT4.SDK.API;

    public static class AccountInformationTests
    {
        public static void RunTests(MqlHandler script)
        {
            Trace.Write(new LogInfo(LogType.Notifications, message: "Test 'AccountBalance: " + script.AccountBalance() + "'"));
            Trace.Write(new LogInfo(LogType.Notifications, message: "Test 'AccountCredit: " + script.AccountCredit() + "'"));
            Trace.Write(new LogInfo(LogType.Notifications, message: "Test 'AccountCompany: " + script.AccountCompany() + "'"));
            Trace.Write(new LogInfo(LogType.Notifications, message: "Test 'AccountCurrency: " + script.AccountCurrency() + "'"));
            Trace.Write(new LogInfo(LogType.Notifications, message: "Test 'AccountEquity: " + script.AccountEquity() + "'"));
            Trace.Write(new LogInfo(LogType.Notifications, message: "Test 'AccountFreeMargin: " + script.AccountFreeMargin() + "'"));
            Trace.Write(new LogInfo(LogType.Notifications, message: "Test 'AccountFreeMarginCheck: " + script.AccountFreeMarginCheck("EURUSD", ORDER_TYPE.OP_BUY, 1) + "'"));
            Trace.Write(new LogInfo(LogType.Notifications, message: "Test 'AccountFreeMarginMode: " + script.AccountFreeMarginMode() + "'"));
            Trace.Write(new LogInfo(LogType.Notifications, message: "Test 'AccountLeverage: " + script.AccountLeverage() + "'"));
            Trace.Write(new LogInfo(LogType.Notifications, message: "Test 'AccountMargin: " + script.AccountMargin() + "'"));
            Trace.Write(new LogInfo(LogType.Notifications, message: "Test 'AccountName: " + script.AccountName() + "'"));
            Trace.Write(new LogInfo(LogType.Notifications, message: "Test 'AccountNumber: " + script.AccountNumber() + "'"));
            Trace.Write(new LogInfo(LogType.Notifications, message: "Test 'AccountProfit: " + script.AccountProfit() + "'"));
            Trace.Write(new LogInfo(LogType.Notifications, message: "Test 'AccountServer: " + script.AccountServer() + "'"));
            Trace.Write(new LogInfo(LogType.Notifications, message: "Test 'AccountStopoutLevel: " + script.AccountStopoutLevel() + "'"));
            Trace.Write(new LogInfo(LogType.Notifications, message: "Test 'AccountStopoutMode: " + script.AccountStopoutMode() + "'"));
        }
    }
}
