namespace MoveDialog
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonMoveDirectionUp = new System.Windows.Forms.Button();
            this.groupBoxMoveDirection = new System.Windows.Forms.GroupBox();
            this.buttonMoveDirectionDown = new System.Windows.Forms.Button();
            this.buttonMoveDirectionRight = new System.Windows.Forms.Button();
            this.buttonMoveDirectionCentor = new System.Windows.Forms.Button();
            this.buttonMoveDirectionLeft = new System.Windows.Forms.Button();
            this.checkBoxVisible = new System.Windows.Forms.CheckBox();
            this.groupBoxSetWindow = new System.Windows.Forms.GroupBox();
            this.labelWindowHeightValue = new System.Windows.Forms.Label();
            this.labelWindowHeightName = new System.Windows.Forms.Label();
            this.trackBarWindowHeight = new System.Windows.Forms.TrackBar();
            this.labelWindowWidthValue = new System.Windows.Forms.Label();
            this.labelWindowWidthName = new System.Windows.Forms.Label();
            this.trackBarWindowWidth = new System.Windows.Forms.TrackBar();
            this.labelMoveDistanceValue = new System.Windows.Forms.Label();
            this.labelMoveDistanceName = new System.Windows.Forms.Label();
            this.trackBarMoveDistance = new System.Windows.Forms.TrackBar();
            this.groupBoxSetColor = new System.Windows.Forms.GroupBox();
            this.labelWindowColorBlueValue = new System.Windows.Forms.Label();
            this.labelWindowColorBlueName = new System.Windows.Forms.Label();
            this.trackBarWindowColorBlue = new System.Windows.Forms.TrackBar();
            this.labelWindowColorGreenValue = new System.Windows.Forms.Label();
            this.labelWindowColorGreenName = new System.Windows.Forms.Label();
            this.trackBarWindowColorGreen = new System.Windows.Forms.TrackBar();
            this.labelWindowColorRedValue = new System.Windows.Forms.Label();
            this.labelWindowColorRedName = new System.Windows.Forms.Label();
            this.trackBarWindowColorRed = new System.Windows.Forms.TrackBar();
            this.groupBoxMoveDirection.SuspendLayout();
            this.groupBoxSetWindow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarWindowHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarWindowWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMoveDistance)).BeginInit();
            this.groupBoxSetColor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarWindowColorBlue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarWindowColorGreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarWindowColorRed)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonMoveDirectionUp
            // 
            this.buttonMoveDirectionUp.Location = new System.Drawing.Point(72, 29);
            this.buttonMoveDirectionUp.Name = "buttonMoveDirectionUp";
            this.buttonMoveDirectionUp.Size = new System.Drawing.Size(50, 50);
            this.buttonMoveDirectionUp.TabIndex = 0;
            this.buttonMoveDirectionUp.Text = "↑";
            this.buttonMoveDirectionUp.UseVisualStyleBackColor = true;
            this.buttonMoveDirectionUp.Click += new System.EventHandler(this.updatePosition);
            // 
            // groupBoxMoveDirection
            // 
            this.groupBoxMoveDirection.Controls.Add(this.buttonMoveDirectionDown);
            this.groupBoxMoveDirection.Controls.Add(this.buttonMoveDirectionRight);
            this.groupBoxMoveDirection.Controls.Add(this.buttonMoveDirectionCentor);
            this.groupBoxMoveDirection.Controls.Add(this.buttonMoveDirectionLeft);
            this.groupBoxMoveDirection.Controls.Add(this.buttonMoveDirectionUp);
            this.groupBoxMoveDirection.Location = new System.Drawing.Point(12, 12);
            this.groupBoxMoveDirection.Name = "groupBoxMoveDirection";
            this.groupBoxMoveDirection.Size = new System.Drawing.Size(193, 193);
            this.groupBoxMoveDirection.TabIndex = 0;
            this.groupBoxMoveDirection.TabStop = false;
            this.groupBoxMoveDirection.Text = "子ウィンドウ位置制御";
            // 
            // buttonMoveDirectionDown
            // 
            this.buttonMoveDirectionDown.Location = new System.Drawing.Point(72, 133);
            this.buttonMoveDirectionDown.Name = "buttonMoveDirectionDown";
            this.buttonMoveDirectionDown.Size = new System.Drawing.Size(50, 50);
            this.buttonMoveDirectionDown.TabIndex = 4;
            this.buttonMoveDirectionDown.Text = "↓";
            this.buttonMoveDirectionDown.UseVisualStyleBackColor = true;
            this.buttonMoveDirectionDown.Click += new System.EventHandler(this.updatePosition);
            // 
            // buttonMoveDirectionRight
            // 
            this.buttonMoveDirectionRight.Location = new System.Drawing.Point(127, 81);
            this.buttonMoveDirectionRight.Name = "buttonMoveDirectionRight";
            this.buttonMoveDirectionRight.Size = new System.Drawing.Size(50, 50);
            this.buttonMoveDirectionRight.TabIndex = 3;
            this.buttonMoveDirectionRight.Text = "→";
            this.buttonMoveDirectionRight.UseVisualStyleBackColor = true;
            this.buttonMoveDirectionRight.Click += new System.EventHandler(this.updatePosition);
            // 
            // buttonMoveDirectionCentor
            // 
            this.buttonMoveDirectionCentor.Location = new System.Drawing.Point(72, 81);
            this.buttonMoveDirectionCentor.Name = "buttonMoveDirectionCentor";
            this.buttonMoveDirectionCentor.Size = new System.Drawing.Size(50, 50);
            this.buttonMoveDirectionCentor.TabIndex = 2;
            this.buttonMoveDirectionCentor.Text = "・";
            this.buttonMoveDirectionCentor.UseVisualStyleBackColor = true;
            this.buttonMoveDirectionCentor.Click += new System.EventHandler(this.updatePosition);
            // 
            // buttonMoveDirectionLeft
            // 
            this.buttonMoveDirectionLeft.Location = new System.Drawing.Point(18, 81);
            this.buttonMoveDirectionLeft.Name = "buttonMoveDirectionLeft";
            this.buttonMoveDirectionLeft.Size = new System.Drawing.Size(50, 50);
            this.buttonMoveDirectionLeft.TabIndex = 1;
            this.buttonMoveDirectionLeft.Text = "←";
            this.buttonMoveDirectionLeft.UseVisualStyleBackColor = true;
            this.buttonMoveDirectionLeft.Click += new System.EventHandler(this.updatePosition);
            // 
            // checkBoxVisible
            // 
            this.checkBoxVisible.AutoSize = true;
            this.checkBoxVisible.Checked = true;
            this.checkBoxVisible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxVisible.Location = new System.Drawing.Point(223, 12);
            this.checkBoxVisible.Name = "checkBoxVisible";
            this.checkBoxVisible.Size = new System.Drawing.Size(150, 22);
            this.checkBoxVisible.TabIndex = 1;
            this.checkBoxVisible.Text = "子ウィンドウ表示";
            this.checkBoxVisible.UseVisualStyleBackColor = true;
            this.checkBoxVisible.CheckedChanged += new System.EventHandler(this.checkBoxVisible_CheckedChanged);
            // 
            // groupBoxSetWindow
            // 
            this.groupBoxSetWindow.Controls.Add(this.labelWindowHeightValue);
            this.groupBoxSetWindow.Controls.Add(this.labelWindowHeightName);
            this.groupBoxSetWindow.Controls.Add(this.trackBarWindowHeight);
            this.groupBoxSetWindow.Controls.Add(this.labelWindowWidthValue);
            this.groupBoxSetWindow.Controls.Add(this.labelWindowWidthName);
            this.groupBoxSetWindow.Controls.Add(this.trackBarWindowWidth);
            this.groupBoxSetWindow.Controls.Add(this.labelMoveDistanceValue);
            this.groupBoxSetWindow.Controls.Add(this.labelMoveDistanceName);
            this.groupBoxSetWindow.Controls.Add(this.trackBarMoveDistance);
            this.groupBoxSetWindow.Location = new System.Drawing.Point(223, 40);
            this.groupBoxSetWindow.Name = "groupBoxSetWindow";
            this.groupBoxSetWindow.Size = new System.Drawing.Size(249, 165);
            this.groupBoxSetWindow.TabIndex = 2;
            this.groupBoxSetWindow.TabStop = false;
            this.groupBoxSetWindow.Text = "子ウィンドウ座標制御";
            // 
            // labelWindowHeightValue
            // 
            this.labelWindowHeightValue.Location = new System.Drawing.Point(83, 119);
            this.labelWindowHeightValue.Name = "labelWindowHeightValue";
            this.labelWindowHeightValue.Size = new System.Drawing.Size(50, 18);
            this.labelWindowHeightValue.TabIndex = 7;
            this.labelWindowHeightValue.Text = "200";
            this.labelWindowHeightValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelWindowHeightName
            // 
            this.labelWindowHeightName.AutoSize = true;
            this.labelWindowHeightName.Location = new System.Drawing.Point(12, 119);
            this.labelWindowHeightName.Name = "labelWindowHeightName";
            this.labelWindowHeightName.Size = new System.Drawing.Size(47, 18);
            this.labelWindowHeightName.TabIndex = 6;
            this.labelWindowHeightName.Text = "高さ：";
            // 
            // trackBarWindowHeight
            // 
            this.trackBarWindowHeight.AutoSize = false;
            this.trackBarWindowHeight.Location = new System.Drawing.Point(135, 114);
            this.trackBarWindowHeight.Maximum = 1000;
            this.trackBarWindowHeight.Minimum = 1;
            this.trackBarWindowHeight.Name = "trackBarWindowHeight";
            this.trackBarWindowHeight.Size = new System.Drawing.Size(106, 30);
            this.trackBarWindowHeight.TabIndex = 8;
            this.trackBarWindowHeight.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarWindowHeight.Value = 200;
            this.trackBarWindowHeight.Scroll += new System.EventHandler(this.updateRectSize);
            // 
            // labelWindowWidthValue
            // 
            this.labelWindowWidthValue.Location = new System.Drawing.Point(83, 78);
            this.labelWindowWidthValue.Name = "labelWindowWidthValue";
            this.labelWindowWidthValue.Size = new System.Drawing.Size(50, 18);
            this.labelWindowWidthValue.TabIndex = 4;
            this.labelWindowWidthValue.Text = "300";
            this.labelWindowWidthValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelWindowWidthName
            // 
            this.labelWindowWidthName.AutoSize = true;
            this.labelWindowWidthName.Location = new System.Drawing.Point(12, 78);
            this.labelWindowWidthName.Name = "labelWindowWidthName";
            this.labelWindowWidthName.Size = new System.Drawing.Size(35, 18);
            this.labelWindowWidthName.TabIndex = 3;
            this.labelWindowWidthName.Text = "幅：";
            // 
            // trackBarWindowWidth
            // 
            this.trackBarWindowWidth.AutoSize = false;
            this.trackBarWindowWidth.Location = new System.Drawing.Point(135, 73);
            this.trackBarWindowWidth.Maximum = 1000;
            this.trackBarWindowWidth.Minimum = 1;
            this.trackBarWindowWidth.Name = "trackBarWindowWidth";
            this.trackBarWindowWidth.Size = new System.Drawing.Size(106, 30);
            this.trackBarWindowWidth.TabIndex = 5;
            this.trackBarWindowWidth.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarWindowWidth.Value = 300;
            this.trackBarWindowWidth.Scroll += new System.EventHandler(this.updateRectSize);
            // 
            // labelMoveDistanceValue
            // 
            this.labelMoveDistanceValue.Location = new System.Drawing.Point(83, 42);
            this.labelMoveDistanceValue.Name = "labelMoveDistanceValue";
            this.labelMoveDistanceValue.Size = new System.Drawing.Size(50, 18);
            this.labelMoveDistanceValue.TabIndex = 1;
            this.labelMoveDistanceValue.Text = "100";
            this.labelMoveDistanceValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelMoveDistanceName
            // 
            this.labelMoveDistanceName.AutoSize = true;
            this.labelMoveDistanceName.Location = new System.Drawing.Point(12, 42);
            this.labelMoveDistanceName.Name = "labelMoveDistanceName";
            this.labelMoveDistanceName.Size = new System.Drawing.Size(71, 18);
            this.labelMoveDistanceName.TabIndex = 0;
            this.labelMoveDistanceName.Text = "移動量：";
            // 
            // trackBarMoveDistance
            // 
            this.trackBarMoveDistance.AutoSize = false;
            this.trackBarMoveDistance.Location = new System.Drawing.Point(135, 37);
            this.trackBarMoveDistance.Maximum = 100;
            this.trackBarMoveDistance.Minimum = 1;
            this.trackBarMoveDistance.Name = "trackBarMoveDistance";
            this.trackBarMoveDistance.Size = new System.Drawing.Size(106, 30);
            this.trackBarMoveDistance.TabIndex = 2;
            this.trackBarMoveDistance.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarMoveDistance.Value = 100;
            this.trackBarMoveDistance.Scroll += new System.EventHandler(this.trackBarMoveDistance_Scroll);
            // 
            // groupBoxSetColor
            // 
            this.groupBoxSetColor.Controls.Add(this.labelWindowColorBlueValue);
            this.groupBoxSetColor.Controls.Add(this.labelWindowColorBlueName);
            this.groupBoxSetColor.Controls.Add(this.trackBarWindowColorBlue);
            this.groupBoxSetColor.Controls.Add(this.labelWindowColorGreenValue);
            this.groupBoxSetColor.Controls.Add(this.labelWindowColorGreenName);
            this.groupBoxSetColor.Controls.Add(this.trackBarWindowColorGreen);
            this.groupBoxSetColor.Controls.Add(this.labelWindowColorRedValue);
            this.groupBoxSetColor.Controls.Add(this.labelWindowColorRedName);
            this.groupBoxSetColor.Controls.Add(this.trackBarWindowColorRed);
            this.groupBoxSetColor.Location = new System.Drawing.Point(223, 211);
            this.groupBoxSetColor.Name = "groupBoxSetColor";
            this.groupBoxSetColor.Size = new System.Drawing.Size(249, 165);
            this.groupBoxSetColor.TabIndex = 3;
            this.groupBoxSetColor.TabStop = false;
            this.groupBoxSetColor.Text = "子ウィンドウ背景色制御";
            // 
            // labelWindowColorBlueValue
            // 
            this.labelWindowColorBlueValue.Location = new System.Drawing.Point(93, 119);
            this.labelWindowColorBlueValue.Name = "labelWindowColorBlueValue";
            this.labelWindowColorBlueValue.Size = new System.Drawing.Size(40, 18);
            this.labelWindowColorBlueValue.TabIndex = 7;
            this.labelWindowColorBlueValue.Text = "128";
            this.labelWindowColorBlueValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelWindowColorBlueName
            // 
            this.labelWindowColorBlueName.AutoSize = true;
            this.labelWindowColorBlueName.Location = new System.Drawing.Point(12, 119);
            this.labelWindowColorBlueName.Name = "labelWindowColorBlueName";
            this.labelWindowColorBlueName.Size = new System.Drawing.Size(56, 18);
            this.labelWindowColorBlueName.TabIndex = 6;
            this.labelWindowColorBlueName.Text = "色(B)：";
            // 
            // trackBarWindowColorBlue
            // 
            this.trackBarWindowColorBlue.AutoSize = false;
            this.trackBarWindowColorBlue.Location = new System.Drawing.Point(135, 114);
            this.trackBarWindowColorBlue.Maximum = 255;
            this.trackBarWindowColorBlue.Minimum = 1;
            this.trackBarWindowColorBlue.Name = "trackBarWindowColorBlue";
            this.trackBarWindowColorBlue.Size = new System.Drawing.Size(106, 30);
            this.trackBarWindowColorBlue.TabIndex = 8;
            this.trackBarWindowColorBlue.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarWindowColorBlue.Value = 128;
            this.trackBarWindowColorBlue.Scroll += new System.EventHandler(this.updateColor);
            // 
            // labelWindowColorGreenValue
            // 
            this.labelWindowColorGreenValue.Location = new System.Drawing.Point(93, 78);
            this.labelWindowColorGreenValue.Name = "labelWindowColorGreenValue";
            this.labelWindowColorGreenValue.Size = new System.Drawing.Size(40, 18);
            this.labelWindowColorGreenValue.TabIndex = 4;
            this.labelWindowColorGreenValue.Text = "128";
            this.labelWindowColorGreenValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelWindowColorGreenName
            // 
            this.labelWindowColorGreenName.AutoSize = true;
            this.labelWindowColorGreenName.Location = new System.Drawing.Point(12, 78);
            this.labelWindowColorGreenName.Name = "labelWindowColorGreenName";
            this.labelWindowColorGreenName.Size = new System.Drawing.Size(57, 18);
            this.labelWindowColorGreenName.TabIndex = 3;
            this.labelWindowColorGreenName.Text = "色(G)：";
            // 
            // trackBarWindowColorGreen
            // 
            this.trackBarWindowColorGreen.AutoSize = false;
            this.trackBarWindowColorGreen.Location = new System.Drawing.Point(135, 73);
            this.trackBarWindowColorGreen.Maximum = 255;
            this.trackBarWindowColorGreen.Minimum = 1;
            this.trackBarWindowColorGreen.Name = "trackBarWindowColorGreen";
            this.trackBarWindowColorGreen.Size = new System.Drawing.Size(106, 30);
            this.trackBarWindowColorGreen.TabIndex = 5;
            this.trackBarWindowColorGreen.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarWindowColorGreen.Value = 128;
            this.trackBarWindowColorGreen.Scroll += new System.EventHandler(this.updateColor);
            // 
            // labelWindowColorRedValue
            // 
            this.labelWindowColorRedValue.Location = new System.Drawing.Point(93, 42);
            this.labelWindowColorRedValue.Name = "labelWindowColorRedValue";
            this.labelWindowColorRedValue.Size = new System.Drawing.Size(40, 18);
            this.labelWindowColorRedValue.TabIndex = 1;
            this.labelWindowColorRedValue.Text = "128";
            this.labelWindowColorRedValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelWindowColorRedName
            // 
            this.labelWindowColorRedName.AutoSize = true;
            this.labelWindowColorRedName.Location = new System.Drawing.Point(12, 42);
            this.labelWindowColorRedName.Name = "labelWindowColorRedName";
            this.labelWindowColorRedName.Size = new System.Drawing.Size(56, 18);
            this.labelWindowColorRedName.TabIndex = 0;
            this.labelWindowColorRedName.Text = "色(R)：";
            // 
            // trackBarWindowColorRed
            // 
            this.trackBarWindowColorRed.AutoSize = false;
            this.trackBarWindowColorRed.Location = new System.Drawing.Point(135, 37);
            this.trackBarWindowColorRed.Maximum = 255;
            this.trackBarWindowColorRed.Minimum = 1;
            this.trackBarWindowColorRed.Name = "trackBarWindowColorRed";
            this.trackBarWindowColorRed.Size = new System.Drawing.Size(106, 30);
            this.trackBarWindowColorRed.TabIndex = 2;
            this.trackBarWindowColorRed.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarWindowColorRed.Value = 128;
            this.trackBarWindowColorRed.Scroll += new System.EventHandler(this.updateColor);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 390);
            this.Controls.Add(this.groupBoxSetColor);
            this.Controls.Add(this.groupBoxSetWindow);
            this.Controls.Add(this.checkBoxVisible);
            this.Controls.Add(this.groupBoxMoveDirection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "MoveDialog";
            this.groupBoxMoveDirection.ResumeLayout(false);
            this.groupBoxSetWindow.ResumeLayout(false);
            this.groupBoxSetWindow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarWindowHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarWindowWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMoveDistance)).EndInit();
            this.groupBoxSetColor.ResumeLayout(false);
            this.groupBoxSetColor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarWindowColorBlue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarWindowColorGreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarWindowColorRed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonMoveDirectionUp;
        private System.Windows.Forms.GroupBox groupBoxMoveDirection;
        private System.Windows.Forms.Button buttonMoveDirectionDown;
        private System.Windows.Forms.Button buttonMoveDirectionRight;
        private System.Windows.Forms.Button buttonMoveDirectionCentor;
        private System.Windows.Forms.Button buttonMoveDirectionLeft;
        private System.Windows.Forms.CheckBox checkBoxVisible;
        private System.Windows.Forms.GroupBox groupBoxSetWindow;
        private System.Windows.Forms.Label labelWindowHeightValue;
        private System.Windows.Forms.Label labelWindowHeightName;
        private System.Windows.Forms.TrackBar trackBarWindowHeight;
        private System.Windows.Forms.Label labelWindowWidthValue;
        private System.Windows.Forms.Label labelWindowWidthName;
        private System.Windows.Forms.TrackBar trackBarWindowWidth;
        private System.Windows.Forms.Label labelMoveDistanceValue;
        private System.Windows.Forms.Label labelMoveDistanceName;
        private System.Windows.Forms.TrackBar trackBarMoveDistance;
        private System.Windows.Forms.GroupBox groupBoxSetColor;
        private System.Windows.Forms.Label labelWindowColorBlueValue;
        private System.Windows.Forms.Label labelWindowColorBlueName;
        private System.Windows.Forms.TrackBar trackBarWindowColorBlue;
        private System.Windows.Forms.Label labelWindowColorGreenValue;
        private System.Windows.Forms.Label labelWindowColorGreenName;
        private System.Windows.Forms.TrackBar trackBarWindowColorGreen;
        private System.Windows.Forms.Label labelWindowColorRedValue;
        private System.Windows.Forms.Label labelWindowColorRedName;
        private System.Windows.Forms.TrackBar trackBarWindowColorRed;
    }
}

