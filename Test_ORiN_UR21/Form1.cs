using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CAOLib;

namespace Test_ORiN_UR21
{
    public partial class Form1 : Form
    {
        CaoEngine engine;
        CaoWorkspace workspace;
        CaoController controller;
        

        public Form1()
        {
            InitializeComponent();

            engine = new CaoEngine();
            workspace = engine.Workspaces.Item(0);

            btnDisconnect.Enabled = btnRead.Enabled = false;
            btnConnect.Enabled = true;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                controller = workspace.AddController(txtController.Text.Trim(), "CaoProv.DENSO.UR20", "", "conn=COM:" + txtCOM.Text.Trim());

                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;

                btnRead.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error");
            }            
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                workspace.Controllers.Remove(controller.Index);
                controller = null;

                engine.Workspaces.Remove(workspace.Index);
                workspace = null;

                engine = null;

                btnConnect.Enabled = true;

                btnRead.Enabled = btnDisconnect.Enabled = false;

                btnConnect.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }           
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            lstTags.Items.Clear();
            readTags2();
        }

        private void readTags2()
        {
            try
            {
                object[] tags = controller.Execute("ReadUii", false);
                if (tags != null && tags.Length > 0)
                {
                    Console.WriteLine("Tags Read: " + tags.Length);
                    lblTotal.Text = tags.Length.ToString();

                    for (int iTag = 0; iTag < tags.Length; iTag++)
                    {
                        // PC data.
                        object[] tag = tags[iTag] as object[];
                        Console.WriteLine("PC Data:" + tag[0].ToString());

                        // Get UII Data
                        byte[] uiiBytes = tag[1] as byte[];
                        Console.WriteLine("Uii: " + BitConverter.ToString(uiiBytes).Replace("-", ""));

                        lstTags.Items.Add(BitConverter.ToString(uiiBytes).Replace("-", ""));
                    }
                }
                else
                {
                    lblTotal.Text = "-";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void readTags1()
        {
            try
            {
                var tags = controller.Execute("ReadUii", false);
                if(tags != null)
                {
                    long items = (tags as Array).GetUpperBound(0);
                    for (long item = 0; item < items; item++)
                    {
                        // PC data.
                        long pcData = tags[item][0];
                        Console.WriteLine("PC Data:" + pcData.ToString());

                        // Get UII Data
                        byte[] uiiBytes = tags[item][1];

                        // Uii Data Size.
                        long uiiSize = (uiiBytes as Array).GetUpperBound(0);
                        for (long j = 0; j < uiiSize; j++)
                        {
                            byte value = uiiBytes[j];
                            Console.WriteLine("Byte: " + value);
                        }

                        Console.WriteLine(BitConverter.ToString(uiiBytes).Replace("-", ""));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
