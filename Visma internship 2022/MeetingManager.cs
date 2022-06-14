using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Visma_internship_2022
{
    internal class MeetingManager
    {
        internal static List<Meeting> s_meetings = new List<Meeting>();

        public static void CreateMeeting(string loggedInUser, IUserInput input)
        {
            try
            {
                Console.WriteLine("Enter meeting's name:");
                string name;
                while ((name = input.GetInput()).Length == 0);

                Console.WriteLine("Enter meeting's description:");
                string description = input.GetInput();

                Console.WriteLine("Choose meeting's category - 1. CodeMonkey / 2. Hub / 3. Short / 4. TeamBuilding");
                bool isEnumParsed1 = Enum.TryParse(input.GetInput(), true, out Category category) && Enum.IsDefined(typeof(Category), category);
                if (!isEnumParsed1) throw new Exception("No such category");

                Console.WriteLine("Choose meeting's type - 1. Live / 2. InPerson");
                bool isEnumParsed2 = Enum.TryParse(input.GetInput(), true, out Type type) && Enum.IsDefined(typeof(Type), type);
                if (!isEnumParsed2) throw new Exception("No such category");

                Console.WriteLine("Enter start date (yyyy-MM-dd):");
                string dateString = input.GetInput();
                bool startDateParse = DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate);
                if (!startDateParse) throw new FormatException("Wrong date format");

                Console.WriteLine("Enter end date (yyyy-MM-dd):");
                dateString = input.GetInput();
                bool endDateParse = DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate);

                if (!endDateParse) throw new FormatException("Wrong date format");
                if (startDate > endDate) throw new InvalidDataException("Start date cannot be later than end date");

                Meeting createdMeeting = new Meeting(name, description, loggedInUser, category, type, startDate, endDate);
                s_meetings.Add(createdMeeting);
                FileManager.WriteToJsonFile(JsonSerializer.Serialize(s_meetings));
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void DeleteMeeting(string loggedInUser, IUserInput input)
        {
            try
            {
                Console.WriteLine("Enter meeting's name you want to delete:");
                string meetingName = input.GetInput();
                Meeting meeting = s_meetings.Find(x => x.Name == meetingName);

                if (meeting == null) throw new Exception("There are no meetings with this name");
                if (meeting.ResponsiblePerson != loggedInUser) throw new Exception("Only the person responsible can delete the meeting");

                s_meetings.Remove(meeting);
                FileManager.WriteToJsonFile(JsonSerializer.Serialize(s_meetings));

            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void AddPerson(IUserInput input)
        {
            try
            {
                string attendeeName;
                string meetingName;
                Meeting meeting;

                Console.WriteLine("Enter meeting's name to add a person to");
                while ((meetingName = input.GetInput()).Length == 0) Console.WriteLine("Meetin's name can't be blank");

                meeting = s_meetings.Find(x => x.Name == meetingName);
                if (meeting == null) throw new Exception("Meeting does't exist");

                Console.WriteLine("Enter name of the person you want to add to the meeting");
                while ((attendeeName = input.GetInput()).Length == 0) Console.WriteLine("Name can't be blank");

                Console.WriteLine("Pick a date when to attend the meeting");
                bool parseDate = DateTime.TryParseExact(input.GetInput(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime usersPickedDate);
                if (!parseDate) throw new FormatException("Wrong date format");

                if (usersPickedDate < meeting.StartDate || usersPickedDate > meeting.EndDate) 
                    throw new FormatException("Date should be picked in the right intervals");

                if (meeting.Attendees.Exists(x => x.Name == attendeeName))
                    throw new Exception("Choosen person is already in this meeting");

                meeting.Attendees.Add(new Attendee(attendeeName, usersPickedDate));
                FileManager.WriteToJsonFile(JsonSerializer.Serialize(s_meetings));

            } catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void RemovePerson(string loggedInUser, IUserInput input)
        {
            try
            {
                Console.WriteLine("Enter meeting's name:");
                string meetingName = input.GetInput();
                Console.WriteLine("Enter person's name:");
                string personName = input.GetInput();

                Meeting meeting = s_meetings.Find(x => x.Name == meetingName);
                if (meeting == null) throw new Exception("There are no meetings with this name");

                Attendee attendee = meeting.Attendees.Find(x => x.Name == personName);
                if (attendee == null) throw new Exception("This person is not attending this meeting");

                if (attendee.Name == loggedInUser) throw new Exception("Person responsible for the meeting cannot be removed");

                meeting.Attendees.Remove(attendee);
                FileManager.WriteToJsonFile(JsonSerializer.Serialize(s_meetings));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void ListMeetings(IUserInput input)
        {
            List<Meeting> filteredMeetings = new List<Meeting>();
            Console.WriteLine("Filter by:\n1. Description\n2. Responsible person\n3. Category\n4. Type\n5. Date\n6. Attendess");
            try
            {
                switch (input.GetInput())
                {
                    case "1":
                        Console.WriteLine("Enter description:");
                        string description = input.GetInput();
                        filteredMeetings = s_meetings.Where(x => x.Description.Contains(description)).ToList();
                        break;
                    case "2":
                        Console.WriteLine("Enter person's name responsible for the meeting:");
                        string responsiblePerson = input.GetInput();
                        filteredMeetings = s_meetings.Where(x => x.ResponsiblePerson == responsiblePerson).ToList();
                        break;
                    case "3":
                        Console.WriteLine("Enter category - 1. CodeMonkey / 2. Hub / 3. Short / 4. TeamBuilding:");
                        bool isEnumParsed = Enum.TryParse(input.GetInput(), true, out Category category) && Enum.IsDefined(typeof(Category), category);
                        if (!isEnumParsed) throw new Exception("No such category");
                        filteredMeetings = s_meetings.Where(x => x.Category == category).ToList();
                        break;
                    case "4":
                        Console.WriteLine("Enter category - 1. Live / 2. InPerson:");
                        isEnumParsed = Enum.TryParse(input.GetInput(), true, out Type type) && Enum.IsDefined(typeof(Type), type);
                        if (!isEnumParsed) throw new Exception("No such type");
                        filteredMeetings = s_meetings.Where(x => x.Type == type).ToList();
                        break;
                    case "5":
                        DateTime endingDate;
                        DateTime startingDate;
                        Console.WriteLine("Enter starting date (yyyy-MM-dd) or continue");
                        string startingDateString = input.GetInput();
                        bool startingDateParse = DateTime.TryParseExact(startingDateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out startingDate);
                        if (startingDateString != "")
                        {
                            if (!startingDateParse) throw new FormatException("Wrong date format");

                            Console.WriteLine("Enter ending date (yyyy-MM-dd) or continue");
                            string endingDateString = input.GetInput();
                            if (endingDateString != "")
                            {
                                bool endingDateParse = DateTime.TryParseExact(endingDateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out endingDate);
                                if (!endingDateParse) throw new FormatException("Wrong date format");

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
                            bool endingDateParse = DateTime.TryParseExact(input.GetInput(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out endingDate);
                            if (!endingDateParse) throw new FormatException("Wrong date format");
                            filteredMeetings = s_meetings.Where(x => x.EndDate <= endingDate).ToList();
                        }
                        break;
                    case "6":
                        Console.WriteLine("Enter expression (e.g <10, >10, =10)");
                        string expression = String.Concat(input.GetInput().Where(x => !Char.IsWhiteSpace(x)));
                        int expressionValue;
                        try
                        {
                            expressionValue = int.Parse(expression.Substring(1));
                        }
                        catch (Exception)
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
            } catch(Exception e) 
            {
                Console.WriteLine(e.Message);
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
