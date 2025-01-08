namespace ProiectIA
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            calendar = new MonthCalendar();
            boxUsers = new ComboBox();
            listSchedules = new ListBox();
            btnAddSchedule = new Button();
            btnRunAlgorithm = new Button();
            hoursTextBox = new TextBox();
            SuspendLayout();
            // 
            // calendar
            // 
            calendar.Location = new Point(18, 197);
            calendar.MaxSelectionCount = 14;
            calendar.Name = "calendar";
            calendar.TabIndex = 0;
            // 
            // boxUsers
            // 
            boxUsers.FormattingEnabled = true;
            boxUsers.Location = new Point(18, 43);
            boxUsers.Name = "boxUsers";
            boxUsers.Size = new Size(151, 28);
            boxUsers.TabIndex = 2;
            // 
            // listSchedules
            // 
            listSchedules.FormattingEnabled = true;
            listSchedules.Location = new Point(547, 197);
            listSchedules.Name = "listSchedules";
            listSchedules.Size = new Size(321, 204);
            listSchedules.TabIndex = 3;
            // 
            // btnAddSchedule
            // 
            btnAddSchedule.Location = new Point(345, 239);
            btnAddSchedule.Name = "btnAddSchedule";
            btnAddSchedule.Size = new Size(121, 29);
            btnAddSchedule.TabIndex = 4;
            btnAddSchedule.Text = "Add Schedule";
            btnAddSchedule.UseVisualStyleBackColor = true;
            btnAddSchedule.Click += btnAddSchedule_Click;
            // 
            // btnRunAlgorithm
            // 
            btnRunAlgorithm.Location = new Point(345, 274);
            btnRunAlgorithm.Name = "btnRunAlgorithm";
            btnRunAlgorithm.Size = new Size(121, 29);
            btnRunAlgorithm.TabIndex = 5;
            btnRunAlgorithm.Text = "Run Algorithm";
            btnRunAlgorithm.UseVisualStyleBackColor = true;
            btnRunAlgorithm.Click += btnRunAlgorithm_Click;
            // 
            // hoursTextBox
            // 
            hoursTextBox.Location = new Point(307, 197);
            hoursTextBox.Name = "hoursTextBox";
            hoursTextBox.Size = new Size(198, 27);
            hoursTextBox.TabIndex = 6;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(871, 455);
            Controls.Add(hoursTextBox);
            Controls.Add(btnRunAlgorithm);
            Controls.Add(btnAddSchedule);
            Controls.Add(listSchedules);
            Controls.Add(boxUsers);
            Controls.Add(calendar);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MonthCalendar calendar;
        private ComboBox boxUsers;
        private ListBox listSchedules;
        private Button btnAddSchedule;
        private Button btnRunAlgorithm;
        private TextBox hoursTextBox;
    }
}
