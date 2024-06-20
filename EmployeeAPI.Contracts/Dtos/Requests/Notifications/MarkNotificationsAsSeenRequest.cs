namespace EmployeeAPI.Contracts.Dtos.Requests.Notifications
{
    public class MarkNotificationsAsSeenRequest
    {
        public List<int> Ids { get; set; }
        public MarkNotificationsAsSeenRequest()
        {
            Ids = [];
        }
    }
}
