﻿using SkiaSharp;
using System.Runtime;
using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;

namespace LonesEFTRadar.UI.SKWidgetControl
{
    partial class SettingsWidgetForm : System.Windows.Forms.Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            headerPanel = new Panel();
            minimizeButton = new Button();
            contentPanel = new Panel();
            button_Restart_SettingsWidget = new Button();
            checkBox_MoveSpeed_SettingsWidget = new CheckBox();
            checkBox_MoveSpeed2_SettingsWidget = new CheckBox();
            headerPanel.SuspendLayout();
            contentPanel.SuspendLayout();
            SuspendLayout();
            // 
            // headerPanel
            // 
            headerPanel.BackColor = SystemColors.Control;
            headerPanel.Controls.Add(minimizeButton);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Location = new Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new Size(400, 20);
            headerPanel.TabIndex = 0;
            headerPanel.MouseDown += headerPanel_MouseDown;
            headerPanel.MouseMove += headerPanel_MouseMove;
            // 
            // minimizeButton
            // 
            minimizeButton.Dock = DockStyle.Right;
            minimizeButton.Location = new Point(378, 0);
            minimizeButton.Name = "minimizeButton";
            minimizeButton.Size = new Size(22, 20);
            minimizeButton.TabIndex = 0;
            minimizeButton.Text = "-";
            minimizeButton.UseVisualStyleBackColor = true;
            minimizeButton.Click += minimizeButton_Click;
            // 
            // contentPanel
            // 
            contentPanel.BackColor = SystemColors.Control;
            contentPanel.Controls.Add(checkBox_MoveSpeed_SettingsWidget);
            contentPanel.Controls.Add(checkBox_MoveSpeed2_SettingsWidget);
            contentPanel.Controls.Add(button_Restart_SettingsWidget);
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.Location = new Point(0, 20);
            contentPanel.Name = "contentPanel";
            contentPanel.Size = new Size(400, 280);
            contentPanel.TabIndex = 1;
            // 
            // button_Restart_SettingsWidget
            // 
            button_Restart_SettingsWidget.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button_Restart_SettingsWidget.Location = new Point(12, 6);
            button_Restart_SettingsWidget.Name = "button_Restart_SettingsWidget";
            button_Restart_SettingsWidget.Size = new Size(88, 29);
            button_Restart_SettingsWidget.TabIndex = 19;
            button_Restart_SettingsWidget.Text = "Restart Radar";
            button_Restart_SettingsWidget.UseVisualStyleBackColor = false;
            button_Restart_SettingsWidget.Click += button_Restart_SettingsWidget_Click;
            // 
            // checkBox_MoveSpeed_SettingsWidget
            // 
            checkBox_MoveSpeed_SettingsWidget.Anchor = AnchorStyles.Right;
            checkBox_MoveSpeed_SettingsWidget.AutoSize = true;
            checkBox_MoveSpeed_SettingsWidget.Location = new Point(12, 41);
            checkBox_MoveSpeed_SettingsWidget.Name = "checkBox_MoveSpeed_SettingsWidget";
            checkBox_MoveSpeed_SettingsWidget.Size = new Size(152, 19);
            checkBox_MoveSpeed_SettingsWidget.TabIndex = 80;
            checkBox_MoveSpeed_SettingsWidget.Text = "1.2x Move Speed (Risky)";
            checkBox_MoveSpeed_SettingsWidget.UseVisualStyleBackColor = false;
            checkBox_MoveSpeed_SettingsWidget.CheckedChanged += checkBox_MoveSpeed_SettingsWidget_CheckedChanged;

            // 
            // checkBox_MoveSpeed2__SettingsWidget
            // 
            checkBox_MoveSpeed2_SettingsWidget.Anchor = AnchorStyles.Right;
            checkBox_MoveSpeed2_SettingsWidget.AutoSize = true;
            checkBox_MoveSpeed2_SettingsWidget.Location = new Point(12, 66);
            checkBox_MoveSpeed2_SettingsWidget.Name = "checkBox_MoveSpeed2_SettingsWidget";
            checkBox_MoveSpeed2_SettingsWidget.Size = new Size(152, 19);
            checkBox_MoveSpeed2_SettingsWidget.TabIndex = 81;
            checkBox_MoveSpeed2_SettingsWidget.Text = "1.4x Move Speed (Risky)";
            checkBox_MoveSpeed2_SettingsWidget.UseVisualStyleBackColor = false;
            checkBox_MoveSpeed2_SettingsWidget.CheckedChanged += checkBox_MoveSpeed2_SettingsWidget_CheckedChanged;

            // 
            // SettingsWidgetForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(400, 300);
            Controls.Add(contentPanel);
            Controls.Add(headerPanel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "SettingsWidgetForm";
            Text = "SettingsWidgetForm";
            TopMost = true;
            Load += SettingsWidgetForm_Load;
            headerPanel.ResumeLayout(false);
            contentPanel.ResumeLayout(false);
            contentPanel.PerformLayout();
            ResumeLayout(false);

        }

        private void SettingsWidgetForm_Load(object sender, EventArgs e)
        {
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            bool isDarkMode = IsDarkModeEnabled();

            Color contentBackgroundColor = isDarkMode ? Color.FromArgb(45, 45, 48) : SystemColors.ControlLightLight;
            Color headerBackgroundColor = isDarkMode ? Color.FromArgb(35, 35, 38) : SystemColors.Control;
            Color foregroundColor = isDarkMode ? Color.White : SystemColors.ControlText;

            BackColor = contentBackgroundColor;
            ForeColor = foregroundColor;

            headerPanel.BackColor = headerBackgroundColor;
            headerPanel.ForeColor = foregroundColor;
            contentPanel.BackColor = contentBackgroundColor;
            contentPanel.ForeColor = foregroundColor;
            minimizeButton.BackColor = headerBackgroundColor;
            minimizeButton.ForeColor = foregroundColor;
        }
        private bool IsDarkModeEnabled()
        {
            const string registryKey = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            const string registryValue = "AppsUseLightTheme";

            object registryValueObject = Registry.GetValue(registryKey, registryValue, null);
            if (registryValueObject == null)
            {
                return false;
            }

            int registryValueInt = (int)registryValueObject;
            return registryValueInt == 0;
        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Button minimizeButton;
        private System.Windows.Forms.Panel contentPanel;
        private Button button_Restart_SettingsWidget;
        private CheckBox checkBox_MoveSpeed_SettingsWidget;
        private CheckBox checkBox_MoveSpeed2_SettingsWidget;
    }
}