﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using eft_dma_radar.UI.Radar;

namespace LonesEFTRadar.UI.SKWidgetControl
{
    public partial class SettingsWidgetForm : Form
    {
        private bool isMinimized = false;
        private Point lastMousePosition;
        private MainForm _mainForm;

        public SettingsWidgetForm(MainForm mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
            UpdateCheckboxStates();
        }

        private void minimizeButton_Click(object sender, EventArgs e)
        {
            if (isMinimized)
            {
                this.Size = new System.Drawing.Size(400, 300);
                this.contentPanel.Visible = true;
                this.minimizeButton.Text = "-";
            }
            else
            {
                this.Size = new System.Drawing.Size(400, 20);
                this.contentPanel.Visible = false;
                this.minimizeButton.Text = "+";
            }
            isMinimized = !isMinimized;
        }

        private void headerPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                lastMousePosition = e.Location;
            }
        }

        private void headerPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(
                    this.Location.X + e.X - lastMousePosition.X,
                    this.Location.Y + e.Y - lastMousePosition.Y);
            }
        }

        private void button_Restart_SettingsWidget_Click(object sender, EventArgs e)
        {
            MainForm.button_Restart_Click(sender, e);
        }

        public void UpdateCheckboxStates()
        {
            checkBox_MoveSpeed_SettingsWidget.Checked = _mainForm.checkBox_MoveSpeed.Checked;
            checkBox_MoveSpeed2_SettingsWidget.Checked = _mainForm.checkBox_MoveSpeed2.Checked;
        }

        public void UpdateMoveSpeedCheckbox(bool isChecked)
        {
            checkBox_MoveSpeed_SettingsWidget.Checked = isChecked;
        }

        public void UpdateMoveSpeed2Checkbox(bool isChecked)
        {
            checkBox_MoveSpeed2_SettingsWidget.Checked = isChecked;
        }

        private void checkBox_MoveSpeed_SettingsWidget_CheckedChanged(object sender, EventArgs e)
        {
            _mainForm.checkBox_MoveSpeed.Checked = checkBox_MoveSpeed_SettingsWidget.Checked;
        }

        private void checkBox_MoveSpeed2_SettingsWidget_CheckedChanged(object sender, EventArgs e)
        {
            _mainForm.checkBox_MoveSpeed2.Checked = checkBox_MoveSpeed2_SettingsWidget.Checked;
        }

        private void button_GymHack_SettingsWidget_Click(object sender, EventArgs e)
        {
            _mainForm.button_GymHack_Click(sender, e);
        }
    }
}
