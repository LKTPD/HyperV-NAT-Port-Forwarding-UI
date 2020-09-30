using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HyperV_NAT_Port_Forwarding_UI
{
    public partial class FormMain : Form
    {
        DataTable dtConfig = new DataTable();
        public FormMain()
        {
            InitializeComponent();

            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Normal;


            dtConfig.Columns.Add(new DataColumn("HostIP", typeof(string)));
            dtConfig.Columns.Add(new DataColumn("HostPort", typeof(string)));
            dtConfig.Columns.Add(new DataColumn("VMIP", typeof(string)));
            dtConfig.Columns.Add(new DataColumn("VMPort", typeof(string)));
            dtConfig.Columns.Add(new DataColumn("Delete", typeof(string)));

            loadConfig();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            typeof(DataGridView).InvokeMember(
               "DoubleBuffered",
               BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
               null,
               this.dataGridView1,
               new object[] { true });
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            loadConfig();
        }



        void loadConfig()
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c netsh interface portproxy show v4tov4",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            StringBuilder sb = new StringBuilder();
            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine();
                sb.AppendLine(line.Trim());
            }

            var lines = sb.ToString().Trim().Split(new char[] { '\n' }).ToList();

            if (lines.Count > 3)
            {
                dtConfig.Rows.Clear();
                for (var i = 4; i < lines.Count; i++)
                {
                    var line = lines[i].Split(new char[] { ' ' });
                    List<string> line2 = new List<string>();
                    for (var j = 0; j < line.Length; j++)
                    {
                        if (line[j].Trim() != "")
                        {
                            line2.Add(line[j]);
                        }
                    }

                    var drNew = dtConfig.NewRow();
                    drNew["HostIP"] = line2[0];
                    drNew["HostPort"] = line2[1];
                    drNew["VMIP"] = line2[2];
                    drNew["VMPort"] = line2[3];
                    drNew["Delete"] = "Delete";


                    dtConfig.Rows.Add(drNew);
                }
                this.dataGridView1.DataSource = dtConfig;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            addConfig();
        }
        
        void addConfig()
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = string.Format("/c netsh interface portproxy add v4tov4 listenport={0} listenaddress={1} connectaddress={2} connectport={3}", this.txtHostPort.Text.Trim(), this.txtHostIP.Text.Trim(), this.txtVMIP.Text.Trim(), this.txtVMPort.Text.Trim()),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            StringBuilder sb = new StringBuilder();
            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine();
                sb.AppendLine(line.Trim());
            }

            loadConfig();

            MessageBox.Show("NAT Port Forwarding Added");

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            removeConfig(e);
        }

        void removeConfig(DataGridViewCellEventArgs e)
        {
            if (this.dataGridView1.Columns[e.ColumnIndex].Name == "Delete")
            {
                var row = this.dataGridView1.Rows[e.RowIndex];

                var hostIP = row.Cells[0].Value.ToString();
                var hostPort = row.Cells[1].Value.ToString();

                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = string.Format("/c netsh interface portproxy delete v4tov4 listenaddress={0} listenport={1}", hostIP, hostPort),
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                proc.Start();
                StringBuilder sb = new StringBuilder();
                while (!proc.StandardOutput.EndOfStream)
                {
                    string line = proc.StandardOutput.ReadLine();
                    sb.AppendLine(line.Trim());
                }

                loadConfig();

                MessageBox.Show("NAT Port Forwarding Deleted!");
            }
        }
    }
}
