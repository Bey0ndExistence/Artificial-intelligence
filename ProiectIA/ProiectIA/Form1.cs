using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ProiectIA
{
    public partial class Form1 : Form
    {
        // userAvailability[user][day] = list of intervals (Start, End) in TimeSpan
        private Dictionary<string, Dictionary<DateTime, List<(TimeSpan Start, TimeSpan End)>>> userAvailability;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            userAvailability = new Dictionary<string, Dictionary<DateTime, List<(TimeSpan, TimeSpan)>>>();

            // Example: add users
            boxUsers.Items.Add("User1");
            boxUsers.Items.Add("User2");
            boxUsers.Items.Add("User3");
            boxUsers.SelectedIndex = 0;

            // Prepare each user's day dictionary
            foreach (var u in boxUsers.Items)
            {
                userAvailability[u.ToString()] = new Dictionary<DateTime, List<(TimeSpan, TimeSpan)>>();
            }

            // Optional: allow multiple day selection in the MonthCalendar
            // (by default it might allow only a single day selection).
            // For example:
            //   calendar.MaxSelectionCount = 7; // up to 7 days
        }

        /// <summary>
        /// When clicking "Add Schedule", parse the interval from hoursTextBox,
        /// validate it, and store in userAvailability.
        /// </summary>
        private void btnAddSchedule_Click(object sender, EventArgs e)
        {
            if (boxUsers.SelectedItem == null)
            {
                MessageBox.Show("Please select a user first.");
                return;
            }

            string user = boxUsers.SelectedItem.ToString();
            DateTime selectedDay = calendar.SelectionStart;

            string intervalText = hoursTextBox.Text.Trim(); // e.g. "08:00-10:15"
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

            // Ensure the dictionary for that day exists
            if (!userAvailability[user].ContainsKey(selectedDay))
            {
                userAvailability[user][selectedDay] = new List<(TimeSpan, TimeSpan)>();
            }

            // Add this interval
            userAvailability[user][selectedDay].Add((start, end));

            // Show in listSchedules for feedback
            listSchedules.Items.Add(
                $"{user} => {selectedDay.ToShortDateString()} : {start:hh\\:mm}-{end:hh\\:mm}");
        }

        /// <summary>
        /// Parse "HH:MM-HH:MM", ensuring start < end.
        /// </summary>
        private bool TryParseInterval(string text, out TimeSpan start, out TimeSpan end)
        {
            start = TimeSpan.Zero;
            end = TimeSpan.Zero;

            string[] parts = text.Split('-', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2) return false;

            if (!TimeSpan.TryParse(parts[0], out TimeSpan tempStart)) return false;
            if (!TimeSpan.TryParse(parts[1], out TimeSpan tempEnd)) return false;

            // start must be strictly less than end
            if (tempStart >= tempEnd) return false;

            start = tempStart;
            end = tempEnd;
            return true;
        }

        /// <summary>
        /// Run the forward-checking algorithm across all (or selected) users
        /// for each day in the MonthCalendar's selection range.
        /// Display the (first) common intersection found per day.
        /// 
        /// If you want a single intersection that spans *all* selected days,
        /// you'd need a different approach (combine days into your domain).
        /// </summary>
        private void btnRunAlgorithm_Click(object sender, EventArgs e)
        {
            listSchedules.Items.Clear();

            // Let's gather all users from boxUsers (could also allow the user
            // to select only a subset).
            var allUsers = boxUsers.Items.Cast<string>().ToList();

            // We'll consider each day from SelectionStart to SelectionEnd
            DateTime day = calendar.SelectionRange.Start;
            DateTime lastDay = calendar.SelectionRange.End;

            // If a user wants to select multiple *non-contiguous* days, they'd have to
            // do that differently. MonthCalendar typically handles a continuous range.

            while (day <= lastDay)
            {
                // Build the domain for each user for the current day
                var domains = new Dictionary<string, List<(TimeSpan Start, TimeSpan End)>>();
                foreach (var user in allUsers)
                {
                    if (userAvailability[user].ContainsKey(day))
                    {
                        // Copy intervals for that day
                        domains[user] = new List<(TimeSpan, TimeSpan)>(userAvailability[user][day]);
                    }
                    else
                    {
                        // No intervals => domain is empty => no solution for that user
                        domains[user] = new List<(TimeSpan, TimeSpan)>();
                    }
                }

                // Attempt forward checking for this day
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

                // Move to next day
                day = day.AddDays(1);
            }
        }

        /// <summary>
        /// Forward Checking approach for time-interval "domains":
        /// 
        /// users         = the list of all users (e.g. ["User1", "User2", "User3"])
        /// domains       = map from user -> list of intervals that user can do (for a single day)
        /// userIndex     = which user we are assigning currently
        /// currentIntersection = intersection intervals so far (null if none assigned yet)
        /// 
        /// Returns a list of intervals that remain after all users have been assigned,
        /// or null/empty if no solution was found.
        /// 
        /// This returns only the *first* valid intersection it finds; if you need all
        /// possible solutions, you'd collect them in a list instead of returning immediately.
        /// </summary>
        private List<(TimeSpan, TimeSpan)> ForwardChecking(
            List<string> users,
            Dictionary<string, List<(TimeSpan Start, TimeSpan End)>> domains,
            int userIndex,
            List<(TimeSpan, TimeSpan)> currentIntersection)
        {
            // If we've assigned intervals to all users, currentIntersection is final
            if (userIndex == users.Count)
            {
                return currentIntersection;
            }

            // Next user to assign
            var user = users[userIndex];
            var userDomain = domains[user]; // intervals for this user on the day

            if (userDomain.Count == 0)
            {
                // No intervals => no solution for this user
                return null;
            }

            // Try each interval in this user's domain
            foreach (var interval in userDomain)
            {
                // Build a new intersection
                List<(TimeSpan, TimeSpan)> newIntersection;
                if (currentIntersection == null)
                {
                    // First user => the "intersection" is just this interval
                    newIntersection = new List<(TimeSpan, TimeSpan)> { interval };
                }
                else
                {
                    // Intersect this interval with the existing intersection
                    newIntersection = IntersectListWithInterval(currentIntersection, interval);
                    if (newIntersection.Count == 0)
                    {
                        // No overlap => skip
                        continue;
                    }
                }

                // Forward check => reduce domains for future users
                var reducedDomains = ReduceDomains(users, domains, userIndex, interval);

                // Recur
                var solution = ForwardChecking(users, reducedDomains, userIndex + 1, newIntersection);
                if (solution != null && solution.Count > 0)
                {
                    // Found a solution => return it
                    return solution;
                }
                // If it fails, we try the next interval in userDomain
            }

            // None of the intervals worked => fail
            return null;
        }

        /// <summary>
        /// We reduce the domains of future users (from userIndex+1 onward)
        /// to only keep intervals overlapping with `assignedInterval`.
        /// 
        /// Classic forward-checking concept: picking an assignment for one variable
        /// prunes future variables' domains to only consistent values.
        /// </summary>
        private Dictionary<string, List<(TimeSpan, TimeSpan)>> ReduceDomains(
            List<string> users,
            Dictionary<string, List<(TimeSpan Start, TimeSpan End)>> domains,
            int userIndex,
            (TimeSpan Start, TimeSpan End) assignedInterval)
        {
            // Make a deep copy so we don't ruin the original domains
            var newDomains = new Dictionary<string, List<(TimeSpan, TimeSpan)>>();
            foreach (var kvp in domains)
            {
                newDomains[kvp.Key] = new List<(TimeSpan, TimeSpan)>(kvp.Value);
            }

            // For all future users, filter out intervals that don't overlap
            for (int i = userIndex + 1; i < users.Count; i++)
            {
                var futureUser = users[i];
                var originalList = newDomains[futureUser];
                var filtered = new List<(TimeSpan, TimeSpan)>();

                foreach (var candidate in originalList)
                {
                    if (IntervalsOverlap(candidate, assignedInterval))
                    {
                        filtered.Add(candidate);
                    }
                }

                newDomains[futureUser] = filtered;
            }

            return newDomains;
        }

        /// <summary>
        /// Checks if two intervals overlap (start < other.end && other.start < end).
        /// </summary>
        private bool IntervalsOverlap(
            (TimeSpan Start, TimeSpan End) A,
            (TimeSpan Start, TimeSpan End) B)
        {
            return (A.Start < B.End && B.Start < A.End);
        }

        /// <summary>
        /// Intersects an existing list of intervals with a single interval,
        /// returning the portions that overlap.
        /// 
        /// E.g. 
        ///   currentIntersection = [ (08:00, 10:00), (12:00, 13:00) ]
        ///   interval = (09:00, 12:30)
        /// => [ (09:00, 10:00), (12:00, 12:30) ]
        /// </summary>
        private List<(TimeSpan Start, TimeSpan End)> IntersectListWithInterval(
            List<(TimeSpan Start, TimeSpan End)> currentIntersection,
            (TimeSpan Start, TimeSpan End) interval)
        {
            var result = new List<(TimeSpan Start, TimeSpan End)>();

            foreach (var (cStart, cEnd) in currentIntersection)
            {
                // Overlap start = max of starts
                var overlapStart = (cStart > interval.Start) ? cStart : interval.Start;
                // Overlap end   = min of ends
                var overlapEnd = (cEnd < interval.End) ? cEnd : interval.End;

                if (overlapStart < overlapEnd)
                {
                    result.Add((overlapStart, overlapEnd));
                }
            }

            return result;
        }
    }
}
