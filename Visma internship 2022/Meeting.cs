using System.Text.Json.Serialization;
namespace Visma_internship_2022
{
    internal class Meeting
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string ResponsiblePerson { get; private set; }
        public Category Category { get; private set; }
        public Type Type { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public List<Attendee> Attendees { get; private set; }

        public Meeting(string name, string description, string responsiblePerson, Category category, Type type, DateTime startDate, DateTime endDate)
        {
            Name = name;
            Description = description;
            ResponsiblePerson = responsiblePerson;
            Category = category;
            Type = type;
            StartDate = startDate;
            EndDate = endDate;
            Attendees = new List<Attendee>();
        }
        [JsonConstructor]
        public Meeting(string name, string description, string responsiblePerson, Category category, Type type, DateTime startDate, DateTime endDate, List<Attendee> attendees)
        {
            Name = name;
            Description = description;
            ResponsiblePerson = responsiblePerson;
            Category = category;
            Type = type;
            StartDate = startDate;
            EndDate = endDate;
            Attendees = attendees;
        }

        public override string ToString() 
        {
            return "-----------------------------------------\n" +
                $"Name: {Name}\n" +
                $"Description: {Description}\n" +
                $"Responsible user: {ResponsiblePerson}\n" +
                $"Category: {Category} \n" +
                $"Type: {Type} \n" +
                $"Starting date: {StartDate:yyyy-MM-dd} \n" +
                $"Ending date: {EndDate:yyyy-MM-dd} \n" +
                $"Number of attendees: { Attendees.Count } \n" +
                "-----------------------------------------";
        }
    }

    public enum Category
    {
        CodeMonkey = 1,
        Hub = 2,
        Short = 3,
        TeamBuilding = 4
    }

    public enum Type
    {
        Live = 1,
        InPerson = 2
    }
}
