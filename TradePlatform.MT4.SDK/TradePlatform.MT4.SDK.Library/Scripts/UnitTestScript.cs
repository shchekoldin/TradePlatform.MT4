﻿

namespace TradePlatform.MT4.SDK.Library.Scripts
{
    using System.Diagnostics;
    using TradePlatform.MT4.Core;
    using TradePlatform.MT4.Core.Exceptions;
    using TradePlatform.MT4.Core.Utils;
    using TradePlatform.MT4.Engine.Log;
    using TradePlatform.MT4.SDK.Library.UnitTests;

    public class UnitTestScript : ExpertAdvisor
    {
        public UnitTestScript()
        {
            this.MqlError += this.OnMqlError;
        }

        private void OnMqlError(MqlErrorException mqlErrorException)
        {
            Trace.Write(new LogInfo(LogType.Notifications, message: "MQL Error: " + mqlErrorException.Message));
        }

        protected override int Init()
        {
            Trace.Write(new LogInfo(LogType.Notifications, message: "Init()"));

           // AccountInformationTests.RunTests(this);
            CommonFunctionsTests.RunTests(this);
            //PredefinedVariablesTests.RunTests(this);
           // TechnicalIndicatorsTests.RunTests(this);
//TradingFunctionsTests.RunTests(this);
          //  WindowFunctionsTests.RunTests(this);

            return 1;
        }

        protected override int Start()
        {
            Trace.Write(new LogInfo(LogType.Notifications, message: "Start()"));

            return 2;
        }

        protected override int DeInit()
        {
            Trace.Write(new LogInfo(LogType.Notifications, message: "DeInit()"));

            return 3;
        }
    }
}

