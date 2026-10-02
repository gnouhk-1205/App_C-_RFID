using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace App_GFID
{
    public partial class Form1 : Form
    {
        int k = 0;
        string TransmitData = string.Empty;
        string ReceiveData = string.Empty;
        public Form1()
        {
            InitializeComponent();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            Serial_Port.PortName = "Select COM Port....";
            Serial_Port.BaudRate = 9600;
            Serial_Port.DataBits = 8;
            Serial_Port.Parity = Parity.None;
            Serial_Port.StopBits = StopBits.One;
            button_ON_GFID.Enabled = false;
            button_OFF_GFID.Enabled = false;
            button_Test.Enabled = false;
            button5.Enabled = false;
            String[] ports = SerialPort.GetPortNames();
            foreach (String port in ports)
            {
                comboBox_COMPort.Items.Add(port);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(comboBox_COMPort.Text))
            {
                MessageBox.Show("Select COM PORT.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (Serial_Port.IsOpen)
                {
                    MessageBox.Show("COM Port is connected and ready for use.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    button5.Enabled = true;
                    button_Test.Enabled = true;
                    button_ON_GFID.Enabled = true;
                    button_OFF_GFID.Enabled = true;
                    Serial_Port.PortName = comboBox_COMPort.Text;
                    Serial_Port.BaudRate = 9600;
                    Serial_Port.Open();
                    MessageBox.Show(comboBox_COMPort.Text + " is connected.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBox_Status.BackColor = Color.Lime;
                    textBox_Status.Text = "Connected";
                    comboBox_COMPort.Enabled = false;
                    ReceiveData = String.Empty;
                    TransmitData = String.Empty;
                }
            }
            catch (Exception)
            {
                textBox_Status.BackColor = Color.Red;
                textBox_Status.Text = "Disconnected!";
                MessageBox.Show("COM Port is not found. Please check your COM or cable.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (Serial_Port.IsOpen)
                {
                    button5.Enabled = false;
                    button_Test.Enabled = false;
                    button_ON_GFID.Enabled = false;
                    button_OFF_GFID.Enabled = false;
                    Serial_Port.Close();
                    textBox_Status.BackColor = Color.Red;
                    textBox_Status.Text = "Disconnected!";
                    MessageBox.Show("COM PORT is disconnected.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    comboBox_COMPort.Enabled = true;
                }
                else
                {
                    MessageBox.Show("COM Port have been disconnected. Please reconnect to use.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Disconnection appears error.Unale to disconnect.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button_Exit_Click(object sender, EventArgs e)
        {
            DialogResult answer = MessageBox.Show("Do you want to exit the program?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (answer == DialogResult.Yes)
            {
                if (Serial_Port.IsOpen)
                {
                    Serial_Port.Close();
                }
                this.Close();
            }
        }
        public void Send_Data(string Send_Text)
        {
            Serial_Port.Write(Send_Text);
            textBox_DataSend.Text = Send_Text.ToString();
        }
        private void Form_SampleCOM_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Serial_Port.IsOpen)
                Serial_Port.Close();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (Serial_Port.IsOpen)
                {
                    TransmitData = textBox_DataSend.Text.ToString();
                    Send_Data(TransmitData);
                }
                else
                {
                    textBox_Status.BackColor = Color.Red;
                    textBox_Status.Text = "Disconnected!";
                }
            }
            catch (Exception)
            {
                MessageBox.Show("The control appears error. Action can not be completed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DialogResult answer = MessageBox.Show("Do you want to exit the program?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (answer == DialogResult.Yes)
            {
                if (Serial_Port.IsOpen)
                {
                    Serial_Port.Close();
                }
                this.Close();
            }
        }

        private void comboBox_COMPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            Serial_Port.Close();
            textBox_Status.BackColor = Color.Red;
            textBox_Status.Text = "Disconnect!";
            Serial_Port.PortName = comboBox_COMPort.Text;
        }
        private string rfidBuffer = string.Empty;
        private int testCount = 0;
        private void Serial_Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;

            try
            {
                string incoming = Serial_Port.ReadExisting();
                foreach (char c in incoming)
                {
                    if (c == 'A')
                    {
                        testCount++;
                        textBox_TestReceive.AppendText($"[{testCount}] A{Environment.NewLine}");
                    }
                }
                rfidBuffer += incoming;

                while (rfidBuffer.Contains("@") && rfidBuffer.Contains("&"))
                {
                    int start = rfidBuffer.IndexOf('@');
                    int end = rfidBuffer.IndexOf('&');

                    if (start < end)
                    {
                        string packet = rfidBuffer.Substring(start, end - start + 1);
                        rfidBuffer = rfidBuffer.Substring(end + 1);

                        k++;
                        textBox_receive.AppendText($"[{k}] {packet}{Environment.NewLine}");

                        if (packet == "@Rrf_on&")
                        {
                            pictureBox2.BackColor = Color.Red;
                        }
                        else if (packet == "@Rrf_of&")
                        {
                            pictureBox2.BackColor = Color.Black;
                        }
                    }
                    else
                    {
                        rfidBuffer = rfidBuffer.Substring(start);
                    }
                }
                if (!rfidBuffer.Contains("@") && rfidBuffer.Length >= 8)
                {
                    string rfidVal = rfidBuffer.Replace("A", "").Trim();
                    if (!string.IsNullOrEmpty(rfidVal))
                    {
                        k++;
                        textBox_receive.AppendText($"[RFID]: {rfidVal}{Environment.NewLine}");
                    }
                    rfidBuffer = string.Empty;
                }
            }
            catch (Exception)
            {
            }
        }
        
        private void button_ON_GFID_Click(object sender, EventArgs e)
        {
            try
            {
                if (Serial_Port.IsOpen)
                {
                    TransmitData = "@rfi_on&";
                    Send_Data(TransmitData);
                }
                else
                {
                    textBox_Status.BackColor = Color.Red;
                    textBox_Status.Text = "Disconnected!";
                    MessageBox.Show("COM Port is not connected. Please reconnect to use.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("The control appears error. Action can not be completed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox_receive.Clear();
            textBox_TestReceive.Clear();
            k = 0;
            
        }

        private void textBox_receive_TextChanged(object sender, EventArgs e)
        {

        }

        private void button_OFF_GFID_Click(object sender, EventArgs e)
        {
            try
            {
                if (Serial_Port.IsOpen)
                {
                    TransmitData = "@rfi_of&";
                    Send_Data(TransmitData);
                }
                else
                {
                    textBox_Status.BackColor = Color.Red;
                    textBox_Status.Text = "Disconnected!";
                    MessageBox.Show("COM Port is not connected. Please reconnect to use.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("The control appears error. Action can not be completed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void button_Test_Click(object sender, EventArgs e)
        {
            button_Test.Enabled = false;

            try
            {
                string t = "@testap&";

                for (int i = 1; i <= 1000; i++)
                {
                    if (!Serial_Port.IsOpen) break;
                    Serial_Port.Write(t);
                    textBox_DataSend.Text = $"{i}: {t}";
                    Application.DoEvents();
                    await Task.Delay(2);
                }
                testCount = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                button_Test.Enabled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(Serial_Port.IsOpen)
            { 
                
            
            
            
            }    
            





        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
    
}