using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace forms
{
    public partial class Form1 : Form
    {
        EventWaitHandle ew;
        EventWaitHandle ewC; // counter
        EventWaitHandle ewLR; // Line right
        EventWaitHandle ewLL; // Line left
        EventWaitHandle ewRT; // Rectangle top
        EventWaitHandle ewRR; // Rectangle right
        EventWaitHandle ewRD; // Rectangle down
        EventWaitHandle ewRL; // Rectangle left
        bool Stop, Pause;
        bool StopC, PauseC;
        bool StopL, PauseL;
        bool StopR, PauseR;
        Pen pn = new Pen(Color.Red, 4);      
        Pen pnLR = new Pen(Color.Blue, 10);  
        Pen pnLL = new Pen(Color.Blue, 10);  
        Pen pnRT = new Pen(Color.Blue, 10);  
        Pen pnRR = new Pen(Color.Blue, 10);  
        Pen pnRD = new Pen(Color.Blue, 10);  
        Pen pnRL = new Pen(Color.Blue, 10);
        Thread[] thL = new Thread[2];
        Thread[] thR = new Thread[4];


        Graphics g;
        Graphics gLL;
        Graphics gLR;
        Graphics gRT;
        Graphics gRR;
        Graphics gRD;
        Graphics gRL;
        int XR = 760;       // X rect   Top-left point
        int YR = 150;       // Y rect
        int Rlen = 150;     // Length of side rect
        int XL = 500;       // Betvin X Line
        int YL = 200;       // betwin Y Line
        int XLR;            // X right Line
        int XLL;            // X left Line
        int XRTR;           // X top-right
        int YRRD;           // Y right-down
        int XRDL;           // X down-left
        int YRLT;           // Y left-top

        public Form1()
        {

            InitializeComponent();
            ew = new EventWaitHandle(false, EventResetMode.AutoReset);
            ewC = new EventWaitHandle(false, EventResetMode.AutoReset);
            ewLR = new EventWaitHandle(false, EventResetMode.AutoReset);
            ewLL = new EventWaitHandle(false, EventResetMode.AutoReset);
            ewRT = new EventWaitHandle(false, EventResetMode.AutoReset);
            ewRR = new EventWaitHandle(false, EventResetMode.AutoReset);
            ewRD = new EventWaitHandle(false, EventResetMode.AutoReset);
            ewRL = new EventWaitHandle(false, EventResetMode.AutoReset);
            Stop = true;
            StopC = true;
            StopL = true;
            StopR = true;
            XLR = XL;
            XLL = XL;

            XRTR = XR;
            YRRD = YR;
            XRDL = XR + Rlen;
            YRLT = YR + Rlen;

            g = CreateGraphics();
            gLL = CreateGraphics();
            gLR = CreateGraphics();
            gRT = CreateGraphics();
            gRR = CreateGraphics();
            gRD = CreateGraphics();
            gRL = CreateGraphics();

        }

        void ThreadMethod(int value)
        {
            Action<int> act = new Action<int>((v) =>
            {
                labelC1.Text = v.ToString();
            });

            int i = 0;
            while (!StopC && i <= value)
            {
                if (PauseC)
                {
                    ewC.WaitOne();
                    PauseC = false;
                }
                i += 1;
                if (labelC1.InvokeRequired)
                    labelC1.Invoke(new MethodInvoker(() =>
                    {
                        act(i);
                    }));
                else
                    act(i);
                Thread.Sleep(500);
            }
        }

        private void button9_Click(object sender, EventArgs e) // Play Counter
        {
            if (StopC)
            {
                buttonC2.Enabled = buttonC3.Enabled = true;
                buttonC1.Enabled = false;
                StopC = false;
                ThreadPool.QueueUserWorkItem(new WaitCallback((s) =>
                {
                    ThreadMethod(100);
                }));
            }
            else
            {
                ewC.Set();
                buttonC3.Enabled = true;
            }
            buttonC1.Enabled = false;
        }

        private void button8_Click(object sender, EventArgs e) // Pause Counter
        {
            PauseC = true;
            buttonC3.Enabled = false;
            buttonC1.Enabled = true;
        }

        private void button4_Click_1(object sender, EventArgs e) // Stop Counter
        {
            StopC = true;
            buttonC2.Enabled = false;
            buttonC1.Enabled = true;
        }


        private void button3_Click(object sender, EventArgs e) // Play Rectangle
        {
            if (StopR)
            {
                buttonR2.Enabled = buttonR3.Enabled = true;
                buttonR1.Enabled = false;
                StopR = false;

                //Top
                thR[0] = new Thread(() =>
                {
                    while (!StopR && XRTR <= XR + Rlen)
                    {
                        if (PauseR)
                        {
                            ewRT.WaitOne();
                            PauseR = false;
                        }
                        Thread.Sleep(50);

                        gRT.DrawLine(pnRT, new Point(XR, YR), new Point(XRTR, YR));
                        XRTR++;

                    }
                });

                //right
                thR[1] = new Thread(() =>
                {
                    while (!StopR && YRRD <= YR + Rlen)
                    {
                        if (PauseR)
                        {
                            ewRR.WaitOne();
                            PauseR = false;
                        }
                        Thread.Sleep(50);

                        gRR.DrawLine(pnRR, new Point(XR + Rlen, YR), new Point(XR + Rlen, YRRD));
                        YRRD++;

                    }
                });

                //down
                thR[2] = new Thread(() =>
                {
                    while (!StopR && XRDL >= XR)
                    {
                        if (PauseR)
                        {
                            ewRD.WaitOne();
                            PauseR = false;
                        }
                        Thread.Sleep(50);

                        gRD.DrawLine(pnRD, new Point(XR + Rlen, YR + Rlen), new Point(XRDL, YR + Rlen));
                        XRDL--;

                    }
                });

                //left
                thR[3] = new Thread(() =>
                {
                    while (!StopR && YRLT >= YR)
                    {
                        if (PauseR)
                        {
                            ewRL.WaitOne();
                            PauseR = false;
                        }
                        Thread.Sleep(50);

                        gRL.DrawLine(pnRL, new Point(XR, YR + Rlen), new Point(XR, YRLT));
                        YRLT--;

                    }
                });

                foreach (var item in thR)
                {
                    item.Start();
                }
            }
            else
            {
                ewRT.Set();
                ewRR.Set();
                ewRD.Set();
                ewRL.Set();
                buttonL3.Enabled = true;
            }
            buttonL1.Enabled = false;
        }

        private void buttonR2_Click(object sender, EventArgs e) // Rectangle Pause
        {
            PauseR = true;
            buttonR3.Enabled = false;
            buttonR1.Enabled = true;
        }

        private void buttonR3_Click(object sender, EventArgs e) // Rectangle Stop
        {
            StopR = true;
            buttonR2.Enabled = false;
            buttonR1.Enabled = true;
        }


        private void button5_Click(object sender, EventArgs e)// Play Line
        {
            if (StopL)
            {
                buttonL2.Enabled = buttonL3.Enabled = true;
                buttonL1.Enabled = false;
                StopL = false;

                //Right
                thL[0] = new Thread(() =>
                {
                    while (!StopL && XLR <= XL + 200)
                    {
                        if (PauseL)
                        {
                            ewLR.WaitOne();
                            PauseL = false;
                        }
                        Thread.Sleep(50);

                        gLR.DrawLine(pnLR, new Point(XL, YL), new Point(XLR, YL));
                        XLR++;
                    
                    }
                });

                //Left
                thL[1] = new Thread(() =>
                {
                    while (!StopL && XLL >= XL - 200)
                    {
                        if (PauseL)
                        {
                            ewLL.WaitOne();
                            PauseL = false;
                        }
                        Thread.Sleep(50);

                        gLL.DrawLine(pnLL, new Point(XLL, YL) , new Point(XL, YL));
                        XLL--;

                    }
                });

                foreach (var item in thL)
                {
                    item.Start();
                }
            }
            else
            {
                ewLL.Set();
                ewLR.Set();
                buttonL3.Enabled = true;
            }
            buttonL1.Enabled = false;
        }

        private void button6_Click(object sender, EventArgs e) // Pause Line
        {
            PauseL = true;
            buttonL3.Enabled = false;
            buttonL1.Enabled = true;
        }

        private void button7_Click(object sender, EventArgs e) // Stop Line
        {
            StopL = true;
            buttonL2.Enabled = false;
            buttonL1.Enabled = true;
        }

    }
}
