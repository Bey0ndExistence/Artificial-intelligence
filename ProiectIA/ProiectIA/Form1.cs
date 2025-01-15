using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

// For JSON
using Newtonsoft.Json;
using System.IO;

namespace ProiectIA
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// userAvailability[user][date] = List of (Start, End) intervals in TimeSpan
        /// </summary>
        private Dictionary<string, Dictionary<DateTime, List<(TimeSpan Start, TimeSpan End)>>> userAvailability = new Dictionary<string, Dictionary<DateTime, List<(TimeSpan, TimeSpan)>>>();

        public Form1()
        {
            InitializeComponent();
        }
     
        /// <summary>
        /// When the user clicks "Load Schedule":
        /// 1) We read from a JSON file (e.g. "schedules.json").
        /// 2) Fill userAvailability with the data.
        /// 3) Populate boxUsers from the loaded user names.
        /// 4) Show the schedules in listSchedules for feedback.
        /// 
        /// No default users are created at startup; everything is from JSON or from AddUser.
        /// </summary>
        private void buttonLoadSchedule_Click(object sender, EventArgs e)
        {
            // Example path: same folder as EXE
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schedules.json");
            if (!File.Exists(filePath))
            {
                MessageBox.Show("File not found: " + filePath);
                return;
            }

            try
            {
                string json = File.ReadAllText(filePath);

                // The JSON shape we expect:
                // {
                //   "Alice": {
                //     "2025-01-09": [ { "Start": "07:00:00", "End": "09:20:00" }, ... ],
                //     ...
                //   },
                //   "Bob": { ... },
                //   ...
                // }
                var loadedData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<TimeSpanInterval>>>>(json);

                if (loadedData == null)
                {
                    MessageBox.Show("Invalid or empty JSON file.");
                    return;
                }

                // Clear any old data in userAvailability
                userAvailability.Clear();

                // Clear the combo box and list box so we start fresh
                boxUsers.Items.Clear();
                listSchedules.Items.Clear();

                // Merge data from JSON into userAvailability
                foreach (var kvpUser in loadedData)
                {
                    string userName = kvpUser.Key;

                    // If user doesn't exist yet, add it
                    if (!userAvailability.ContainsKey(userName))
                    {
                        userAvailability[userName] = new Dictionary<DateTime, List<(TimeSpan, TimeSpan)>>();
                        boxUsers.Items.Add(userName); // also add to the UI
                    }

                    // For each date in that user
                    foreach (var kvpDate in kvpUser.Value)
                    {
                        // Convert e.g. "2025-01-09" => DateTime
                        if (!DateTime.TryParse(kvpDate.Key, out DateTime date))
                            continue; // or skip if invalid

                        if (!userAvailability[userName].ContainsKey(date))
                            userAvailability[userName][date] = new List<(TimeSpan, TimeSpan)>();

                        // For each interval
                        foreach (var intervalObj in kvpDate.Value)
                        {
                            userAvailability[userName][date].Add((intervalObj.Start, intervalObj.End));
                        }
                    }
                }

                // Optionally, select the first user in the list
                if (boxUsers.Items.Count > 0)
                {
                    boxUsers.SelectedIndex = 0;
                }

                // Show loaded schedules in the list box for feedback
                foreach (var user in userAvailability.Keys)
                {
                    foreach (var dayKvp in userAvailability[user])
                    {
                        foreach (var (start, end) in dayKvp.Value)
                        {
                            listSchedules.Items.Add($"{user} => {dayKvp.Key.ToShortDateString()} : {start:hh\\:mm}-{end:hh\\:mm}");
                        }
                    }
                }

                MessageBox.Show("Schedules loaded successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading JSON:\n" + ex.Message);
            }
        }
      

        /// <summary>
        /// Adds a new user at runtime. If a user with the same name doesn't exist,
        /// we add them to userAvailability and boxUsers.
        /// </summary>
        private void btnAddUser_Click(object sender, EventArgs e)
        {
            string newUserName = Microsoft.VisualBasic.Interaction.InputBox("Enter new user name:", "Add User", "");
            // returns empty string if user cancels

            if (string.IsNullOrWhiteSpace(newUserName))
                return; // user canceled or typed nothing

            if (userAvailability.ContainsKey(newUserName))
            {
                MessageBox.Show("That user already exists!");
                return;
            }

            userAvailability[newUserName] = new Dictionary<DateTime, List<(TimeSpan, TimeSpan)>>();
            boxUsers.Items.Add(newUserName);
            boxUsers.SelectedItem = newUserName;
        }
      

     
        /// <summary>
        /// Add an interval to the selected user on the selected day.
        /// </summary>
        private void btnAddSchedule_Click(object sender, EventArgs e)
        {
            if (boxUsers.SelectedItem == null)
            {
                MessageBox.Show("Please select a user first.");
                return;
            }

            string ?user = boxUsers.SelectedItem.ToString();
            if (string.IsNullOrEmpty(user))
            {
                MessageBox.Show("Please enter a valid name.");
                return;
            }
            DateTime selectedDay = calendar.SelectionStart;

            // e.g. "07:00-09:20"
            string intervalText = hoursTextBox.Text.Trim();
            if (string.IsNullOrEmpty(intervalText))
            {
                MessageBox.Show("Please enter an interval in the HH:MM-HH:MM format.");
                return;
            }

            if (!TryParseInterval(intervalText, out TimeSpan start, out TimeSpan end))
            {
                MessageBox.Show("Invalid interval! Must be HH:MM-HH:MM and start < end.");
                return;
            }

            if (!userAvailability[user].ContainsKey(selectedDay))
            {
                userAvailability[user][selectedDay] = new List<(TimeSpan, TimeSpan)>();
            }

            userAvailability[user][selectedDay].Add((start, end));

            // Show in listSchedules
            listSchedules.Items.Add($"{user} => {selectedDay.ToShortDateString()} : {start:hh\\:mm}-{end:hh\\:mm}");
        }

        /// <summary>
        /// Attempt to parse "HH:MM-HH:MM" into TimeSpans. 
        /// Returns false if invalid or if start >= end.
        /// </summary>
        private bool TryParseInterval(string text, out TimeSpan start, out TimeSpan end)
        {
            start = TimeSpan.Zero;
            end = TimeSpan.Zero;

            string[] parts = text.Split('-', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2) return false;

            if (!TimeSpan.TryParse(parts[0], out TimeSpan tempStart)) return false;
            if (!TimeSpan.TryParse(parts[1], out TimeSpan tempEnd)) return false;

            if (tempStart >= tempEnd) return false;

            start = tempStart;
            end = tempEnd;
            return true;
        }
  

        /// <summary>
        /// Example of forward checking across multiple days in the monthCalendar's selected range.
        /// Shows intersection results (or no intersection).
        /// </summary>
        private void btnRunAlgorithm_Click(object sender, EventArgs e)
        {
            listSchedules.Items.Clear();

            // All users currently known
            var allUsers = boxUsers.Items.Cast<string>().ToList();

            // Iterate from SelectionRange.Start to SelectionRange.End
            DateTime day = calendar.SelectionRange.Start;
            DateTime lastDay = calendar.SelectionRange.End;

            while (day <= lastDay)
            {
                // Build domain for each user for this day
                var domains = new Dictionary<string, List<(TimeSpan Start, TimeSpan End)>>();
                foreach (var user in allUsers)
                {
                    if (userAvailability.ContainsKey(user) && userAvailability[user].ContainsKey(day))
                    {
                        domains[user] = new List<(TimeSpan, TimeSpan)>(userAvailability[user][day]);
                    }
                    else
                    {
                        domains[user] = new List<(TimeSpan, TimeSpan)>();
                    }
                }

                // Attempt forward checking
                var result = ForwardChecking(allUsers, domains, 0, null);

                // Show the result
                if (result == null || result.Count == 0)
                {
                    listSchedules.Items.Add($"{day.ToShortDateString()} => No common intersection");
                }
                else
                {
                    listSchedules.Items.Add($"{day.ToShortDateString()} => Intersection(s):");
                    foreach (var (s, e2) in result)
                    {
                        listSchedules.Items.Add($"   {s:hh\\:mm}-{e2:hh\\:mm}");
                    }
                }

                day = day.AddDays(1);
            }
        }


        /// <summary>
        /// ForwardChecking tries to find an intersection among all users:
        ///   - If userIndex == users.Count => we assigned intervals for everyone => success (return currentIntersection).
        ///   - Otherwise, pick the next user's domain and try each interval.
        ///     Intersect that interval with currentIntersection so far, reduce future users' domains, etc.
        /// </summary>
        private List<(TimeSpan, TimeSpan)> ?ForwardChecking(List<string> users,
                                                            Dictionary<string, List<(TimeSpan Start, TimeSpan End)>> domains, 
                                                            int userIndex, 
                                                            List<(TimeSpan, TimeSpan)> ?currentIntersection)
        {
            if (userIndex == users.Count)   // verified all users
                return currentIntersection; // success

            string user = users[userIndex];
            var userDomain = domains[user];

            if (userDomain.Count == 0) // There are no free days for a user. Don't ckeck further
                return null; // fail

            foreach (var interval in userDomain)
            {
                List<(TimeSpan, TimeSpan)> newIntersection;
                if (currentIntersection == null)
                {
                    // first user => intersection is just his interval
                    newIntersection = new List<(TimeSpan, TimeSpan)> { interval };
                }
                else
                {
                    // intersect with the existing intersection
                    newIntersection = IntersectListWithInterval(currentIntersection, interval);
                    if (newIntersection.Count == 0)
                        continue; // no overlap => try next
                }

                // reduce domains for future users
                var reducedDomains = ReduceDomains(users, domains, userIndex, interval);

                // recurse
                var solution = ForwardChecking(users, reducedDomains, userIndex + 1, newIntersection);
                if (solution != null && solution.Count > 0)
                {
                    // found a valid intersection => return it
                    return solution;
                }
            }
            return null; // no interval worked
        }

        /// <summary>
        /// Reduce the domain of future users so they only keep intervals that overlap with assignedInterval.
        /// </summary>
        private Dictionary<string, List<(TimeSpan, TimeSpan)>> ReduceDomains( List<string> users, 
                                                                              Dictionary<string, List<(TimeSpan Start, TimeSpan End)>> domains, 
                                                                              int userIndex,
                                                                              (TimeSpan Start, TimeSpan End) assignedInterval)
        {
            var newDomains = new Dictionary<string, List<(TimeSpan, TimeSpan)>>();
            foreach (var kvp in domains)
            {
                newDomains[kvp.Key] = new List<(TimeSpan, TimeSpan)>(kvp.Value);
            }

            for (int i = userIndex + 1; i < users.Count; i++)
            {
                var futureUser = users[i];
                var originalList = newDomains[futureUser];
                var filtered = new List<(TimeSpan, TimeSpan)>();

                foreach (var candidate in originalList)
                {
                    if (IntervalsOverlap(candidate, assignedInterval))
                        filtered.Add(candidate);
                }
                newDomains[futureUser] = filtered;
            }
            return newDomains;
        }

        private bool IntervalsOverlap((TimeSpan Start, TimeSpan End) A, (TimeSpan Start, TimeSpan End) B)
        {
            return (A.Start < B.End && B.Start < A.End);
        }

        /// <summary>
        /// Intersect a list of intervals (the current intersection) with a single interval.
        /// Return the portions that overlap.
        /// </summary>
        private List<(TimeSpan Start, TimeSpan End)> IntersectListWithInterval(List<(TimeSpan Start, TimeSpan End)> currentIntersection,
                                                                               (TimeSpan Start, TimeSpan End) interval
                                                                              )
        {
            var result = new List<(TimeSpan Start, TimeSpan End)>();
            foreach (var (cStart, cEnd) in currentIntersection)
            {
                var overlapStart = cStart > interval.Start ? cStart : interval.Start;
                var overlapEnd = cEnd < interval.End ? cEnd : interval.End;
                if (overlapStart < overlapEnd)
                {
                    result.Add((overlapStart, overlapEnd));
                }
            }
            return result;
        }

    
        /// <summary>
        /// Used when deserializing from JSON. 
        /// We'll parse Start/End as TimeSpan from "hh:mm:ss" or "hh:mm" strings.
        /// </summary>
        public class TimeSpanInterval
        {
            public TimeSpan Start { get; set; }
            public TimeSpan End { get; set; }
        }

    }
}
