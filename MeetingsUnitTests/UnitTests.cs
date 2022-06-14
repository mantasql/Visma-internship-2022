using Visma_internship_2022;
using System.Globalization;
using System.Text.Json;

namespace MeetingsUnitTests
{
    public class UnitTests
    {
        [SetUp]
        public void Setup()
        {
            MeetingManager.s_meetings.Clear();
        }

        [Test]
        public void CreateMeeting_GivenValidData_ShouldPass()
        {
            FakeUserInput input = new FakeUserInput();
            string _name = "test name";
            string _description = "test description";
            string _responsiblePerson = "test";
            Category _category = Category.Hub;
            Visma_internship_2022.Type _type = Visma_internship_2022.Type.Live;
            DateTime _startDate = DateTime.ParseExact("2022-06-14", "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime _endDate = DateTime.ParseExact("2022-06-24", "yyyy-MM-dd", CultureInfo.InvariantCulture);

            Meeting expectedMeeting = new Meeting(_name, _description, _responsiblePerson, _category, _type, _startDate, _endDate);
            List<Meeting> meetings = new List<Meeting>();
            meetings.Add(expectedMeeting);
            input.LinesToRead.Add(_name);
            input.LinesToRead.Add(_description);
            input.LinesToRead.Add("Hub");
            input.LinesToRead.Add("Live");
            input.LinesToRead.Add("2022-06-14");
            input.LinesToRead.Add("2022-06-24");
            MeetingManager.CreateMeeting(_responsiblePerson,input);
            string expectedJson = JsonSerializer.Serialize(meetings);
            string actualJson = FileManager.GetJsonStringFromFile();

            Assert.That(actualJson, Is.EqualTo(expectedJson));
        }

        [Test]
        public void DeleteMeeting_GivenValidMeetingNameAndLoggedInPerson_ShouldPass()
        {
            FakeUserInput input = new FakeUserInput();
            string meetingName = "testMeetingName";
            string loggedInUser = "respPerson";
            DateTime _startDate = DateTime.ParseExact("2022-06-14", "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime _endDate = DateTime.ParseExact("2022-06-24", "yyyy-MM-dd", CultureInfo.InvariantCulture);
            Meeting meeting = new Meeting(meetingName, "desc", loggedInUser,Category.Hub,Visma_internship_2022.Type.InPerson,_startDate,_endDate);
            MeetingManager.s_meetings.Add(meeting);
            int meetingCount = MeetingManager.s_meetings.Count;
            input.LinesToRead.Add("testMeetingName");
            MeetingManager.DeleteMeeting(loggedInUser,input);

            int meetingCountAfterDelete = MeetingManager.s_meetings.Count;
            int expectedMeetingCount = meetingCount - 1;
            Assert.That(meetingCountAfterDelete, Is.EqualTo(expectedMeetingCount));
        }

        [Test]
        public void AddPerson_GivenValidData_ShouldPass()
        {
            FakeUserInput input = new FakeUserInput();
            Meeting meeting = new Meeting("TestMeet","Desc","Jonas",Category.Hub,Visma_internship_2022.Type.InPerson,
                DateTime.ParseExact("2022-06-14", "yyyy-MM-dd", null),
                DateTime.ParseExact("2022-06-24", "yyyy-MM-dd", null));
            MeetingManager.s_meetings.Add(meeting);
            int meetingAttendeesBefore = meeting.Attendees.Count;

            input.LinesToRead.Add("TestMeet");
            input.LinesToRead.Add("TestPersonName");
            input.LinesToRead.Add("2022-06-20");

            MeetingManager.AddPerson(input);
            int expectedCount = meetingAttendeesBefore + 1;
            Assert.That(meeting.Attendees.Count, Is.EqualTo(expectedCount));
        }

        [Test]
        public void AddPerson_GivenBadMeetingName_ShouldFail()
        {
            FakeUserInput input = new FakeUserInput();
            Meeting meeting = new Meeting("TestMeet", "Desc", "Jonas", Category.Hub, Visma_internship_2022.Type.InPerson,
                DateTime.ParseExact("2022-06-14", "yyyy-MM-dd", null),
                DateTime.ParseExact("2022-06-24", "yyyy-MM-dd", null));
            MeetingManager.s_meetings.Add(meeting);
            int meetingAttendeesBefore = meeting.Attendees.Count;

            input.LinesToRead.Add("BadMeetingName");
            input.LinesToRead.Add("TestPersonName");
            input.LinesToRead.Add("2022-06-20");

            MeetingManager.AddPerson(input);
            int expectedCount = meetingAttendeesBefore;
            Assert.That(meeting.Attendees.Count, Is.EqualTo(expectedCount));
        }

        [Test]
        public void RemovePerson_GivenValidData_ShouldPass()
        {
            FakeUserInput input = new FakeUserInput();
            Meeting meeting = new Meeting("TestMeet", "Desc", "Jonas", Category.Hub, Visma_internship_2022.Type.InPerson,
                DateTime.ParseExact("2022-06-14", "yyyy-MM-dd", null),
                DateTime.ParseExact("2022-06-24", "yyyy-MM-dd", null));
            meeting.Attendees.Add(new Attendee("Jonas", DateTime.ParseExact("2022-06-16", "yyyy-MM-dd", null)));
            MeetingManager.s_meetings.Add(meeting);

            int attendeeCount = meeting.Attendees.Count;


            input.LinesToRead.Add("TestMeet");
            input.LinesToRead.Add("Jonas");

            MeetingManager.RemovePerson("Arturas",input);
            int expectedCount = attendeeCount - 1;
            Assert.That(meeting.Attendees.Count, Is.EqualTo(expectedCount));
        }
    }
}