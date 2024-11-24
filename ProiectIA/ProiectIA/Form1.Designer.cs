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
            hoursBox = new ComboBox();
            boxUsers = new ComboBox();
            listSchedules = new ListBox();
            btnAddSchedule = new Button();
            btnRunAlgorithm = new Button();
            SuspendLayout();
            // 
            // calendar
            // 
            calendar.Location = new Point(36, 174);
            calendar.Name = "calendar";
            calendar.TabIndex = 0;
            // 
            // hoursBox
            // 
            hoursBox.FormattingEnabled = true;
            hoursBox.Items.AddRange(new object[] { "00-01", "01-02", "02-03", "03-04", "04-05", "05-06", "06-07", "07-08", "08-09", "09-10", "10-11", "11-12", "12-13", "13-14", "14-15", "15-16", "16-17", "17-18", "18-19", "19-20", "20-21", "21-22", "22-23", "23-24" });
            hoursBox.Location = new Point(331, 174);
            hoursBox.Name = "hoursBox";
            hoursBox.Size = new Size(151, 28);
            hoursBox.TabIndex = 1;
            // 
            // boxUsers
            // 
            boxUsers.FormattingEnabled = true;
            boxUsers.Location = new Point(36, 43);
            boxUsers.Name = "boxUsers";
            boxUsers.Size = new Size(151, 28);
            boxUsers.TabIndex = 2;
            // 
            // listSchedules
            // 
            listSchedules.FormattingEnabled = true;
            listSchedules.Location = new Point(538, 174);
            listSchedules.Name = "listSchedules";
            listSchedules.Size = new Size(321, 204);
            listSchedules.TabIndex = 3;
            // 
            // btnAddSchedule
            // 
            btnAddSchedule.Location = new Point(570, 393);
            btnAddSchedule.Name = "btnAddSchedule";
            btnAddSchedule.Size = new Size(121, 29);
            btnAddSchedule.TabIndex = 4;
            btnAddSchedule.Text = "Add Schedule";
            btnAddSchedule.UseVisualStyleBackColor = true;
            btnAddSchedule.Click += btnAddSchedule_Click;
            // 
            // btnRunAlgorithm
            // 
            btnRunAlgorithm.Location = new Point(706, 393);
            btnRunAlgorithm.Name = "btnRunAlgorithm";
            btnRunAlgorithm.Size = new Size(121, 29);
            btnRunAlgorithm.TabIndex = 5;
            btnRunAlgorithm.Text = "Run Algorithm";
            btnRunAlgorithm.UseVisualStyleBackColor = true;
            btnRunAlgorithm.Click += btnRunAlgorithm_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(871, 455);
            Controls.Add(btnRunAlgorithm);
            Controls.Add(btnAddSchedule);
            Controls.Add(listSchedules);
            Controls.Add(boxUsers);
            Controls.Add(hoursBox);
            Controls.Add(calendar);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion

        private MonthCalendar calendar;
        private ComboBox hoursBox;
        private ComboBox boxUsers;
        private ListBox listSchedules;
        private Button btnAddSchedule;
        private Button btnRunAlgorithm;
    }
}
