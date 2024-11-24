namespace ProiectIA
{
    public partial class Form1 : Form
    {
        // Store user availabilities
        private Dictionary<string, List<(DateTime Day, string Hours)>> userAvailability = new Dictionary<string, List<(DateTime, string)>>();

        // Selected users for the meeting
        private List<string> selectedUsers = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize users
            boxUsers.Items.Add("User1");
            boxUsers.Items.Add("User2");
            boxUsers.Items.Add("User3");
            boxUsers.SelectedIndex = 0;

            // Initialize user availability
            foreach (string user in boxUsers.Items)
            {
                userAvailability[user] = new List<(DateTime, string)>();
            }
        }

        private void btnAddSchedule_Click(object sender, EventArgs e)
        {
            if (boxUsers.SelectedItem == null || hoursBox.SelectedItem == null || calendar.SelectionStart == null)
            {
                MessageBox.Show("Please select a user, a day, and an hour.");
                return;
            }

            string user = boxUsers.SelectedItem.ToString();
            DateTime selectedDay = calendar.SelectionStart;
            string selectedHour = hoursBox.SelectedItem.ToString();

            // Add availability for the user
            userAvailability[user].Add((selectedDay, selectedHour));
            listSchedules.Items.Add($"{user}: {selectedDay.ToShortDateString()} - {selectedHour}");
        }

        private Dictionary<string, List<string>> userSchedules = new Dictionary<string, List<string>>();

        private void btnRunAlgorithm_Click(object sender, EventArgs e)
        {
            // Populate userSchedules with data from userAvailability
            userSchedules = userAvailability.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Select(v => $"{v.Day.ToShortDateString()} - {v.Hours}").ToList()
            );

            // Combine all users and their available time slots into variables and domains
            var variables = new List<string>(userSchedules.Keys);
            var domains = new Dictionary<string, List<string>>(userSchedules);

            // Forward Checking: Solve for common meeting time
            var commonSlots = ForwardChecking(variables, domains, new Dictionary<string, string>());

            // Display the result in the ListBox
            listSchedules.Items.Clear();
            if (commonSlots.Any())
            {
                listSchedules.Items.Add("Common Available Time Slot:");
                foreach (var slot in commonSlots.Distinct()) // Ensure unique common slots
                {
                    listSchedules.Items.Add(slot);
                }
            }
            else
            {
                listSchedules.Items.Add("No common time slot found.");
            }
        }




        private List<string> ForwardChecking(
      List<string> variables,
      Dictionary<string, List<string>> domains,
      Dictionary<string, string> solution)
        {
            // If all variables are assigned, check if there's a common time slot and return it
            if (variables.Count == 0)
            {
                // Check if all users have the same time slot (common available time slot)
                var commonSlot = solution.Values.Distinct().ToList();
                if (commonSlot.Count == 1)
                {
                    return new List<string> { commonSlot.First() };
                }
                return new List<string>(); // No common slot if they differ
            }

            // Select the first unassigned variable
            var currentUser = variables[0];
            var remainingUsers = variables.Skip(1).ToList();

            // List to store all possible common time slots
            List<string> allCommonSlots = new List<string>();

            foreach (var timeSlot in domains[currentUser])
            {
                // Assign the time slot to the current user
                solution[currentUser] = timeSlot;

                // Reduce the domain of remaining users
                var reducedDomains = ReduceDomains(currentUser, timeSlot, domains, remainingUsers);

                // Check if any domain becomes empty
                if (reducedDomains.Values.Any(domain => domain.Count == 0))
                    continue;

                // Recursive call to explore further assignments
                var partialCommonSlots = ForwardChecking(remainingUsers, reducedDomains, solution);

                // Merge results
                allCommonSlots.AddRange(partialCommonSlots);
            }

            return allCommonSlots;
        }


        private Dictionary<string, List<string>> ReduceDomains(
            string currentUser,
            string assignedTimeSlot,
            Dictionary<string, List<string>> domains,
            List<string> remainingUsers)
        {
            // Create a deep copy of domains
            var reducedDomains = new Dictionary<string, List<string>>(domains);

            foreach (var user in remainingUsers)
            {
                // Remove the assigned time slot from other users' domains
                reducedDomains[user] = reducedDomains[user].Where(slot => slot == assignedTimeSlot).ToList();
            }

            return reducedDomains;
        }
      
    }
}
