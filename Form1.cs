using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoginScreen
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void txtID_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtID_Enter(object sender, EventArgs e)
        {
            if (txtID.Text == "아이디")
            {
                txtID.Text = ""; 
                txtID.ForeColor = Color.Black;
            }
        }
        private void txtID_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtID.Text))
            { 
                txtID.Text = "아이디"; 
                txtID.ForeColor = Color.Silver; 
            }
        }

        private void txtPW_Enter(object sender, EventArgs e)
        {
            if (txtPW.Text == "패스워드")
            {
                txtPW.Text = "";
                txtPW.ForeColor = Color.Black;
                txtPW.UseSystemPasswordChar = true;
            }
        }
        private void txtPW_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPW.Text)) 
            {
                txtPW.UseSystemPasswordChar = false;
                txtPW.Text = "패스워드"; 
                txtPW.ForeColor = Color.Silver; 
            }
        }
        string myID = "admin";
        string myPW = "superman";

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string inputID = txtID.Text; 
            string inputPW = txtPW.Text; 
            
            if (inputID == myID && inputPW == myPW) { 
                MessageBox.Show("로그인성공!"); 
            }
            else {
                MessageBox.Show("로그인실패~"); 
            }
        }
    }
}
