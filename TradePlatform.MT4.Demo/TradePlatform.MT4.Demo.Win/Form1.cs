﻿using System;
using System.Windows.Forms;

namespace TradePlatform.MT4.Demo.Win
{
    using TradePlatform.MT4.Core;
    using TradePlatform.MT4.Core.Exceptions;
    using TradePlatform.MT4.SDK.API;

    public partial class Form1 : Form
    {
        private MetaTrader4 metaTrader4;
        private int OrderCount = 0;
        private int HistoryCount = 0;

        public Form1()
        {
            InitializeComponent();

            Bridge.InitializeHosts(true);

            metaTrader4 = Bridge.GetTerminal(2088782777, "EURUSD.arm");
            metaTrader4.QuoteRecieved += metaTrader4_QuoteRecieved;
            metaTrader4.MqlError += metaTrader4_MqlError;
        }

        private void metaTrader4_MqlError(MqlErrorException mql)
        {
            // handler mql error
        }

        private void metaTrader4_QuoteRecieved(QuoteListener mql)
        {
            if (OrderCount != mql.OrdersTotal())
            {
                this.BeginInvoke((MethodInvoker)(() => dataGridView1.Rows.Clear()));


                for (int i = mql.OrdersTotal() - 1; i >= 0; i--)
                {
                    if (mql.OrderSelect(i, SELECT_BY.SELECT_BY_POS))
                    {
                        int orderTicket = mql.OrderTicket();
                        string orderSymbol = mql.OrderSymbol();
                        double orderLots = mql.OrderLots();
                        double orderProfit = mql.OrderProfit();

                        this.BeginInvoke((MethodInvoker)(() => dataGridView1.Rows.Add(orderTicket, orderSymbol, orderLots, orderProfit, "Close")));
                    }
                }
            }

            if (HistoryCount != mql.HistoryTotal())
            {
                this.BeginInvoke((MethodInvoker)(() => dataGridView2.Rows.Clear()));

                for (int i = mql.HistoryTotal() - 1; i >= 0; i--)
                {
                    if (mql.OrderSelect(i, SELECT_BY.SELECT_BY_POS, POOL_MODES.MODE_HISTORY))
                    {
                        int orderTicket = mql.OrderTicket();
                        string orderSymbol = mql.OrderSymbol();
                        double orderLots = mql.OrderLots();
                        double orderProfit = mql.OrderProfit();

                        this.BeginInvoke((MethodInvoker)(() => dataGridView2.Rows.Add(orderTicket, orderSymbol, orderLots, orderProfit)));
                    }
                }
            }

            HistoryCount = mql.HistoryTotal();
            OrderCount = mql.OrdersTotal();
            double bid = mql.Bid();
            double ask = mql.Ask();

            this.BeginInvoke((MethodInvoker) (() => BidPrice.Text = "Bid: " + bid.ToString()));
            this.BeginInvoke((MethodInvoker) (() => AskPrice.Text = "Ask: " + ask.ToString()));
        }

        private void buttonBuy_Click(object sender, EventArgs e)
        {
            metaTrader4.MqlScope(mql => mql.OrderSend("EURUSD", ORDER_TYPE.OP_BUY, 1, mql.Ask(), 10, 0, 0));
        }

        private void buttonSell_Click(object sender, EventArgs e)
        {
            metaTrader4.MqlScope(mql => mql.OrderSend("EURUSD", ORDER_TYPE.OP_SELL, 1, mql.Bid(), 10, 0, 0));
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {
                DataGridViewRow dataGridViewRow = dataGridView1.Rows[e.RowIndex];
                int value = (int) dataGridViewRow.Cells[0].Value;
                metaTrader4.MqlScope(mql =>
                    {
                        mql.OrderSelect(value, SELECT_BY.SELECT_BY_TICKET);
                        if (mql.OrderType() == ORDER_TYPE.OP_BUY)
                        {
                            mql.OrderClose(value, mql.OrderLots(), mql.Bid(), 3);
                        }
                        else
                        {
                            mql.OrderClose(value, mql.OrderLots(), mql.Ask(), 3);
                        }
                    });
            }
        }
    }
}
