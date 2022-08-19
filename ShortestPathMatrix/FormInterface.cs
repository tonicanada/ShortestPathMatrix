using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

namespace ShortestPathMatrix
{
    public partial class FormInterface : Form
    {
        private Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
        

        public FormInterface()
        {
            InitializeComponent();
            this.acDoc.LockDocument();
            
        }

        private void FormInterface_Load(object sender, EventArgs e)
        {

        }

        private void btnInsertSampleGraph_Click(object sender, EventArgs e)
        {
            //Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            //acDoc.LockDocument();
            Database acCurDb = this.acDoc.Database;
            SampleGraphLoader.InsertSampleGraph(this.acDoc, acCurDb, "graph1", "circle");
        }

        private void btnOutputFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.textBoxOutputFolder.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void btnGenerateShortestPathMatrix_Click(object sender, EventArgs e)
        {
            //Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            //this.acDoc.LockDocument();
            Database acCurDb = this.acDoc.Database;
            try
            {
                if (this.textBoxOutputFolder.Text == "")
                {
                    throw new Exception("Please select the output folder where CSV files will be saved");
                }
                else
                {
                    Dijkstra.GenerateShortestPathMatrix(acCurDb, this.textBoxOutputFolder.Text);
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
    }
}
