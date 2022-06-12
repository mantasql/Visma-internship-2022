using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Visma_internship_2022
{
    internal class MeetingManager
    {
        static List<Meeting> s_meetings = new List<Meeting>();

        public static void CreateMeeting(string loggedInUser)
        {
            Console.WriteLine("Enter meeting's name:");
            string name;
            while ((name = Console.ReadLine()).Length == 0);

            Console.WriteLine("Enter meeting's description:");
            string description = Console.ReadLine();

            bool isEnumParsed;
            Console.WriteLine("Choose meeting's category - 1. CodeMonkey / 2. Hub / 3. Short / 4. TeamBuilding");
            isEnumParsed = Enum.TryParse(Console.ReadLine(), true, out Category category) && Enum.IsDefined(typeof(Category), category);
            if (!isEnumParsed)
            {
                Console.WriteLine("No such category");
                return;
            }

            Console.WriteLine("Choose meeting's type - 1. Live / 2. InPerson");
            isEnumParsed = Enum.TryParse(Console.ReadLine(), true, out Type type) && Enum.IsDefined(typeof(Type), type);
            if (!isEnumParsed)
            {
                Console.WriteLine("No such category");
                return;
            }

            Console.WriteLine("Enter start date (yyyy-MM-dd):");
            string dateString = Console.ReadLine();
            bool startDateParse = DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate);
            if (!startDateParse)
            {
                Console.WriteLine("Wrong date format\n");
                return;
            }

            Console.WriteLine("Enter end date (yyyy-MM-dd):");
            dateString = Console.ReadLine();
            bool endDateParse = DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate);
            if (!endDateParse)
            {
                Console.WriteLine("Wrong date format\n");
                return;
            }

            if(startDate > endDate)
            {
                Console.WriteLine("Start date cannot be later that end date\n");
                return;
            }

            Meeting createdMeeting = new Meeting(name, description, loggedInUser, category, type, startDate, endDate);
            s_meetings.Add(createdMeeting);

            FileManager.WriteToJsonFile(JsonSerializer.Serialize(s_meetings));
        }

        public static void DeleteMeeting(string loggedInUser)
        {
            Console.WriteLine("Enter meeting's name you want to delete:");
            string meetingName = Console.ReadLine();
            Meeting meeting = s_meetings.Find(x => x.Name == meetingName);
            if(meeting != null)
            {
                if(meeting.ResponsiblePerson == loggedInUser)
                {
                    s_meetings.Remove(meeting);
                    FileManager.WriteToJsonFile(JsonSerializer.Serialize(s_meetings));
                }
                else
                {
                    Console.WriteLine("Only the person responsible can delete the meeting\n");
                }
            }
            else
            {
                Console.WriteLine("There are no meetings with this name\n");
            }
        }

        public static void AddPerson()
        {
            string attendeeName;
            string meetingName;
            Meeting meeting;

            Console.WriteLine("Enter meeting's name to add a person to");
            while ((meetingName = Console.ReadLine()).Length == 0) Console.WriteLine("Meetin's name can't be blank");

            meeting = s_meetings.Find(x => x.Name == meetingName);

            if(meeting == null)
            {
                Console.WriteLine("Meeting does't exist");
                return;
            }

            Console.WriteLine("Enter name of the person you want to add to the meeting");
            while ((attendeeName = Console.ReadLine()).Length == 0) Console.WriteLine("Name can't be blank");

            Console.WriteLine("Pick a date when to attend the meeting");
            bool parseDate = DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime meetingsDate);

            if (parseDate)
            {
                if(meetingsDate < meeting.StartDate || meetingsDate > meeting.EndDate)
                {
                    Console.WriteLine("Date should be picked in the right intervals");
                    return;
                }
            } else
            {
                Console.WriteLine("Wrong date format");
                return;
            }

            if (meeting.Attendees.Exists(x => x.Name == attendeeName))
            {
                Console.WriteLine("Choosen person is already in this meeting");
                return;
            }

            meeting.Attendees.Add(new Attendee(attendeeName,meetingsDate));
            FileManager.WriteToJsonFile(JsonSerializer.Serialize(s_meetings));
        }

        public static void RemovePerson(string loggedInUser)
        {
            Console.WriteLine("Enter meeting's name:");
            string meetingName = Console.ReadLine();
            Console.WriteLine("Enter person's name:");
            string personName = Console.ReadLine();

            Meeting meeting = s_meetings.Find(x => x.Name == meetingName);

            if(meeting == null)
            {
                Console.WriteLine("There are no meetings with this name");
                return;
            }

            Attendee attendee = meeting.Attendees.Find(x => x.Name == personName);

            if(attendee == null)
            {
                Console.WriteLine("This person is not attending this meeting");
                return;
            }

            if(attendee.Name == loggedInUser)
            {
                Console.WriteLine("Person responsible for the meeting cannot be removed");
                return;
            }

            meeting.Attendees.Remove(attendee);
            FileManager.WriteToJsonFile(JsonSerializer.Serialize(s_meetings));
        }

        public static void ListMeetings()
        {
            List<Meeting> filteredMeetings = new List<Meeting>();
            Console.WriteLine("Filter by:\n1. Description\n2. Responsible person\n3. Category\n4. Type\n5. Date\n6. Attendess");
            switch (Console.ReadLine())
            {
                case "1":
                    Console.WriteLine("Enter description:");
                    string description = Console.ReadLine();
                    filteredMeetings = s_meetings.Where(x => x.Description.Contains(description)).ToList();
                    break;
                case "2":
                    Console.WriteLine("Enter person's name responsible for the meeting:");
                    string responsiblePerson = Console.ReadLine();
                    filteredMeetings = s_meetings.Where(x => x.ResponsiblePerson == responsiblePerson).ToList();
                    break;
                case "3":
                    Console.WriteLine("Enter category - 1. CodeMonkey / 2. Hub / 3. Short / 4. TeamBuilding:");
                    bool isEnumParsed = Enum.TryParse(Console.ReadLine(), true, out Category category) && Enum.IsDefined(typeof(Category), category);
                    if (isEnumParsed) filteredMeetings = s_meetings.Where(x => x.Category == category).ToList();
                    else
                    {
                        Console.WriteLine("No such category");
                        return;
                    }
                    break;
                case "4":
                    Console.WriteLine("Enter category - 1. Live / 2. InPerson:");
                    isEnumParsed = Enum.TryParse(Console.ReadLine(), true, out Type type) && Enum.IsDefined(typeof(Type), type);
                    if (isEnumParsed) filteredMeetings = s_meetings.Where(x => x.Type == type).ToList();
                    else
                    {
                        Console.WriteLine("No such type");
                        return;
                    }
                    break;
                case "5":
                    DateTime endingDate;
                    DateTime startingDate;
                    Console.WriteLine("Enter starting date (yyyy-MM-dd) or continue");
                    string startingDateString = Console.ReadLine();
                    bool startingDateParse = DateTime.TryParseExact(startingDateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out startingDate);
                    if (startingDateString != "")
                    {
                        if (!startingDateParse)
                        {
                            Console.WriteLine("Wrong date format");
                            return;
                        }
                        Console.WriteLine("Enter ending date (yyyy-MM-dd) or continue");
                        string endingDateString = Console.ReadLine();
                        if (endingDateString != "")
                        {
                            Console.WriteLine(endingDateString);
                            bool endingDateParse = DateTime.TryParseExact(endingDateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out endingDate);
                            if (!endingDateParse)
                            {
                                Console.WriteLine("Wrong date format");
                                return;
                            }
                            filteredMeetings = s_meetings.Where(x => x.StartDate >= startingDate && x.EndDate <= endingDate).ToList();
                        }
                        else
                        {
                            filteredMeetings = s_meetings.Where(x => x.StartDate >= startingDate).ToList();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Enter ending date (yyyy-MM-dd)");
                        bool endingDateParse = DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out endingDate);
                        if (!endingDateParse)
                        {
                            Console.WriteLine("Wrong date format");
                            return;
                        }
                        filteredMeetings = s_meetings.Where(x => x.EndDate <= endingDate).ToList();
                    }
                    break;
                case "6":
                    Console.WriteLine("Enter expression (e.g <10, >10, =10)");
                    string expression = String.Concat(Console.ReadLine().Where(x => !Char.IsWhiteSpace(x)));
                    int expressionValue;
                    try
                    {
                        expressionValue = int.Parse(expression.Substring(1));
                    } catch(Exception) 
                    {
                        Console.WriteLine("Wrong expression");
                        return;
                    }
                    switch (expression[0]) 
                    {
                        case '<':
                            filteredMeetings = s_meetings.Where(x => x.Attendees.Count < expressionValue).ToList();
                            break;
                        case '>':
                            filteredMeetings = s_meetings.Where(x => x.Attendees.Count > expressionValue).ToList();
                            break;
                        case '=':
                            filteredMeetings = s_meetings.Where(x => x.Attendees.Count == expressionValue).ToList();
                            break;
                        default:
                            Console.WriteLine("Wrong expression");
                            break;
                    }
                    break;
                default:
                    Console.WriteLine("[Wrong option]");
                    break;
            }

            if (filteredMeetings.Count != 0) PrintMeetings(filteredMeetings);
            else Console.WriteLine("No meetings found that match the filter criteria");
        }

        public static void LoadMeetingData()
        {
            s_meetings = JsonSerializer.Deserialize<List<Meeting>>(FileManager.GetJsonStringFromFile())!;
        }

        private static void PrintMeetings(List<Meeting> meetings)
        {
            foreach (Meeting meeting in meetings)
            {
                Console.WriteLine(meeting.ToString());
            }
        }
    }
}
