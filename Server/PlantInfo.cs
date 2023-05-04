namespace WWWatering
{
    public class PlantInfo
    {
        public string? ErrorInfo { get; set; }
        public DateTime? LastWatering { get; set; }
        public DateTime? LastChecked { get; private set; }

        private int? _humidity = null;
        public int? Humidity
        { 
            get 
            { 
                return _humidity;
            } 
            set 
            { 
                LastChecked = DateTime.UtcNow;
                _humidity = value;
            }
        }
    }
}
