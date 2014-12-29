using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace PipeTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(RunClient));
            thread.Start();
            button2.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(RunServer));
            thread.Start();
            button1.Enabled = false;
        }

        private void EnableButton(Button btn)
        {
            if (btn.InvokeRequired)
                btn.Invoke((Action)delegate()
                {
                    btn.Enabled = true;
                });
            else
                btn.Enabled = true;
        }

        const string PIPE_NAME = "\\\\.\\pipe\\PIPE_NAME";

        private void RunServer()
        {
            IntPtr pipe = PipeCommunication.Pipe.CreateNamedPipe(
                PIPE_NAME, 
                (uint)PipeCommunication.PipeOpenModeFlags.PIPE_ACCESS_DUPLEX, 
                (uint)(PipeCommunication.PipeModeFlags.PIPE_TYPE_BYTE | PipeCommunication.PipeModeFlags.PIPE_READMODE_BYTE),
                1, 512, 512, 0, IntPtr.Zero);

            if (pipe == PipeCommunication.Pipe.INVALID_HANDLE)
            {
                MessageBox.Show("Create pipe failed!");
                EnableButton(button1);
                return;
            }

            Int32 value = 0;
            byte[] bytes = BitConverter.GetBytes(value);//new byte[sizeof(int)];//Buffer.BlockCopy()
            uint bytesWrittenOrRed = 0;
            NativeOverlapped nativeOverlapped = new NativeOverlapped();
            PipeCommunication.Pipe.ConnectNamedPipe(pipe, ref nativeOverlapped);
            PipeCommunication.Pipe.WriteFile(pipe, bytes, (uint)(sizeof(byte) * bytes.Length), bytesWrittenOrRed, ref nativeOverlapped);
            try
            {
                while (value < 10)
                {
                    //omit to Clear bytes
                    PipeCommunication.Pipe.ReadFile(pipe, bytes, (uint)(sizeof(byte) * bytes.Length), bytesWrittenOrRed, ref nativeOverlapped);
                    value = BitConverter.ToInt32(bytes, 0);
                    value++;
                    bytes = BitConverter.GetBytes(value);
                    Thread.Sleep(300);
                    PipeCommunication.Pipe.WriteFile(pipe, bytes, (uint)(sizeof(byte) * bytes.Length), bytesWrittenOrRed, ref nativeOverlapped);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                EnableButton(button1);
                return;
            }

            PipeCommunication.Pipe.CloseHandle(pipe);
            EnableButton(button1);
        }

        private void RunClient()
        {
            IntPtr pipe = PipeCommunication.Pipe.CreateFile(
                PIPE_NAME,
                (uint)(PipeCommunication.DesireMode.GENERIC_READ | PipeCommunication.DesireMode.GENERIC_WRITE),
                0, IntPtr.Zero, 3, 128, IntPtr.Zero);

            if (pipe == PipeCommunication.Pipe.INVALID_HANDLE)
            {
                MessageBox.Show("Connect pipe failed!");
                EnableButton(button2);
                return;
            }

            Int32 value = 0;
            byte[] bytes = BitConverter.GetBytes(value);//new byte[sizeof(int)];//Buffer.BlockCopy()
            uint bytesWrittenOrRed = 0;
            NativeOverlapped nativeOverlapped = new NativeOverlapped();
            try
            {
                while (value < 10)
                {
                    //omit to Clear bytes
                    PipeCommunication.Pipe.ReadFile(pipe, bytes, (uint)(sizeof(byte) * bytes.Length), bytesWrittenOrRed, ref nativeOverlapped);
                    value = BitConverter.ToInt32(bytes, 0);
                    value++;
                    bytes = BitConverter.GetBytes(value);
                    Thread.Sleep(300);
                    PipeCommunication.Pipe.WriteFile(pipe, bytes, (uint)(sizeof(byte) * bytes.Length), bytesWrittenOrRed, ref nativeOverlapped);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                EnableButton(button2);
                return;
            }

            PipeCommunication.Pipe.CloseHandle(pipe);
            EnableButton(button2);
        }
    }
}
