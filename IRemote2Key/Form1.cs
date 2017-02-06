using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace TestRemote
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] PortList = SerialPort.GetPortNames();
            comboBox1.Items.Clear();

            foreach (string PortName in PortList)
                comboBox1.Items.Add(PortName);
            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                button1.Text = "Connect";
            } else
            {
                serialPort1.PortName = comboBox1.SelectedItem.ToString();
                serialPort1.BaudRate = 9600;

                serialPort1.DataBits = 8;
                serialPort1.Parity = Parity.None;
                serialPort1.StopBits = StopBits.One;

                serialPort1.Handshake = Handshake.None;
                serialPort1.Encoding = Encoding.UTF8;

                try
                {
                    serialPort1.Open();
                    button1.Text = "Disconnect";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                string receivedData = serialPort1.ReadLine();
                if (!string.IsNullOrEmpty(receivedData))
                {
                    Console.Write(receivedData);
                }
            }
        }
    }
}
