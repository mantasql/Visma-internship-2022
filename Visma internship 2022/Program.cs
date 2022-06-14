namespace Visma_internship_2022
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string loggedInUser;
            bool programIsRunning = true;
            UserInput input = new UserInput();
            Console.WriteLine("Type your name:\n");
            while((loggedInUser = Console.ReadLine()).Length == 0)
            {
                Console.WriteLine("Name has to be atleast 1 character long\n");
            }

            MeetingManager.LoadMeetingData();

            while (programIsRunning)
            {
                Console.Write($"Current user: {loggedInUser}\n\nChoose your option:\n1. Create meeting\n2. Delete meeting\n3. Add a person\n4. Remove a person\n5. List all meetings\n6. Exit\nOption: ");
                switch (Console.ReadLine())
                {
                    case "1":
                        MeetingManager.CreateMeeting(loggedInUser, input);
                        break;
                    case "2":
                        MeetingManager.DeleteMeeting(loggedInUser, input);
                        break;
                    case "3":
                        MeetingManager.AddPerson(input);
                        break;
                    case "4":
                        MeetingManager.RemovePerson(loggedInUser, input);
                        break;
                    case "5":
                        MeetingManager.ListMeetings(input);
                        break;
                    case "6":
                        programIsRunning = false;
                        break;
                    default:
                        Console.WriteLine("\n[Wrong input]");
                        break;
                }
            }
        }
    }
}