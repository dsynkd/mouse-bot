using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace MouseBot
{
    public partial class Mouse_Bot : Form
    {
        List<int> commands = new List<int>();
        List<int> xs = new List<int>();
        List<int> ys = new List<int>();
        List<int> times = new List<int>();
        public Mouse_Bot()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
        static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        private const int LEFTDOWN = 0x2;
        private const int LEFTUP = 0x4;
        private const int RIGHTDOWN = 0x0008;
        private const int RIGHTUP = 0x0010;

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
            public static implicit operator System.Drawing.Point(POINT p)
            {
                return new System.Drawing.Point(p.X, p.Y);
            }
            public static implicit operator POINT(System.Drawing.Point p)
            {
                return new POINT(p.X, p.Y);
            }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(String sClassName, String sAppName);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            POINT xy;
            GetCursorPos(out xy);
            textBox3.Text = xy.X.ToString();
            textBox4.Text = xy.Y.ToString();
        }

        public void MouseBot()
        {
            for (int index = 0; index < lstActions.Items.Count; index++)
            {
                int xx = xs[index];
                int yy = ys[index];
                int action = commands[index];
                int xtime = times[index];
                if (action < 3)
                {
                    DoMoveMouse(xx,yy,xtime); 
                }
                switch (action)
                {
                    case 1:
                        DoMouseClick(xx, yy);
                        Thread.Sleep(100);
                        break;
                    case 2:
                        DoRightClick(xx, yy);
                        Thread.Sleep(100);
                        break;
                    case 3:
                        DoDrag(xx, yy,xtime);
                        Thread.Sleep(100);
                        break;
                    case 4:
                        Thread.Sleep(xtime);
                        break;
                }
            }
        }

        void DoMoveMouse(int xx, int yy, int xtime)
        {
            POINT myxy;
            GetCursorPos(out myxy);
            double disX = Math.Abs(xx - myxy.X);
            double disY = Math.Abs(yy - myxy.Y);
            double dstime = Double.Parse((disX > disY ? xtime / disX : xtime / disY).ToString());
            double stime = Math.Ceiling(dstime);
            double divtime = xtime / double.Parse(stime.ToString());
            double stepy = disY / divtime;
            double stepx = disX / divtime;
            double fx = myxy.X, fy = myxy.Y;
            if (myxy.X < xx && myxy.Y > yy)
            {
                while (fx < xx && fy > yy)
                {
                    SetCursorPos(int.Parse(Math.Ceiling(fx).ToString()), int.Parse(Math.Ceiling(fy).ToString()));
                    fx += stepx;
                    fy -= stepy;
                    Thread.Sleep(int.Parse(stime.ToString()));
                }
            }
            else if (myxy.X < xx && myxy.Y < yy)
            {
                while (fx < xx && fy < yy)
                {
                    SetCursorPos(int.Parse(Math.Ceiling(fx).ToString()), int.Parse(Math.Ceiling(fy).ToString()));
                    fx += stepx;
                    fy += stepy;
                    Thread.Sleep(int.Parse(stime.ToString()));
                }
            }
            else if (myxy.X > xx && myxy.Y < yy)
            {
                while (fx > xx && fy < yy)
                {
                    SetCursorPos(int.Parse(Math.Ceiling(fx).ToString()), int.Parse(Math.Ceiling(fy).ToString()));
                    fx -= stepx;
                    fy += stepy;
                    Thread.Sleep(int.Parse(stime.ToString()));
                }
            }
            else if (myxy.X > xx && myxy.Y > yy)
            {
                while (fx > xx && fy > yy)
                {
                    SetCursorPos(int.Parse(Math.Ceiling(fx).ToString()), int.Parse(Math.Ceiling(fy).ToString()));
                    fx -= stepx;
                    fy -= stepy;
                    Thread.Sleep(int.Parse(stime.ToString()));
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if(checkBox1.Checked){
                    for (int lx = 0; lx < int.Parse(textBox5.Text); lx++)
                    {
                        MouseBot();
                    }
            }else{
                MouseBot();
            }
        }

        public void DoMouseClick(int xpos, int ypos)
        {
            mouse_event(LEFTDOWN, xpos, ypos, 0, 0);
            Thread.Sleep(20);
            mouse_event(LEFTUP, xpos, ypos, 0, 0);
        }

        public void DoRightClick(int xpos, int ypos)
        {
            mouse_event(RIGHTDOWN, xpos, ypos, 0, 0);
            mouse_event(RIGHTUP, xpos, ypos, 0, 0);
        }

        public void DoDrag(int xpos, int ypos, int dur)
        {
            POINT cxy;
            GetCursorPos(out cxy);
            mouse_event(LEFTDOWN, cxy.X, cxy.Y, 0, 0);
            Application.DoEvents();
            Thread.Sleep(50);
            DoMoveMouse(xpos, ypos, dur);
            Application.DoEvents();
            Thread.Sleep(50);
            mouse_event(LEFTUP, cxy.X, cxy.Y, 0, 0);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox5.Enabled = true;
            }
            else
            {
                textBox5.Enabled = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedIndex == 4)
                {
                    lstActions.Items.Add(comboBox1.SelectedItem.ToString() + " For " + txtDuration.Text + " Miliseconds");
                    times.Add(int.Parse(txtDuration.Text));
                    ys.Add(1);
                    xs.Add(1);
                    commands.Add(comboBox1.SelectedIndex);
                }
                else
                {
                    lstActions.Items.Add(comboBox1.SelectedItem.ToString() + " On " + txtXpos.Text + "," + txtYpos.Text + " During "+txtDuration.Text + " Miliseconds");
                    times.Add(int.Parse(txtDuration.Text));
                    xs.Add(int.Parse(txtXpos.Text=="0" ? "1" : txtXpos.Text));
                    ys.Add(int.Parse(txtYpos.Text == "0" ? "1" : txtYpos.Text));
                    commands.Add(comboBox1.SelectedIndex);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("One Of The Enteries Was Incorrect!\nPlease Notice That You Should Enter Integers As The Mouse Position!");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int si = lstActions.SelectedIndex;
            if (si != -1)
            {
                lstActions.Items.Remove(lstActions.SelectedItem);
                times.Remove(si);
                xs.Remove(si);
                ys.Remove(si);
                commands.Remove(si);
                lstActions.Refresh();
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            lstActions.Items.Clear();
            times.Clear();
            xs.Clear();
            ys.Clear();
            commands.Clear();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Process.Start("cmd /c http://V0R73X.net63.net");
        }

        private void cmdSetMousePos_Click(object sender, EventArgs e)
        {
            try
            {
                SetCursorPos(int.Parse(txtSetX.Text), int.Parse(txtSetY.Text));
            }
            catch (Exception err) { MessageBox.Show("Please Enter Integer Numbers!"); }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("Tutorials:\nMouse Bot accepts a series of actions as input.\nYou may set these actions by entering: \nX/Y position to which the cursor should move,\naction that should be done by the cursor,\nand the time the cursor should take to do the action. \nThese fields are all positioned on the top, right of the form, below the title.\nUse the \"Add Button\" to add the action to the series.\nUse the \"Set Mouse Position\" for setting quickly moving the cursor to an x/y coordinate.\nUse the Remove button to remove the selected action and the Clear button to remove all actions.\nClick on \"Bot My Mouse\" button to start the process!\nShortcuts:\nAlt+e: Records the current position of the cursor.\nOur plans for the next versions:\nTeach-Bot\nSend Keyboard Input\nTarget Applications\nAn escape key that stops the mouse bot process.\nBe on check for the upcoming versions!\nHope you're having fun using Mouse-Bot!", "Help", MessageBoxButtons.OK);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process.Start("cmd /c http://beez.net78.net/donations.php");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            txtXpos.Text = textBox3.Text;
            txtYpos.Text = textBox4.Text;
        }
    }
}
