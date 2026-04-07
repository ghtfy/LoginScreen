using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoginScreen
{
    public partial class Form1 : Form
    {
        private const int MaxFailCount = 2;
        private static readonly TimeSpan LockDuration = TimeSpan.FromSeconds(30);
        private readonly Random _random = new Random();

        private int _failCount = 0;
        private DateTime? _lockedUntil;
        private bool _needSecondStep = false;

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
                txtPW.UseSystemPasswordChar = !chkShowPW.Checked;
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
            if (IsLocked())
            {
                var remain = (_lockedUntil.Value - DateTime.Now).TotalSeconds;
                lblErrorMsg.Text = "로그인 제한 중입니다. " + Math.Ceiling(remain) + "초 후 다시 시도하세요.";
                lblErrorMsg.Visible = true;
                return;
            }

            if (_needSecondStep)
            {
                if (!RunSecondStepVerification())
                {
                    lblErrorMsg.Text = "2단계 확인 실패";
                    lblErrorMsg.Visible = true;
                    return;
                }
                _needSecondStep = false;
            }

            string inputID = txtID.Text;
            string inputPW = txtPW.Text;

            if (inputID == myID && inputPW == myPW)
            {
                _failCount = 0;
                _lockedUntil = null;
                _needSecondStep = false;
                lblErrorMsg.Visible = false;
                MessageBox.Show("로그인성공!");
                return;
            }

            if (!ValidateInputs(inputID, inputPW, out string msg))
            {
                HandleFailure(msg);
                return;
            }

            HandleFailure("아이디 또는 비밀번호가 잘못 되었습니다.");
        }

        private bool ValidateInputs(string id, string pw, out string message)
        {
            message = string.Empty;

            if (id == "아이디" || string.IsNullOrWhiteSpace(id) || pw == "패스워드" || string.IsNullOrWhiteSpace(pw))
            {
                message = "아이디와 비밀번호를 입력하세요.";
                return false;
            }

            if (!Regex.IsMatch(id, "^[a-zA-Z0-9_]{4,20}$"))
            {
                message = "아이디는 영문/숫자/_만 입력 가능합니다.";
                return false;
            }

            if (pw.Contains(" "))
            {
                message = "비밀번호에 공백은 사용할 수 없습니다.";
                return false;
            }

            if (!Regex.IsMatch(pw, "[a-zA-Z]"))
            {
                message = "비밀번호에는 영문이 1개 이상 필요합니다.";
                return false;
            }

            if (!Regex.IsMatch(pw, "[0-9!@#$%^&*]"))
            {
                message = "비밀번호에는 숫자 또는 특수문자가 1개 이상 필요합니다.";
                return false;
            }

            return true;
        }

        private void HandleFailure(string message)
        {
            _failCount++;

            if (_failCount >= MaxFailCount)
            {
                _lockedUntil = DateTime.Now.Add(LockDuration);
                _needSecondStep = true;
                lblErrorMsg.Text = "실패 " + MaxFailCount + "회: 30초 후 2단계 확인 필요";
            }
            else
            {
                int remain = MaxFailCount - _failCount;
                lblErrorMsg.Text = message + " (남은 횟수: " + remain + ")";
            }

            lblErrorMsg.Visible = true;
        }

        private bool IsLocked()
        {
            return _lockedUntil.HasValue && DateTime.Now < _lockedUntil.Value;
        }

        private bool RunSecondStepVerification()
        {
            string code = GenerateVerificationCode();
            using (var verifyForm = new Form())
            using (var lblGuide = new Label())
            using (var txtCode = new TextBox())
            using (var btnOk = new Button())
            using (var btnCancel = new Button())
            {
                verifyForm.Text = "2단계 확인";
                verifyForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                verifyForm.StartPosition = FormStartPosition.CenterParent;
                verifyForm.ClientSize = new Size(330, 140);
                verifyForm.MaximizeBox = false;
                verifyForm.MinimizeBox = false;
                verifyForm.AcceptButton = btnOk;
                verifyForm.CancelButton = btnCancel;

                lblGuide.AutoSize = true;
                lblGuide.Location = new Point(20, 20);
                lblGuide.Text = "보안코드 입력: " + code;

                txtCode.Location = new Point(20, 50);
                txtCode.Width = 280;

                btnOk.Text = "확인";
                btnOk.Location = new Point(140, 90);
                btnOk.DialogResult = DialogResult.OK;

                btnCancel.Text = "취소";
                btnCancel.Location = new Point(225, 90);
                btnCancel.DialogResult = DialogResult.Cancel;

                verifyForm.Controls.Add(lblGuide);
                verifyForm.Controls.Add(txtCode);
                verifyForm.Controls.Add(btnOk);
                verifyForm.Controls.Add(btnCancel);

                var result = verifyForm.ShowDialog(this);
                return result == DialogResult.OK && string.Equals(txtCode.Text, code, StringComparison.OrdinalIgnoreCase);
            }
        }

        private string GenerateVerificationCode()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var sb = new StringBuilder(6);
            for (int i = 0; i < 6; i++)
            {
                sb.Append(chars[_random.Next(chars.Length)]);
            }
            return sb.ToString();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtID.Text = "아이디";
            txtID.ForeColor = Color.Silver;

            txtPW.UseSystemPasswordChar = false;
            txtPW.Text = "패스워드";
            txtPW.ForeColor = Color.Silver;

            chkShowPW.Checked = false;
            lblErrorMsg.Visible = false;
            txtID.Focus();
        }

        private void chkShowPW_CheckedChanged(object sender, EventArgs e)
        {
            if (txtPW.Text == "패스워드" || string.IsNullOrWhiteSpace(txtPW.Text))
            {
                txtPW.UseSystemPasswordChar = false;
                return;
            }

            txtPW.UseSystemPasswordChar = !chkShowPW.Checked;
        }

        private void txtID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // 기본 비프음 방지
                txtPW.Focus(); // 패스워드 입력창이 포커스를 갖게 끔
            }
        }

        private void txtPW_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // 기본비프음방지
                btnLogin.PerformClick(); // 버튼이 눌린 것처럼 만들기
            }
        }
    }
}
